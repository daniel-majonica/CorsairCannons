using EmptySkull.Management;
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
        _navAgent.destination = Manager.Use<InputManager>().FocusPointOnWater;
    }
}
