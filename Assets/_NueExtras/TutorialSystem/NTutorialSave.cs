using System;
using _NueCore.Common.KeyValueDict;
using _NueCore.SaveSystem;

namespace _NueExtras.TutorialSystem
{
    public class NTutorialSave : NBaseSave
    {
        #region Setup
        private const string SavePath = "TutorialSave";
        protected override string GetSavePath()
        {
            var path = SavePath;
#if UNITY_EDITOR
            path += "_Editor";
#endif
            return path;
        }
        public override void Save()
        {
            NSaver.SaveData<NTutorialSave>();
        }

        public override void Load()
        {
            NSaver.GetSaveData<NTutorialSave>();
        }

        public override void ResetSave()
        {
            NSaver.ResetSave<NTutorialSave>();
        }

        public override SaveTypes GetSaveType()
        {
            return SaveTypes.Global;
        }
        #endregion
        public KeyValueDict<string, NTutorialSaveInfo> TutorialSaveInfoDict = new KeyValueDict<string, NTutorialSaveInfo>();

        public bool TutorialFinished;

        public void CompleteTask(string id)
        {
            if (TutorialSaveInfoDict.ContainsKey(id))
            {
                TutorialSaveInfoDict[id].IsCompleted = true;
            }
            else
            {
                TutorialSaveInfoDict.Add(id, new NTutorialSaveInfo(id, true));
            }
        }
        
        [Serializable]
        public class NTutorialSaveInfo
        {
            public string ID;
            public bool IsCompleted;
        
            public NTutorialSaveInfo(string id,bool isCompleted)
            {
                ID = id;
                IsCompleted = isCompleted;
            }
        }
    }
}