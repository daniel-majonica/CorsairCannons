using UnityEngine;

public class SelectableCubeEntity : Entity, ISelectable, ISelectionCallbackReceiver
{
    private const string MainColorUrpShaderKey = "_BaseColor";

    public GameObject SelectableObject => gameObject;

    [SerializeField] private Renderer[] _renderers;
    [SerializeField] private Color _focusColor = Color.yellow;
    [SerializeField] private Color _selectionColor = Color.green;

    private Color[] _nativeColors;
    private bool _isSelected;

    protected virtual void Awake()
    {
        if(_renderers == null)
            _renderers = GetComponentsInChildren<Renderer>();

        if (_renderers == null)
            Debug.LogWarning($"[{nameof(SelectableCubeEntity)} on: {gameObject.name}] Could not find valid renderer.", this);
        else
        {
            _nativeColors = new Color[_renderers.Length];
            for (int i = 0; i < _renderers.Length; i++)
            {
                _nativeColors[i] = _renderers[i].material.GetColor(MainColorUrpShaderKey);
            }
        }
    }


    public void HandelDeselected()
    {
        SwitchToColor(_nativeColors);
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
            SwitchToColor(_nativeColors);
    }

    public void HandelSelected()
    {
        SwitchToColor(_selectionColor);
        _isSelected = true;
    }

    private void SwitchToColor(Color[] colors)
    {
        for (int i = 0; i < _renderers.Length; i++)
        {
            _renderers[i].material.SetColor(MainColorUrpShaderKey, colors[i]);
        }
    }

    private void SwitchToColor(Color color)
    {
        for (int i = 0; i < _renderers.Length; i++)
        {
            _renderers[i].material.SetColor(MainColorUrpShaderKey, color);
        }
    }
}
