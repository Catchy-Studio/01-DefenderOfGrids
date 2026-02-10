using UnityEngine;

namespace __Project.Systems.NLootSystem
{
    public struct LootSpawnInfo
    {
        public LootData Data;
        public Vector3 Center;
        public int Count;
        public Transform Collector;
        public float StayDelay;

        public LootSpawnInfo(LootData data, Vector3 center)
        {
            Data = data;
            Center = center;
            Count = 1;
            Collector = null;
            StayDelay = 0;
        }
    }
}