using System;
using UnityEngine;

/// <summary>
/// Lightweight static event broker for tower lifecycle events.
/// Any system (LevelGridManager, towers themselves, etc.) can fire these events;
/// any listener (SpeedBufferTower, UI, analytics) can subscribe.
/// </summary>
public static class TowerEvents
{
    /// <summary>
    /// Fired after a new tower has been fully initialized on the grid.
    /// Carries the placed tower's GameObject so listeners can inspect/query it.
    /// </summary>
    public static event Action<GameObject> OnTowerBuilt;

    /// <summary>
    /// Call this whenever a tower is successfully placed and ready to operate.
    /// </summary>
    public static void NotifyTowerBuilt(GameObject tower)
    {
        OnTowerBuilt?.Invoke(tower);
    }
}
