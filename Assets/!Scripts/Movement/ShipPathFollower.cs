using DG.Tweening;
using System;
using UnityEngine;

public class ShipPathFollower : MonoBehaviour
{
    private float _maxSpeed = 1f; //Unity-units per second, controlled by stats json
    [SerializeField, Range(0,1f)] private float WaitingSpeedModifier = 0f;

    [SerializeField] private AnimationCurve _speedModByCurvature = AnimationCurve.Linear(0f, 1f, 1f, 0f);

    [SerializeField] private bool _useJumpSmoothing = true;
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.HideIf("@!_useJumpSmoothing")]
    [Sirenix.OdinInspector.DisableIf("@UnityEngine.Application.isPlaying")]
#endif
    [SerializeField] private float _jumpSmoothingThreshold = .05f;
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.HideIf("@!_useJumpSmoothing")]
#endif
    [SerializeField] private float _jumpSmoothingStrength = .25f;

    [SerializeField] private bool _useTweening = true;
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.HideIf("@!_useTweening")]
#endif
    [SerializeField] private float _tweenTime = 1f;


    private float _currentSpeedModifier;


    private ShipPathManager.ShipPath? _path;
    private bool HasPath => _path.HasValue;
    private ShipPathManager.ShipPath Path => _path.Value;
    private float _lastPathPosition;

    public float EffectiveSpeed { get; private set; }


    public event Action OnPathFinished;
    public event Action OnPathCanceled;
    public event Action OnPathEnded;

    private float _jumpSmoothingThresholdSqr;
    private Vector3 _jumpSmoothingVelocity;

    protected virtual void Awake()
    {
        _jumpSmoothingThresholdSqr = _jumpSmoothingThreshold * _jumpSmoothingThreshold;
    }

    protected virtual void Update()
    {
        ExecuteMovementUpdate();
    }


    public void AssignPath(ShipPathManager.ShipPath path, float speed)
    {
        _lastPathPosition = 0;
        _path = path;
        _maxSpeed = speed;
    }

    public void RemovePath(bool isCancel = true)
    {
        _path = null;
        _lastPathPosition = 0;

        if(_useTweening)
            _currentSpeedModifier = 0;

        OnPathEnded?.Invoke();

        if (isCancel)
            OnPathCanceled?.Invoke();
    }

    public void StartWaiting()
    {
        SetRelativeSpeed(WaitingSpeedModifier);
    }

    public void EndWaiting()
    {
        SetRelativeSpeed(1f);
    }


    public void SetRelativeSpeed(float modifier, bool neverTween = false)
    {
        if (modifier < 0 || modifier > 1)
            throw new ArgumentException("Relative speed modifier must be between 0 and 1!");

        if(neverTween || !_useTweening)
            _currentSpeedModifier = modifier;
        else
            DOTween.To(() => EffectiveSpeed / _maxSpeed, x => _currentSpeedModifier = x, modifier, _tweenTime);
    }


    private void ExecuteMovementUpdate()
    {
        if (!HasPath)
            return;

        _lastPathPosition += EffectiveSpeed * Time.deltaTime;

        if (_lastPathPosition > Path.PathLength)
        {
            RemovePath(false);

            OnPathFinished?.Invoke();
            return;
        }

        Vector2 targetPosition = Path.Evaluate(_lastPathPosition, out float curvature);

        EffectiveSpeed = _maxSpeed * _currentSpeedModifier * _speedModByCurvature.Evaluate(curvature);

        Vector3 targetPos3D = PlanarProjectionHelper.FromPlanarVector(targetPosition);
        Vector3 targetFaceing = targetPos3D - transform.position;

        if(_useJumpSmoothing && (targetPos3D - transform.position).sqrMagnitude > _jumpSmoothingThresholdSqr)
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPos3D, ref _jumpSmoothingVelocity, _jumpSmoothingStrength);
        }
        else
        {
            _jumpSmoothingVelocity = Vector3.zero;
            transform.position = targetPos3D;
        }

        if (targetFaceing.normalized.sqrMagnitude > Mathf.Epsilon) //Facing away from zero required to calculate rotation
            transform.rotation = Quaternion.LookRotation(targetFaceing.normalized, Vector3.up);
    }
}
