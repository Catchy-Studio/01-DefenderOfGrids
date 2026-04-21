using UnityEngine;

public enum BuffRangeTier
{
    HorizontalOnly, // Left/Right
    Cross,          // Up/Down/Left/Right
    Surrounding     // All 8 tiles
}

[CreateAssetMenu(fileName = "SpeedBufferTowerData", menuName = "DefenderOfGrids/TowerData/Speed Buffer")]
public class SpeedBufferTowerData : TowerData // Assuming TowerData is your base SO
{
    [Header("Buff Settings")]
    [Tooltip("The percentage to reduce standard tower cooldowns (e.g., 0.15 = 15%)")]
    public float baseCooldownReduction = 0.15f;
    
    [Tooltip("The current layout of the buff range")]
    public BuffRangeTier currentRangeTier = BuffRangeTier.HorizontalOnly;
    
    // You can tie your __Project.Systems.NUpgradeSystem into these variables later
    // to dynamically increase baseCooldownReduction or shift the currentRangeTier.
}