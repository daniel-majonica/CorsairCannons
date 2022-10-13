using EmptySkull.Management;
using EmptySkull.Utilities;
using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class SimpleClickToMove : MonoBehaviour
{
    private NavMeshAgent _navAgent;

    protected virtual void Awake()
    {
        _navAgent = GetComponent<NavMeshAgent>();
    }

    protected virtual void OnEnable()
    {
        Manager.Use<InputManager>().OnMouseLeftPressed += HandelMousePressed;
    }

    protected virtual void OnDisable()
    {
        Manager.Use<InputManager>().OnMouseLeftPressed -= HandelMousePressed;
    }


    private void HandelMousePressed()
    {
        _navAgent.destination = ReadCurrentTargetWorldPosition();

        Vector3 ReadCurrentTargetWorldPosition()
        {
            if(!UnityMath.TryIntersectionPlaneRay(new Plane(Vector3.up, 0f), Manager.Use<InputManager>().MouseFocusRay, out Vector3 target))
                throw new InvalidOperationException("Could not find a valid target position from current input focus ray.");
            return target;
        }
    }
}
