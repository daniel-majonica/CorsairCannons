using System;
using UnityEngine;

[Serializable]
public struct ObservableFloat
{
    [SerializeField, HideInInspector]
    private float _value;
    public float Value
    {
        get => _value;
        set
        {
            if (value == _value)
                return;

            OnBeforeValueChanges?.Invoke();
            _value = value;
            OnValueChanged?.Invoke();
        }
    }

    public ObservableFloat(float value)
    {
        _value = value;

        OnValueChanged = default;
        OnBeforeValueChanges = default;
    }

    public event Action OnValueChanged;
    public event Action OnBeforeValueChanges;

    public static implicit operator float(ObservableFloat f) => f.Value;
    public static implicit operator ObservableFloat(float f) => new ObservableFloat(f);
}
