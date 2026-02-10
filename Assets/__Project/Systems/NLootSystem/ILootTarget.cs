using UnityEngine;

namespace __Project.Systems.NLootSystem
{
    public interface ILootTarget
    {
        bool CanCollectLoot();
        void CollectLoot(Transform collector);
    }
}