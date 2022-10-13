using JSONData;
using UnityEngine;

/// <summary>
/// Base class for any game-entity to be managed
/// </summary>
public abstract class Entity : MonoBehaviour
{
    [SerializeField] private string StatPath = "standard";
    [SerializeField] private Stats Stats;

    protected virtual void Start()
    {
        Stats = Access.GetStats(StatPath);
    }
}
