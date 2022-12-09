using EmptySkull.Management;
using EmptySkull.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(Ship))]
public class ShipAutoattackController : MonoBehaviour
{
    [System.Serializable]
    public class ShipAttackEmitter
    {
        public Transform AttackOrigin;

        public Ray TargetRay => AttackOrigin.GetForwardRay();
    }

    [DisableIf("@UnityEngine.Application.isPlaying")]
    [SerializeField] private float _detectionRange = 5f;
    [SerializeField] private NullableFloat _detectionRateSeconds = new NullableFloat() { SetToNull = true };
    [SerializeField] private ShipAttackEmitter[] _emitter;


    private ITargetable[] _targetablesInRange;
    private float _detectionRangeSqr;


    private Ship _ship;
    private float _detectionTimer;

    protected virtual void OnDrawGizmos()
    {
        using(new GizmoColorSwitcher(new Color(1f,.5f,0)))
        {
            foreach(ShipAttackEmitter emitter in _emitter)
            {
                if(emitter == null)
                    continue;

                Gizmos.DrawRay(emitter.TargetRay);
            }
        }

        using(new GizmoColorSwitcher(new Color(0, 1f, 0, .25f)))
        {
            Gizmos.DrawWireSphere(transform.position, _detectionRange);
        }

        using (new GizmoColorSwitcher(new Color(1f, 0, 0, .25f)))
        {
            if (_targetablesInRange != null)
            {
                foreach (ITargetable targetable in _targetablesInRange)
                {
                    Gizmos.DrawWireSphere(targetable.WorldPosition, .5f);
                }
            }
        }
    }

    protected virtual void Awake()
    {
        _detectionRangeSqr = _detectionRange * _detectionRange;

        _ship = GetComponent<Ship>();
    }

    protected virtual void Start()
    {
        UpdateTargetsInRange();
    }

    protected virtual void Update()
    {
        if(_detectionRateSeconds.TryGetValue(out float detectionRate))
        {
            _detectionTimer += Time.deltaTime;

            if (_detectionTimer < detectionRate)
                return;

            _detectionTimer -= detectionRate;
        }

        UpdateTargetsInRange();
    }


    private void UpdateTargetsInRange()
    {
        _targetablesInRange = Manager.Use<TargetPositionManager>().GetTargetsInRangeSqr(_ship, _detectionRangeSqr);
    }
}
