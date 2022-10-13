using EmptySkull.Management;
using EmptySkull.Utilities;
using System.Collections.Generic;
using UnityEngine;

public class ShipPathManager : ManagerModule
{
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

    public struct ShipPath //No custom structure required?
    {
        public ShipPathWaypoint[] Waypoints;
    }

    public float MinTargetUpdateDistanceSqr => _minTargetUpdateDistance * _minTargetUpdateDistance;

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.DisableIf("@UnityEngine.Application.isPlaying")]
#endif
    [SerializeField] private float _minStartPathDistance = 2f;
    [SerializeField] private float _pathStepSize = .5f;
    [SerializeField] private float _minTargetUpdateDistance = .05f;

    private Dictionary<Ship, ShipPath> _activePaths = new Dictionary<Ship, ShipPath>();

    private bool _isBuildingPath;
    private List<ShipPathWaypoint> _currentBuildingWaypoints = new List<ShipPathWaypoint>();
    private Ship _currentBuildingShip;

    private float _minStartPathDistSqr;


    protected override void Awake()
    {
        base.Awake();

        _minStartPathDistSqr = _minStartPathDistance * _minStartPathDistance;
    }

    protected virtual void Update()
    {
        if (_isBuildingPath)
            UpdatePathBuilding();
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
        //Draw currently building path
        if(_isBuildingPath)
        {
            using (new GizmoColorSwitcher(Color.red))
            {
                if(_currentBuildingShip != null)
                    Gizmos.DrawWireSphere(_currentBuildingShip.transform.position, 1f);

                foreach (ShipPathWaypoint waypoint in _currentBuildingWaypoints)
                {
                    Gizmos.DrawSphere(PlanarProjectionHelper.FromPlanarVector(waypoint.Position), .1f);
                    Gizmos.DrawRay(PlanarProjectionHelper.FromPlanarVector(waypoint.Position), PlanarProjectionHelper.FromPlanarVector(waypoint.Heading) * .2f);
                }
            }
        }
    }


    public void ReportShipPathUpdate(Ship ship, int reachedPathIndex)
    {
        if (!_activePaths.TryGetValue(ship, out ShipPath path)) //No active path for ship
        {
            ship.ClearTarget();
        }
        else if (reachedPathIndex +1 >= path.Waypoints.Length) //Ship reached end of path
        {
            ship.ClearTarget();
            _activePaths.Remove(ship);
        }
        else
        {
            int newTargetIndex = reachedPathIndex +1;
            ship.SetTarget(path.Waypoints[newTargetIndex].Position, newTargetIndex);
        }
    }


    private void HandelTryStartNewPath()
    {
        if (_isBuildingPath)
            return; //Cannot build path while already building

        if (!TryGetCurrentSelectedShipPosition(out Ship selectedShip, out Vector2 selectedShipPos))
            return; //Cannot build path when no ship is selected

        if (PlanarProjectionHelper.DistanceSqr(selectedShipPos, Manager.Use<InputManager>().FocusPointOnWater2D) > _minStartPathDistSqr)
            return; //Cannot start path to far away from selected ship

        StartPathBuilding(selectedShip);

        bool TryGetCurrentSelectedShipPosition(out Ship ship, out Vector2 pos)
        {
            pos = Vector2.zero;
            ISelectable selectedEntity = Manager.Use<SelectionManager>().Selected;

            ship = (Ship)selectedEntity;
            if (ship == null)
                return false;

            if (selectedEntity == null)
                return false;

            pos = PlanarProjectionHelper.AsPlanarVector(selectedEntity.SelectableObject.transform.position);
            return true;
        }
    }

    private void StartPathBuilding(Ship ship)
    {
        if(_activePaths.ContainsKey(ship))
        {
            _activePaths.Remove(ship);
        }

        _currentBuildingWaypoints.Clear();

        _currentBuildingWaypoints.Add(new ShipPathWaypoint(PlanarProjectionHelper.AsPlanarVector(ship.transform.position), ship.Heading.normalized));
        _currentBuildingShip = ship;

        _isBuildingPath = true;
    }

    private void UpdatePathBuilding()
    {
        Vector2 lastWaypointPos = _currentBuildingWaypoints[_currentBuildingWaypoints.Count - 1].Position;
        Vector2 lastToNext = PlanarProjectionHelper.Between(lastWaypointPos, Manager.Use<InputManager>().FocusPointOnWater2D);
        if (lastToNext.sqrMagnitude < _pathStepSize)
            return; //To close to last point for new point

        //TODO Check angle

        _currentBuildingWaypoints.Add(new ShipPathWaypoint(lastWaypointPos + lastToNext.normalized * _pathStepSize, lastToNext.normalized));
    }

    private void EndCurrentPathBuilding()
    {
        if (!_isBuildingPath)
            return;

        if(_currentBuildingShip != null && _currentBuildingWaypoints.Count > 1)
        {
            _activePaths.Add(_currentBuildingShip, new ShipPath { Waypoints = _currentBuildingWaypoints.ToArray() });
            _currentBuildingShip.SetTarget(_activePaths[_currentBuildingShip].Waypoints[1].Position, 1);
        }

        _currentBuildingWaypoints.Clear();
        _currentBuildingShip = null;

        _isBuildingPath = false;
    }
}
