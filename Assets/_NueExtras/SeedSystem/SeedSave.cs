using System;
using System.Collections.Generic;
using System.Text;
using _NueCore.SaveSystem;

namespace _NueExtras.SeedSystem
{
    public class SeedSave : NBaseSave
    {
        #region Setup
        private static readonly StringBuilder Str = new StringBuilder();
        protected override string GetSavePath()
        {
            Str.Clear();
            Str.Append("Seed").Append("_").Append(SaveStatic.ActiveSaveIndex);;
            return Str.ToString();
        }
        public override void Save()
        {
            NSaver.SaveData<SeedSave>();
        }
        public override void Load()
        {
            NSaver.GetSaveData<SeedSave>();
        }
        public override void ResetSave()
        {
            NSaver.ResetSave<SeedSave>();
        }
        public override SaveTypes GetSaveType()
        {
            return SaveTypes.InGame;
        }
        #endregion
        
        [Serializable]
        public class RandomInfo
        {
            public string id;
            public List<float> randomList = new List<float>();
        }
        
        public List<RandomInfo> RandomSeedList = new List<RandomInfo>();
        
        public RandomInfo GetRandomInfo(string seedID)
        {
            foreach (var seed in RandomSeedList)
            {
                if (seed.id == seedID)
                {
                    return seed;
                }
            }
            var newInfo = new RandomInfo
            {
                id = seedID,
                randomList = SeedRandomGenerator.GenerateRandomList()
            };
            return newInfo;
        }
    }
}