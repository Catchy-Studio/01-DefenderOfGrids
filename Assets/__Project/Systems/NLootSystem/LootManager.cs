using _NueCore.ManagerSystem.Core;
using UnityEngine;

namespace __Project.Systems.NLootSystem
{
    public class LootManager : NManagerBase
    {
        [SerializeField] private LootSpawner spawner;

        public static LootManager Instance { get; private set; }

        public LootSpawner Spawner => spawner;

        public override void NAwake()
        {
            Instance = InitSingleton<LootManager>();
            base.NAwake();
        }

        public override void NStart()
        {
            base.NStart();
            Spawner.Build();
        }
    }
}