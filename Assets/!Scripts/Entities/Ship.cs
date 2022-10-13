using EmptySkull.Management;
using EmptySkull.Utilities;
using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Ship : Entity, ISelectable
{
    private NavMeshAgent _agent;

    public GameObject SelectableObject => gameObject;
    public Vector2 Heading => PlanarProjectionHelper.AsPlanarVector(transform.forward);

    private Vector3? _target;
    private bool HasTarget => _target.HasValue;
    
    private int _targetIndex;

    protected virtual void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    protected virtual void Update()
    {
        if (HasTarget)
            UpdateTarget();
    }

    protected virtual void OnDrawGizmos()
    {
        if (HasTarget)
        {
            using (new GizmoColorSwitcher(Color.yellow))
            {
                Gizmos.DrawWireSphere(_target.Value, .25f);
            }
        }
    }


    public void SetTarget(Vector2 target, int targetIndex)
    {
        if (!NavMesh.SamplePosition(PlanarProjectionHelper.FromPlanarVector(target), out NavMeshHit hit, /*Manager.Use<ShipPathManager>().MinTargetUpdateDistanceSqr*/ 1, NavMesh.AllAreas) /*|| PlanarProjectionHelper.DistanceSqr(hit.position, target) > 1*/)
            throw new ArgumentException($"Target position '{target}' is not valid on the nav-mesh.");

        _target = hit.position;
        _targetIndex = targetIndex;

        _agent.isStopped = false;
        _agent.destination = _target.Value;
    }

    public void ClearTarget()
    {
        _agent.isStopped = true;
        _target = null;
    }

    private void UpdateTarget()
    {
        float distSqr = PlanarProjectionHelper.DistanceSqr(_target.Value, transform.position);

        if (distSqr <= Manager.Use<ShipPathManager>().MinTargetUpdateDistanceSqr)
            Manager.Use<ShipPathManager>().ReportShipPathUpdate(this, _targetIndex);
    }
}
