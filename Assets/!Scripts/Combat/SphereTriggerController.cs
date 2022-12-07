using EmptySkull.Utilities;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

[InfoBox("This component forces the sphere collider on this gameobject to be a trigger!")]
[RequireComponent(typeof(SphereCollider))]
public class SphereTriggerController : MonoBehaviour
{
    private SphereCollider _sphereColliderValue;
    private SphereCollider SphereCollider
    {
        get
        {
            if(_sphereColliderValue == null)
                _sphereColliderValue = GetComponent<SphereCollider>();
            if (_sphereColliderValue == null)
            {
                _sphereColliderValue = gameObject.AddComponent<SphereCollider>();
                _sphereColliderValue.isTrigger = true;
            }
            return _sphereColliderValue;
        }
    }

    public float Radius
    {
        get => SphereCollider.radius;
        set => SphereCollider.radius = value;
    }

    public event Action<Collider> OnTriggerEntered;
    public event Action<Collider> OnTriggerExited;


    protected virtual void OnValidate()
    {
        ForceColliderToTrigger();
    }

    protected virtual void OnDrawGizmosSelected()
    {
        using(new GizmoColorSwitcher(new Color(0,1f,0,.25f)))
        {
            Gizmos.DrawWireSphere(transform.position + SphereCollider.center, Radius);
        }
    }

    protected virtual void Update()
    {
        ForceColliderToTrigger();
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        Debug.Log("[Max] Trigger entered: " + other.gameObject.name);
        OnTriggerEntered?.Invoke(other);
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        OnTriggerExited?.Invoke(other);
    }


    private void ForceColliderToTrigger()
    {
        if (SphereCollider != null && !SphereCollider.isTrigger)
            SphereCollider.isTrigger = true;
    }
}
