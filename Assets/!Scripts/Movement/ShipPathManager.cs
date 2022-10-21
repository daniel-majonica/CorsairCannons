using EmptySkull.Management;
using EmptySkull.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ShipPathManager : ManagerModule
{
    public enum PathGenerationState
    {
        Inactive,
        Starting,
        Generating
    }

    public struct ShipPathWaypoint
    {
        public Vector2 Position;
        public Vector2 Heading;

        public ShipPathWaypoint(Vector2 pos, Vector2 heading)
        {
            Position = pos;
            Heading = heading;
        }
    }

    public struct ShipPath
    {
        public const float CurvatureSampleOffset = .25f;

        public float StepSize;
        public List<ShipPathWaypoint> Waypoints;

        public float PathLength => (Waypoints.Count - 1) * StepSize;

        public ShipPath(ShipPathWaypoint initalWaypoint, float stepSize)
        {
            StepSize = stepSize;
            Waypoints = new List<ShipPathWaypoint> { initalWaypoint };
        }

        public Vector2 Evaluate(float pathPosition, out float curvature)
        {
            if (!ValidatePathPosition(pathPosition, PathLength))
                throw new ArgumentException($"Position '{pathPosition}' not valid in path.");

            Vector2 pointPosition = SamplePointInternal(Waypoints, pathPosition, StepSize, out Vector2 pointNormal);

            Vector2? normalBefore = null;
            Vector2? normalAfter = null;

            float samplePositionBefore = pathPosition - CurvatureSampleOffset;
            if (ValidatePathPosition(samplePositionBefore, PathLength))
            {
                SamplePointInternal(Waypoints, samplePositionBefore, StepSize, out Vector2 normal);
                normalBefore = normal;
            }

            float samplePositionAfter = pathPosition + CurvatureSampleOffset;
            if(ValidatePathPosition(samplePositionAfter, PathLength))
            {
                SamplePointInternal(Waypoints, samplePositionAfter, StepSize, out Vector2 normal);
                normalAfter = normal;
            }

            curvature = 0;

            if (normalBefore.HasValue)
                curvature += (1 - (Vector2.Dot(normalBefore.Value.normalized, pointNormal.normalized) + 1) * .5f) * .5f;

            if (normalAfter.HasValue)
                curvature += (1 - (Vector2.Dot(normalAfter.Value.normalized, pointNormal.normalized) + 1) * .5f) * .5f;

            return pointPosition;

            Vector2 SamplePointInternal(List<ShipPathWaypoint> waypoints, float pathPosition, float stepSize, out Vector2 normal)
            {
                float indexAccurate = pathPosition / stepSize;
                int index = (int)indexAccurate;

                ShipPathWaypoint a;
                ShipPathWaypoint? b = null;

                a = waypoints[index];

                float localIndex = indexAccurate - index;

                //Debug.Log($"[Max] Out of range: {index + 1} > {waypoints.Count} | raw-index: {indexAccurate} | raw input: {pathPosition}");
                if (Mathf.Abs(localIndex) > Mathf.Epsilon)
                    b = waypoints[index + 1];

                Vector2 point;

                if (!b.HasValue)
                {
                    point = a.Position;
                    normal = Vector2.Perpendicular(a.Heading);
                }
                else
                {
                    point = Bezier2DHelper.Sample(a.Position, a.Heading, b.Value.Position, b.Value.Heading, localIndex, out normal);
                }

                return point;
            }

            bool ValidatePathPosition(float pathPosition, float pathLength)
                => pathPosition >= 0 && pathPosition < pathLength;
        }
    }

    public float MinTargetUpdateDistanceSqr => _minTargetUpdateDistance * _minTargetUpdateDistance;

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.DisableIf("@UnityEngine.Application.isPlaying")]
#endif
    [SerializeField] private float _minStartPathDistance = 2f;
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.DisableIf("@UnityEngine.Application.isPlaying")]
#endif
    [SerializeField] private float _pathStepSize = .5f;
    [SerializeField] private float _minTargetUpdateDistance = .05f;

    [SerializeField, Range(0,2f)] private float _bezierSmoothing = 1;
    [SerializeField] private bool _autoadjustHeading = true;

    [SerializeField] private bool _waitWhileDrawPath = true;

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.Title("Debugging")]
    [Sirenix.OdinInspector.ShowInInspector(), Sirenix.OdinInspector.ReadOnly()]
