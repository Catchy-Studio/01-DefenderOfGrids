using UnityEngine;

[CreateAssetMenu(fileName = "New Bowling Tower", menuName = "Tower Defense/Bowling Tower Data")]
public class BowlingTowerData : TowerData
{
    [Header("Firing Stats")]
    public float fireCooldown = 3f;
    public float ballSpeed = 5f;
    
    [Header("Impact Stats")]
    public float damage = 5f;
    public float stunDuration = 1.5f;
}
