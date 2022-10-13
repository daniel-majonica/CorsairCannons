using EmptySkull.Management;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : ManagerModule //TODO Rework to more event-based approach
{
    public Vector2 MousePosition => Mouse.current.position.ReadValue();
    public Ray MouseFocusRay => InputCamera.ScreenPointToRay(MousePosition);

    public event Action OnMouseLeftPressed;


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
    }
}
