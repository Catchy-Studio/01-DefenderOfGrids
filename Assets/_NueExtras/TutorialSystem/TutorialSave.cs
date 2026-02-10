using System;
using _NueCore.Common.KeyValueDict;
using _NueCore.SaveSystem;

namespace _NueExtras.TutorialSystem
{
    public class TutorialSave : NBaseSave
    {
        #region Setup
        private const string SavePath = "TutorialSave";
        protected override string GetSavePath()
        {
            return SavePath;
        }
        public override void Save()
        {
            NSaver.SaveData<TutorialSave>();
        }

        public override void Load()
        {
            NSaver.GetSaveData<TutorialSave>();
        }

        public override void ResetSave()
        {
            NSaver.ResetSave<TutorialSave>();
        }

        public override SaveTypes GetSaveType()
        {
            return SaveTypes.Global;
        }
        #endregion
        public KeyValueDict<string, TutorialSaveInfo> TutorialSaveInfoDict = new KeyValueDict<string, TutorialSaveInfo>();
        public KeyValueDict<string, TutorialSaveInfo> QuestSaveInfoDict = new KeyValueDict<string, TutorialSaveInfo>();
        
        [Serializable]
        public class TutorialSaveInfo
        {
            public string Guid;
            public int TaskStep;
            public bool IsOrdered;
            public bool IsShown;
            public bool IsCompleted;
        
            public TutorialSaveInfo(string guid, int taskStep, bool isOrdered, bool isShown, bool isCompleted)
            {
                Guid = guid;
                TaskStep = taskStep;
                IsOrdered = isOrdered;
                IsShown = isShown;
                IsCompleted = isCompleted;
            }
        }
    }
}