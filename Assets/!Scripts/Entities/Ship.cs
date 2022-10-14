using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Ship : Entity, ISelectable
{
    private NavMeshAgent _agent;

    public GameObject SelectableObject => gameObject;
    public Vector2 Heading => PlanarProjectionHelper.AsPlanarVector(transform.forward);


    [SerializeField] private float _speed = 1f;
    public float Speed => _speed;

    protected virtual void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

}
