using JSONData;
using UnityEngine;

/// <summary>
/// Base class for any game-entity to be managed
/// </summary>
public abstract class Entity : MonoBehaviour
{
    [SerializeField] protected string _statPath = "standard";
    [SerializeField] protected Stats _stats;

    protected virtual void Start()
    {
        _stats = Access.GetStats(_statPath);
    }
}
