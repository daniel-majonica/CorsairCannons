using EmptySkull.Utilities;
using System.Collections.Generic;
using UnityEngine;

public class ShipAutoattackController : MonoBehaviour
{
    [System.Serializable]
    public class ShipAttackEmitter
    {
        public Transform AttackOrigin;

        public Ray TargetRay => AttackOrigin.GetForwardRay();
    }

    [SerializeField] private float _detectionRange = 5f;
    [SerializeField] private ShipAttackEmitter[] _emitter;

    private SphereTriggerController _sphereTrigger;

    private HashSet<ITargetable> _targetablesInRange = new HashSet<ITargetable>();


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

        using (new GizmoColorSwitcher(Color.red))
        {
            foreach(ITargetable targetable in _targetablesInRange)
            {
                Gizmos.DrawWireSphere(targetable.TargetObject.transform.position, .5f);
            }
        }
    }

    protected virtual void Awake()
    {
        GameObject triggerObj = new GameObject("- SphereTrigger -");
        MakeChild(triggerObj.transform);

        _sphereTrigger = triggerObj.AddComponent<SphereTriggerController>();
        _sphereTrigger.Radius = _detectionRange;

        void MakeChild(Transform tChild)
        {
            tChild.SetParent(transform);
            tChild.localPosition = Vector3.zero;
            tChild.rotation = Quaternion.identity;
            tChild.localScale = Vector3.zero;
        }
    }

    protected virtual void OnEnable()
    {
        _sphereTrigger.OnTriggerEntered += HandelCollisionDetected;
        _sphereTrigger.OnTriggerExited += HandelCollisionDetectionLost;
    }

    protected virtual void OnDisable()
    {
        _sphereTrigger.OnTriggerEntered -= HandelCollisionDetected;
        _sphereTrigger.OnTriggerExited -= HandelCollisionDetectionLost;
    }


    private void HandelCollisionDetected(Collider detectedCollider)
    {
        ITargetable[] targetableComponents = detectedCollider.gameObject.GetComponentsInChildren<ITargetable>();
        if (targetableComponents.Length >= 1)
        {
            _targetablesInRange.Add(targetableComponents[0]);

            if (targetableComponents.Length > 1)
                Debug.LogWarning($"[{nameof(ShipAutoattackController)} on: {gameObject.name}] More that 1 '{nameof(ITargetable)}' on the same GameObject detected. " +
                    $"This is not supported. Only the first scrip (of type '{targetableComponents[0].GetType().Name}') will be assigned as target.");
        }
    }

    private void HandelCollisionDetectionLost(Collider detectedCollider)
    {
        ITargetable[] targetableComponents = detectedCollider.gameObject.GetComponentsInChildren<ITargetable>();

        if(targetableComponents.Length >= 1)
        {
            _targetablesInRange.Remove(targetableComponents[0]);
        }
    }
}
