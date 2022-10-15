using UnityEngine;

public class Ship : Entity, ISelectable
{
    public GameObject SelectableObject => gameObject;
    public Vector2 Heading => PlanarProjectionHelper.AsPlanarVector(transform.forward);


    [SerializeField] private float _speed = 1f;
    public float Speed => _speed;
}
