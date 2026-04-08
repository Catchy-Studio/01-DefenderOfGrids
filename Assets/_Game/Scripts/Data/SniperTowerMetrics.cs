using UnityEngine;

[CreateAssetMenu(fileName = "SniperTowerMetrics", menuName = "TowerDefense/Towers/Sniper Metrics")]
public class SniperTowerMetrics : ScriptableObject
{
    [Header("Attack")]
    [Min(0.1f)] public float range = 8f;
    [Min(0.05f)] public float fireRate = 0.5f;
    [Min(0f)] public float damage = 10f;

    [Header("Line Shot")]
    [Tooltip("Radius used for CircleCast (0 = Raycast).")]
    [Min(0f)] public float lineThickness = 0.05f;
    [Min(1)] public int pierceCount = 1;

    [Header("Targeting")]
    [Tooltip("How often we refresh the aim/target in seconds.")]
    [Min(0.05f)] public float retargetInterval = 0.2f;
}

