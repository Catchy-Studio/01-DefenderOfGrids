using System;
using System.Collections.Generic;

namespace _NueCore.Common.Utility
{
    public static class NRandom
    {
        public static int GetRandom(int min, int max)
        {
            return UnityEngine.Random.Range(min, max);
        }
        
        public static bool Roll(int chance,int max = 100)
        {
            return UnityEngine.Random.Range(0, max) < chance;
        }
        
        public struct ProbabilityStruct<T>
        {
            public ProbabilityStruct(T item, int probability)
            {
                Item = item;
                Probability = probability;
            }
            public T Item;
            public int Probability;
        }
        public static T GetWeightedRandomItem<T>(List<T> items, Func<T, int> probabilitySelector)
        {
            if (items == null || items.Count == 0)
                return default(T);
        
            int totalWeight = 0;
            foreach (var item in items)
            {
                totalWeight += probabilitySelector(item);
            }
        
            int randomValue = UnityEngine.Random.Range(0, totalWeight);
            int cumulativeWeight = 0;
        
            foreach (var item in items)
            {
                cumulativeWeight += probabilitySelector(item);
                if (randomValue < cumulativeWeight)
                {
                    return item;
                }
            }
        
            return items[^1];
        }
    }
}