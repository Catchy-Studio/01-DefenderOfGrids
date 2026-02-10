using _NueCore.Common.KeyValueDict;
using _NueCore.Common.Sandbox;
using _NueExtras.StockSystem._StockSpawnerSubSystem;
using UnityEngine;

namespace _NueExtras.StockSystem._StockUISubSystem
{
    public class StockCounterLayout : MonoBehaviour
    {
        [SerializeField] private Transform root;
        [SerializeField] private KeyValueDict<StockTypes,StockCounterCard> stockCounterCardDictMenu;

        public KeyValueDict<StockTypes, StockCounterCard> StockCounterCardDictMenu => stockCounterCardDictMenu;

        public void Show()
        {
            root.gameObject.SetActive(true);
            foreach (var kvp in stockCounterCardDictMenu)
            {
                StockSpawnerStatic.SetStockMoveRoot(kvp.Key,kvp.Value.transform);
                if (kvp.Key != StockTypes.Coin)
                {
                    var currentStock = StockStatic.GetStock(kvp.Key);
                    if (currentStock>0)
                    {
                        kvp.Value.Root.gameObject.SetActive(true);
                    }
                    else
                    {
                        kvp.Value.Root.gameObject.SetActive(false);
                    }
                }
               
            }
        }

        public void Hide()
        {
            root.gameObject.SetActive(false);
        }
        
    }
}