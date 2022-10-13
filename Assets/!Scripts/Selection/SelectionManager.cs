using EmptySkull.Management;
using EmptySkull.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : ManagerModule
{
    public event Action OnSelectionChanged;
    public event Action OnFocusChanged;

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.Title("Debugging")]
    [Sirenix.OdinInspector.ShowInInspector, Sirenix.OdinInspector.ReadOnly]
#endif
    public bool ExecuteSelection { get; private set; } = true;

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.InfoBox("Selected & Focused fields do not update in real time.", Sirenix.OdinInspector.InfoMessageType.Info)]
    [Sirenix.OdinInspector.ShowInInspector, Sirenix.OdinInspector.ReadOnly]
#endif
    public ISelectable Selected { get; private set; }
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ShowInInspector, Sirenix.OdinInspector.ReadOnly]
#endif
    public ISelectable Focused { get; private set; }

    [SerializeField] private bool _invokeCallbackOnChildren = true;
    [SerializeField] private LayerMask _layerMask = ~0;


    protected virtual void Update()
    {
        if(ExecuteSelection)
            ExecuteSelectionUpdate(Manager.Use<InputManager>().MousePosition);
    }

    protected virtual void OnEnable()
    {
        Manager.Use<InputManager>().OnMouseLeftPressed += ExecuteSelectionAction;
    }

    protected virtual void OnDisable()
    {
        Manager.Use<InputManager>().OnMouseLeftPressed -= ExecuteSelectionAction;
    }


    protected virtual void OnDrawGizmos()
    {
        if(Selected != null)
        {
            using(new GizmoColorSwitcher(Color.green))
            {
                Bounds bounds = GetBounds(Selected);
                Gizmos.DrawWireCube(bounds.center, bounds.size);
            }
        }

        if(Focused != null)
        {
            using (new GizmoColorSwitcher(Color.yellow))
            {
                Bounds bounds = GetBounds(Focused);
                Gizmos.DrawWireCube(bounds.center, bounds.size + Vector3.one * .1f);
            }
        }

        Bounds GetBounds(ISelectable selectable)
        {
            GameObject obj = selectable.SelectableObject;
            Bounds bounds = new Bounds(obj.transform.position, Vector3.zero);
            foreach(Renderer r in gameObject.GetComponentsInChildren<Renderer>())
            {
                bounds.Encapsulate(r.bounds);
            }

            if (bounds.size.sqrMagnitude <= Mathf.Epsilon) //Fallback: Default cube if bounds not found
                bounds.Encapsulate(new Bounds(obj.transform.position, Vector3.one));

            return bounds;
        }
    }



    public void StartSelection()
    {
        ExecuteSelection = true;
    }

    public void StopSelection()
    {
        ExecuteSelection = false;
    }


    public void ExecuteSelectionUpdate(Vector2 cursorScreenPosition)
    {
        Ray selectionRay = Camera.main.ScreenPointToRay(cursorScreenPosition);
        if(!Physics.Raycast(selectionRay, out RaycastHit hit, Mathf.Infinity, _layerMask) || !TryGetSelectable(hit.collider.gameObject, out ISelectable newFocus))
        {
            //No valid selectable in focus
            if (Focused != null) //Update: Lose current focus object
            {
                ISelectable oldFocused = Focused;
                Focused = null;

                OnFocusChanged?.Invoke();

                InvokeFocusCallback(oldFocused, false);
            }
            return;
        }
        //Valid selectable in focus: 'newFocus'
        if(Focused != newFocus)
        {
            //Update: Focus changed
            ISelectable oldFocused = Focused;
            Focused = newFocus;

            OnFocusChanged?.Invoke();

            if(oldFocused != null)
                InvokeFocusCallback(oldFocused, false);

            InvokeFocusCallback(Focused, true);
        }

        static bool TryGetSelectable(GameObject obj, out ISelectable selectable)
            => obj.TryGetComponent(out selectable);
    }

    public void ExecuteSelectionAction()
    {
        if (!ExecuteSelection)
            return;

        if(Focused != Selected)
        {
            ISelectable oldSelected = Selected;
            Selected = Focused;

            OnSelectionChanged?.Invoke();

            if (oldSelected != null)
                InvokeSelectionCallback(oldSelected, false);

            if (Selected != null)
                InvokeSelectionCallback(Selected, true);
        }
    }

    private void InvokeFocusCallback(ISelectable selectable, bool newState)
    {
        IEnumerable<ISelectionCallbackReceiver> receiverList = _invokeCallbackOnChildren 
            ? selectable.SelectableObject.GetComponentsInChildren<ISelectionCallbackReceiver>()
            : selectable.SelectableObject.GetComponents<ISelectionCallbackReceiver>();

        foreach (ISelectionCallbackReceiver callbackReceiver in receiverList)
        {
            if (newState)
                callbackReceiver.HandelGainFocus();
            else
                callbackReceiver.HandelLoseFocus();
        }
    }

    private void InvokeSelectionCallback(ISelectable selectable, bool newState)
    {
        IEnumerable<ISelectionCallbackReceiver> receiverList = _invokeCallbackOnChildren
            ? selectable.SelectableObject.GetComponentsInChildren<ISelectionCallbackReceiver>()
            : selectable.SelectableObject.GetComponents<ISelectionCallbackReceiver>();

        foreach (ISelectionCallbackReceiver callbackReceiver in receiverList)
        {
            if (newState)
                callbackReceiver.HandelSelected();
            else
                callbackReceiver.HandelDeselected();
        }
    }
}
