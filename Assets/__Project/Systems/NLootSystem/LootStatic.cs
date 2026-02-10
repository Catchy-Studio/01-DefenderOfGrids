using _NueExtras.StockSystem;
using UnityEngine;

namespace __Project.Systems.NLootSystem
{
    public static class LootStatic
    {
        public static void SpawnLoot(StockTypes stock,Vector3 center, int count =1)
        {
            LootManager.Instance.Spawner.Spawn(stock, center, count);
        }

        public static void SpawnLoot(LootSpawnInfo info)
        {
            LootManager.Instance.Spawner.Spawn(info);
        }
        
        
    }
}