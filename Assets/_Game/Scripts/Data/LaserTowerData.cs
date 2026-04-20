using UnityEngine;

[CreateAssetMenu(fileName = "New Laser Tower", menuName = "Tower Defense/Laser Tower Data")]
public class LaserTowerData : TowerData
{
    [Header("Targeting")]
    public float range = 5f;
    
    [Header("Support Stats")]
    [Tooltip("Example: 1.5 means the enemy takes 50% more damage from all sources.")]
    public float damageMultiplier = 1.5f; 
}