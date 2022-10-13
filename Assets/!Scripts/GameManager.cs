using EmptySkull.Management;
using UnityEngine;

public class GameManager : ManagerModule
{
    [Header("Settings")]
    [SerializeField] private float _waterLevelY = 0; //Default water level in Y Axis
    public float WaterLevelY => _waterLevelY;
}