#endif
    private PathGenerationState _state;

    private ShipPath? _buildingPath;
    private bool IsBuildingPath
    {
        get => _buildingPath.HasValue;
        set => _buildingPath = value ? throw new InvalidOperationException("Cannot set 'isBuildPath' to true without having a valid starting path point.") : null;
    }
    private ShipPath BuildingPath
    {
        get => _buildingPath.Value;
        set => _buildingPath = value;
    }

    private Ship _buildingPathShip;

    private float? _minStartPathDistSqrValue;
    private float MinStartPathDistSqr
    {
        get
        {
            if (!_minStartPathDistSqrValue.HasValue)
                _minStartPathDistSqrValue = _minStartPathDistance * _minStartPathDistance;
            return _minStartPathDistSqrValue.Value;
        }
    }

    private float? _pathStepSizeSqrValue;
    private float PathSetepSizeSqr
    {
        get
        {
            if (!_pathStepSizeSqrValue.HasValue)
                _pathStepSizeSqrValue = _pathStepSize * _pathStepSize;
            return _pathStepSizeSqrValue.Value;
        }
    }


    protected virtual void Update()
    {
        if (_state != PathGenerationState.Inactive)
            UpdatePathWaypoints();
    }

    protected virtual void OnEnable()
    {
        Manager.Use<InputManager>().OnMouseLeftPressed += HandelTryStartNewPath;
        Manager.Use<InputManager>().OnMouseLeftReleased += EndCurrentPathBuilding;
    }

    protected virtual void OnDisable()
    {
        Manager.Use<InputManager>().OnMouseLeftPressed -= HandelTryStartNewPath;
        Manager.Use<InputManager>().OnMouseLeftReleased -= EndCurrentPathBuilding;
    }


    protected virtual void OnDrawGizmos()
    {
        switch(_state)
        {
            case PathGenerationState.Inactive:
                return;
            case PathGenerationState.Starting:
                using (new GizmoColorSwitcher(Color.yellow))
                {
                    Gizmos.DrawWireSphere(_buildingPathShip.transform.position, .5f);
                    Gizmos.DrawLine(_buildingPathShip.transform.position, Manager.Use<InputManager>().FocusPointOnWater);
                }
                break;
            case PathGenerationState.Generating:
                using (new GizmoColorSwitcher(Color.red))
                {
                    Gizmos.DrawWireSphere(_buildingPathShip.transform.position, .5f);

                    foreach (ShipPathWaypoint waypoint in BuildingPath.Waypoints)
                    {
                        Gizmos.DrawSphere(PlanarProjectionHelper.FromPlanarVector(waypoint.Position), .05f);
                        Gizmos.DrawRay(PlanarProjectionHelper.FromPlanarVector(waypoint.Position), PlanarProjectionHelper.FromPlanarVector(waypoint.Heading));
                    }
                }
                for (float samplePoint = 0; samplePoint < BuildingPath.PathLength; samplePoint += .05f)
                {
                    Vector2 point = BuildingPath.Evaluate(samplePoint, out float curvature);

                    using(new GizmoColorSwitcher(Color.Lerp(Color.white, Color.red, Mathf.Clamp01(curvature * 4f))))
                    {
                        Gizmos.DrawSphere(PlanarProjectionHelper.FromPlanarVector(point), .01f);
                    }
                }
                break;
        }
    }


    private void HandelTryStartNewPath()
    {
        if (IsBuildingPath)
            return; //Cannot start path when already building a path

        if (!TryGetPathStartingShip(out Ship selectedShip))
            return; //Cannot start path when no ship is selected

        if (PlanarProjectionHelper.DistanceSqr(selectedShip.transform.position, Manager.Use<InputManager>().FocusPointOnWater2D) > MinStartPathDistSqr)
            return; //Cannot start path when starting path distance is to far away.

        _buildingPathShip = selectedShip;
        _buildingPathShip.OnPathEndReached += HandelBuildingShipReachedPathEnd;

        _state = PathGenerationState.Starting;

        bool TryGetPathStartingShip(out Ship ship)
        {
            ISelectable selected = Manager.Use<SelectionManager>().Selected;
            if (selected == null || selected is not Ship s)
            {
                ship = null;
                return false;
            }
            ship = s;
            return true;
        }
    }

    private void UpdatePathWaypoints()
    {
        switch(_state)
        {
            case PathGenerationState.Inactive: //Do not process anything
                return; 
            case PathGenerationState.Starting: //Try generate first path waypoint (and assign to ship)

                Vector2 shipPlanarPosition = PlanarProjectionHelper.AsPlanarVector(_buildingPathShip.transform.position);
                Vector2 toNextWaypointOrigin = PlanarProjectionHelper.Between(shipPlanarPosition, Manager.Use<InputManager>().FocusPointOnWater2D);

                if (toNextWaypointOrigin.sqrMagnitude < PathSetepSizeSqr)
                    return; //Current point to close to ship-position to start path

                Vector2 targetPosition = shipPlanarPosition + toNextWaypointOrigin.normalized * _pathStepSize;

                ShipPathWaypoint pathOrigin = new ShipPathWaypoint(shipPlanarPosition, CalculateHeadingForward());
                ShipPathWaypoint firstWaypoint = new ShipPathWaypoint(targetPosition, CalculateHeadingBetween(pathOrigin.Position, targetPosition));

                BuildingPath = new ShipPath(pathOrigin, _pathStepSize); //Add inital point for ship
                BuildingPath.Waypoints.Add(firstWaypoint); //Add first waypoint to approach

                _buildingPathShip.AssignNewPath(BuildingPath);
                if (_waitWhileDrawPath)
                    _buildingPathShip.SetPathWaiting(true);

                _state = PathGenerationState.Generating;

                break;
            case PathGenerationState.Generating: //Try geberate further path waypoints

                ShipPathWaypoint lastWaypoint = BuildingPath.Waypoints[BuildingPath.Waypoints.Count - 1];
                Vector2 lastWaypointPosition = lastWaypoint.Position;

                Vector2 toNextWaypoint = PlanarProjectionHelper.Between(lastWaypointPosition, Manager.Use<InputManager>().FocusPointOnWater2D);

                if (toNextWaypoint.sqrMagnitude < PathSetepSizeSqr)
                    return; //Not enough distant to last waypoint to create new one.

                Vector2 newWaypointPosition = lastWaypointPosition + toNextWaypoint.normalized * _pathStepSize;

                ShipPathWaypoint newWaypoint = new ShipPathWaypoint(newWaypointPosition, CalculateHeadingBetween(lastWaypointPosition, newWaypointPosition));

                BuildingPath.Waypoints.Add(newWaypoint);

                if (_autoadjustHeading)
                {
                    lastWaypoint.Heading = (lastWaypoint.Heading + toNextWaypoint).normalized * _pathStepSize * .5f * _bezierSmoothing;
                    BuildingPath.Waypoints[BuildingPath.Waypoints.Count - 2] = lastWaypoint;
                }
                break;
        }

        Vector2 CalculateHeadingForward()
            => HeadingVector(PlanarProjectionHelper.AsPlanarVector(_buildingPathShip.transform.forward));
        Vector2 CalculateHeadingBetween(Vector2 origin, Vector2 target)
            => HeadingVector(target - origin);

        Vector2 HeadingVector(Vector2 v)
            => v.normalized * _pathStepSize * .5f * _bezierSmoothing;
    }

    private void EndCurrentPathBuilding()
    {
        IsBuildingPath = false;
        _state = PathGenerationState.Inactive;

        if(_buildingPathShip != null)
        {
            _buildingPathShip.SetPathWaiting(false);

            _buildingPathShip.OnPathEndReached -= HandelBuildingShipReachedPathEnd;
            _buildingPathShip = null;
        }
    }

    private void HandelBuildingShipReachedPathEnd()
    {
        if (_buildingPathShip == null)
            throw new InvalidOperationException("Cannot handel new path for active ship. No ship active");

        _buildingPath = null;

        _state = PathGenerationState.Starting;
    }
}
