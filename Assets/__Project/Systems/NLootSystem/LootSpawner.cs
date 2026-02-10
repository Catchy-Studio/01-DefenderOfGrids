using __Project.Systems.RunSystem;
using _NueCore.Common.KeyValueDict;
using _NueExtras.StockSystem;
using DG.Tweening;
using UnityEngine;

namespace __Project.Systems.NLootSystem
{
    public class LootSpawner : MonoBehaviour
    {
        [SerializeField] private KeyValueDict<StockTypes,LootData> stockLootDict = new KeyValueDict<StockTypes, LootData>();
        public void Build()
        {
            
        }
        
        public void Spawn(StockTypes stockType,Vector3 position, int count =1)
        {
            if (!stockLootDict.ContainsKey(stockType)) return;
            var info = new LootSpawnInfo
            {
                Center = position,
                Data = stockLootDict[stockType],
                Count = count
            };
            Spawn(info);
        }
        
        public void Spawn(LootSpawnInfo info)
        {
            var data = info.Data;
            var ct = info.Count;
            if (ct > 20)
                ct = 20;
            if (ct <1)
                ct = 1;
            
            for (int i = 0; i < ct; i++)
            {
                Create(info, data);
            }
            
        }

        private static void Create(LootSpawnInfo info, LootData data)
        {
            var loot = Instantiate(data.GetLootPrefab(),info.Center, Quaternion.identity);
            loot.IgnoreCollectExternal = true;
            loot.Build(data);
            var seq = DOTween.Sequence();
            seq.SetLink(loot.gameObject);
            var randomInside = UnityEngine.Random.insideUnitSphere * 1f;
            var tPos = info.Center + randomInside;
            tPos.y = 0;
            seq.Append(loot.transform.DOLocalJump(tPos,1f,1,0.5f).SetEase(Ease.OutFlash)).AppendInterval(0.2f);
            seq.Append(DOVirtual.DelayedCall(info.StayDelay, () =>
            {
                loot.Rb.isKinematic = true;
                loot.transform.SetParent(info.Collector);
                loot.transform.DOLocalJump(Vector3.zero, 1f, 1, 0.25f).OnComplete((() =>
                {
                    loot.IgnoreCollectExternal = false;
                    loot.Collect();

                })).SetLink(loot.gameObject);
            }, false).SetLink(loot.gameObject));
        }
    }
}