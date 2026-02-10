using System;
using System.Collections.Generic;
using System.Text;
using _NueCore.SaveSystem;

namespace _NueExtras.AchievementSystem
{
    public class AchievementSave : NBaseSave
    {
        #region Setup
        private static readonly StringBuilder Str = new StringBuilder();
        protected override string GetSavePath()
        {
            Str.Clear();
            Str.Append("Achievement_Global");
            return Str.ToString();
        }
        public override void Save()
        {
            NSaver.SaveData<AchievementSave>();
        }
        public override void Load()
        {
            NSaver.GetSaveData<AchievementSave>();
        }
        public override void ResetSave()
        {
            NSaver.ResetSave<AchievementSave>();
        }

        public override SaveTypes GetSaveType()
        {
            return SaveTypes.Global;
        }

        #endregion
        
        public List<AchievementProgress> AchievementProgressList = new List<AchievementProgress>();
        
        
        public void SetAchievementProgress(string achievementID, bool isAchieved)
        {
            var progress = AchievementProgressList.Find(x => x.achievementID == achievementID);
            if (progress == null)
            {
                progress = new AchievementProgress
                {
                    achievementID = achievementID,
                    isAchieved = isAchieved
                };
                AchievementProgressList.Add(progress);
            }
            else
            {
                progress.isAchieved = isAchieved;
            }
        }
        
        public bool GetAchievementProgress(string achievementID)
        {
            var progress = AchievementProgressList.Find(x => x.achievementID == achievementID);
            if (progress == null)
                return false;
            return progress.isAchieved;
        }
        
        public bool IsAchievementAchieved(string achievementID)
        {
            var progress = AchievementProgressList.Find(x => x.achievementID == achievementID);
            if (progress == null)
                return false;
            return progress.isAchieved;
        }
        
        [Serializable]
        public class AchievementProgress
        {
            public string achievementID;
            public bool isAchieved;
        }
    }
}