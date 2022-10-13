using JSONData;
using UnityEngine;

/// <summary>
/// Base class for any game-entity to be managed
/// </summary>
public abstract class Entity : MonoBehaviour
{
    [SerializeField] private string _statPath = "standard";
    [SerializeField] private Stats _stats;

    protected virtual void Start()
    {
        _stats = Access.GetStats(_statPath);
    }
}
