using EmptySkull.Management;
using System;
using UnityEngine;

public class SelectionEventLogger : MonoBehaviour
{
    private Action _focusedChangedAction;
    private Action _selectionChangedAction;

    protected virtual void OnEnable()
    {
        _focusedChangedAction = () => Debug.Log($"[{nameof(SelectionEventLogger)} on: {gameObject.name}] Focus changed. New focus object: '{GetLabel(Manager.Use<SelectionManager>().Focused)}'.");
        _selectionChangedAction = () => Debug.Log($"[{nameof(SelectionEventLogger)} on: {gameObject.name}] Selection changed. New selected object: '{GetLabel(Manager.Use<SelectionManager>().Selected)}'.");

        Manager.Use<SelectionManager>().OnFocusChanged += _focusedChangedAction;
        Manager.Use<SelectionManager>().OnSelectionChanged += _selectionChangedAction;

        string GetLabel(ISelectable selectable)
        {
            if (selectable == null)
                return "<None>";
            return selectable.SelectableObject.name;
        }
    }

    protected virtual void OnDisable()
    {
        Manager.Use<SelectionManager>().OnFocusChanged -= _focusedChangedAction;
        Manager.Use<SelectionManager>().OnSelectionChanged -= _selectionChangedAction;
    }
}
