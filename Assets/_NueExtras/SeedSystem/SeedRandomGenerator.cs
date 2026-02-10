using System;
using System.Collections.Generic;
using _NueCore.SaveSystem;
using Random = UnityEngine.Random;

namespace _NueExtras.SeedSystem
{
    public static class SeedRandomGenerator
    {
        public static List<float> GenerateRandomList(int sampleCount = 100)
        {
            var list = new List<float>();
            for (int i = 0; i < sampleCount; i++)
            {
                var random = Random.value;
                list.Add(random);
            }
            return list;
        }
        
        public static SeedSave.RandomInfo GetRandomInfo(string seedID)
        {
            var seedSave = NSaver.GetSaveData<SeedSave>();
            return seedSave.GetRandomInfo(seedID);
        }
    }
}