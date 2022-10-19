using UnityEngine;

public class ShipPathFollower : MonoBehaviour
{
    public float MaxSpeed = 1f; //Unity-units per second

    [SerializeField] private AnimationCurve _speedModByCurvature = AnimationCurve.Linear(0f, 1f, 1f, 0f);

    private ShipPathManager.ShipPath? _path;
    private bool HasPath => _path.HasValue;
    private ShipPathManager.ShipPath Path => _path.Value;
    private float _lastPathPosition;

    public float EffectiveSpeed { get; private set; }


    protected virtual void Update()
    {
        ExecuteMovementUpdate();
    }


    public void AssignPath(ShipPathManager.ShipPath path)
    {
        _path = path;
    }

    public void RemovePath()
    {
        _path = null;
        _lastPathPosition = 0;
    }


    private void ExecuteMovementUpdate()
    {
        if (!HasPath)
            return;

        _lastPathPosition += EffectiveSpeed * Time.deltaTime;

        if (_lastPathPosition > Path.PathLength)
        {
            RemovePath();
            return;
        }

        Vector2 targetPosition = Path.Evaluate(_lastPathPosition, out float curvature);

        EffectiveSpeed = MaxSpeed * _speedModByCurvature.Evaluate(curvature);

        Vector3 targetPos3D = PlanarProjectionHelper.FromPlanarVector(targetPosition);
        Vector3 targetFaceing = targetPos3D - transform.position;

        transform.position = targetPos3D;
        if (targetFaceing.normalized.sqrMagnitude > Mathf.Epsilon) //Facing away from zero required to calculate rotation
            transform.rotation = Quaternion.LookRotation(targetFaceing.normalized, Vector3.up);
    }
}
