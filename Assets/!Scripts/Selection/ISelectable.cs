using UnityEngine;
/// <summary>
/// Used to identify if an scene-object is selectable. Must be placed on the gameObject, that is holding the collider to be detected for selection.
/// </summary>
public interface ISelectable 
{
    GameObject SelectableObject { get; }
}
