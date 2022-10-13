using EmptySkull.Management;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : ManagerModule //TODO Rework to more event-based approach
{
    public Vector2 MousePosition => Mouse.current.position.ReadValue();

    public event Action OnMouseLeftPressed;

    protected virtual void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
            OnMouseLeftPressed?.Invoke();
    }
}
