using System.Collections.Generic;
using UnityEngine;

public class SpeedBufferTowerController : MonoBehaviour
{
    [SerializeField] private SpeedBufferTowerData towerData;
    
    // Assuming you have a way to get the tower's grid position from your LevelGridManager
    private Vector2Int currentGridPosition; 
    
    // Keep track of who we buffed, so we can un-buff them if this tower is sold/destroyed
    private List<IBuffable> currentlyBuffedTowers = new List<IBuffable>();

    public void Initialize(Vector2Int gridPos)
    {
        currentGridPosition = gridPos;
        ApplyBuffsToSurroundings();
    }

    private void ApplyBuffsToSurroundings()
    {
        // 1. Clear old buffs if this is called during an upgrade
        RemoveAllBuffs();

        // 2. Get the valid relative tile offsets based on our upgrade tier
        List<Vector2Int> targetOffsets = GetOffsetsForCurrentTier();

        // 3. Scan the grid
        foreach (Vector2Int offset in targetOffsets)
        {
            Vector2Int targetCell = currentGridPosition + offset;
            
            // Pseudo-code: Call your LevelGridManager to see if a tower exists at 'targetCell'
            // GameObject towerObj = LevelGridManager.Instance.GetTowerAt(targetCell);
            GameObject towerObj = null; // REPLACE WITH YOUR ACTUAL GRID CHECK

            if (towerObj != null)
            {
                IBuffable buffableTower = towerObj.GetComponent<IBuffable>();
                if (buffableTower != null)
                {
                    buffableTower.ApplySpeedBuff(towerData.baseCooldownReduction);
                    currentlyBuffedTowers.Add(buffableTower);
                }
            }
        }
    }

    private List<Vector2Int> GetOffsetsForCurrentTier()
    {
        List<Vector2Int> offsets = new List<Vector2Int>();

        // Always include Left and Right
        offsets.Add(Vector2Int.left);
        offsets.Add(Vector2Int.right);

        if (towerData.currentRangeTier == BuffRangeTier.Cross || towerData.currentRangeTier == BuffRangeTier.Surrounding)
        {
            // Add Up and Down for Cross and Surrounding
            offsets.Add(Vector2Int.up);
            offsets.Add(Vector2Int.down);
        }

        if (towerData.currentRangeTier == BuffRangeTier.Surrounding)
        {
            // Add Diagonals for max upgrade
            offsets.Add(new Vector2Int(-1, 1)); // Top-Left
            offsets.Add(new Vector2Int(1, 1));  // Top-Right
            offsets.Add(new Vector2Int(-1, -1));// Bottom-Left
            offsets.Add(new Vector2Int(1, -1)); // Bottom-Right
        }

        return offsets;
    }

    private void RemoveAllBuffs()
    {
        foreach (var tower in currentlyBuffedTowers)
        {
            tower.RemoveSpeedBuff(towerData.baseCooldownReduction);
        }
        currentlyBuffedTowers.Clear();
    }

    // Call this via your UI Upgrade event
    public void OnTowerUpgraded()
    {
        // Re-calculate buffs in case the range tier or potency changed
        ApplyBuffsToSurroundings();
    }

    private void OnDestroy() // Or OnSell() depending on your game loop
    {
        RemoveAllBuffs();
    }
}