using UnityEngine;

public class SelectableCubeEntity : Entity, ISelectable, ISelectionCallbackReceiver
{
    private const string MainColorUrpShaderKey = "_BaseColor";

    public GameObject SelectableObject => gameObject;

    [SerializeField] private Renderer _renderer;
    [SerializeField] private Color _focusColor = Color.yellow;
    [SerializeField] private Color _selectionColor = Color.green;

    private Color _nativeColor;
    private bool _isSelected;

    protected virtual void Awake()
    {
        if(_renderer != null)
            _renderer = GetComponentInChildren<Renderer>();

        if (_renderer == null)
            Debug.LogWarning($"[{nameof(SelectableCubeEntity)} on: {gameObject.name}] Could not find valid renderer.", this);
        else
            _nativeColor = _renderer.material.GetColor(MainColorUrpShaderKey);
    }


    public void HandelDeselected()
    {
        SwitchToColor(_nativeColor);
        _isSelected = false;
    }

    public void HandelGainFocus()
    {
        if(!_isSelected)
            SwitchToColor(_focusColor);
    }

    public void HandelLoseFocus()
    {
        if(!_isSelected)
            SwitchToColor(_nativeColor);
    }

    public void HandelSelected()
    {
        SwitchToColor(_selectionColor);
        _isSelected = true;
    }

    private void SwitchToColor(Color c)
    {
        _renderer.material.SetColor(MainColorUrpShaderKey, c);
    }
}
