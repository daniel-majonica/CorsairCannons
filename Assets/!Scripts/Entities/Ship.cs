using EmptySkull.Management;
using System;
using UnityEngine;

[RequireComponent(typeof(ShipPathFollower))]
public class Ship : Entity, ISelectable, ITargetable
{
    public GameObject SelectableObject => gameObject;
    public Vector3 WorldPosition => transform.position;


    private ShipPathFollower _pathFollowerValue;
    private ShipPathFollower PathFollower
    {
        get
        {
            if (_pathFollowerValue == null)
                _pathFollowerValue = GetComponent<ShipPathFollower>();
            return _pathFollowerValue;
        }
    }

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.Title("Debugging")]
    [Sirenix.OdinInspector.ShowInInspector, Sirenix.OdinInspector.ReadOnly]
#endif
    public float CurrentSpeed => PathFollower.EffectiveSpeed;

    public event Action OnPathEndReached;

    private Action _pathFinishedAction;

    protected virtual void OnEnable()
    {
        _pathFinishedAction = () => OnPathEndReached?.Invoke();
        PathFollower.OnPathFinished += _pathFinishedAction;

        Manager.Use<TargetPositionManager>().RegisterTarget(this);
    }

    protected virtual void OnDisable()
    {
        PathFollower.OnPathFinished -= _pathFinishedAction;

        Manager.Use<TargetPositionManager>().DeregisterTarget(this);
    }


    public void AssignNewPath(ShipPathManager.ShipPath path)
    {
        PathFollower.AssignPath(path, _stats.MaxSpeed);
    }

    public void SetPathWaiting(bool state)
    {
        if (state)
            PathFollower.StartWaiting();
        else
            PathFollower.EndWaiting();
    }
}
