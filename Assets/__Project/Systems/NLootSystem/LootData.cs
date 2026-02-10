using _NueCore.NStatSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace __Project.Systems.NLootSystem
{
    [CreateAssetMenu(fileName = "LootData", menuName = "N/LootSystem/LootData", order = 0)]
    public class LootData : ScriptableObject
    {
        [SerializeField,ReadOnly] private string id;
        [SerializeField] private NStatList statList;
        [SerializeField] private LootBase lootPrefab;

        public LootBase GetLootPrefab() => lootPrefab;
        public NStatList GetStatList() => statList;


        public string GetID()
        {
            return id;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                return;
            }

            id = name;
        }
#endif
        public GameObject GetSpawnPrefab()
        {
            return lootPrefab.gameObject;
        }
    }
}