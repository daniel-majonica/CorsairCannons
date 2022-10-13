using EmptySkull.Management;
using EmptySkull.Utilities;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : ManagerModule //TODO Rework to more event-based approach
{
    public Vector2 MousePosition => Mouse.current.position.ReadValue();
    public Ray MouseFocusRay => InputCamera.ScreenPointToRay(MousePosition);

    public Vector3 FocusPointOnWater
    {
        get
        {
            if (!UnityMath.TryIntersectionPlaneRay(new Plane(Vector3.up, Manager.Use<GameManager>().WaterLevelY), MouseFocusRay, out Vector3 focusPoint3D))
                throw new InvalidOperationException($"Input Manager could not create valid focus point on water level (y = {Manager.Use<GameManager>().WaterLevelY}) from cursor. " +
                    $"\nMost likely caused by not supported camera angle.");
            return focusPoint3D;
        }
    }

    public Vector2 FocusPointOnWater2D
        => PlanarProjectionHelper.AsPlanarVector(FocusPointOnWater);

    public event Action OnMouseLeftPressed;
    public event Action OnMouseLeftReleased;

#if UNITY_EDITOR
    [Header("Debugging")]
    [SerializeField] private bool _drawWaterFocusPoint = true;
#endif

    private Camera _inputCameraValue;
    public Camera InputCamera
    {
        get
        {
            if (_inputCameraValue == null)
                _inputCameraValue = Camera.main;
            return _inputCameraValue;
        }
    }


    protected virtual void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
            OnMouseLeftPressed?.Invoke();
        if (Mouse.current.leftButton.wasReleasedThisFrame)
            OnMouseLeftReleased?.Invoke();
    }

#if UNITY_EDITOR
    protected virtual void OnDrawGizmos()
    {
        if (!Application.isPlaying || !_drawWaterFocusPoint)
            return;

        using(new GizmoColorSwitcher(Color.red))
        {
            Gizmos.DrawSphere(FocusPointOnWater, .2f);
        }
    }
#endif
}
