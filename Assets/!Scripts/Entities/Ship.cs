using UnityEngine;

[RequireComponent(typeof(ShipPathFollower))]
public class Ship : Entity, ISelectable
{
    public GameObject SelectableObject => gameObject;


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

    public void AssignNewPath(ShipPathManager.ShipPath path)
    {
        PathFollower.AssignPath(path);
    }
}
