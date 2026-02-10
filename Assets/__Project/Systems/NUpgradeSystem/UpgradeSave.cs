using System.Collections.Generic;
using __Project.Systems.NUpgradeSystem._SkillTree;
using _NueCore.NStatSystem;
using _NueCore.SaveSystem;

namespace __Project.Systems.NUpgradeSystem
{
    public class UpgradeSave : NBaseSave
    {
        #region Setup
        private const string SavePath = "UpgradeSave";
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
            NSaver.SaveData<UpgradeSave>();
        }

        public override void Load()
        {
            NSaver.GetSaveData<UpgradeSave>();
        }

        public override void ResetSave()
        {
            NSaver.ResetSave<UpgradeSave>();
        }

        public override SaveTypes GetSaveType()
        {
            return SaveTypes.InGame;
        }
        #endregion


        public int TotalPurchasedSkillPoints;
        #region Node
        public List<SkillSave> nodeSaveList = new List<SkillSave>();
        public SkillSave GetNodeSave(string id)
        {
            var t = nodeSaveList.Find(s => s.id == id);
            if (t == null)
            {
                t = AddNodeSave(id);
            }
            return t;
        }
        public SkillSave AddNodeSave(string id)
        {
            var newSkillSave = new SkillSave
            {
                id = id
            };
            nodeSaveList.Add(newSkillSave);
            return newSkillSave;
        }
        #endregion

        #region Permanent Tree Stats
        
        public List<NStatSave> PermanentTreeStatSaveList = new List<NStatSave>();
        public NStatSave GetPermanentTreeStatSave(string id)
        {
            var t = PermanentTreeStatSaveList.Find(s => s.key == id) ?? AddPermanentTreeStatSave(id);
            return t;
        }
        
        public NStatSave AddPermanentTreeStatSave(string id)
        {
            var newStatSave = new NStatSave
            {
                key = id
            };
            PermanentTreeStatSaveList.Add(newStatSave);
            return newStatSave;
        }
        
        

        #endregion
        
        #region Skill Tree Stats
        
        public List<NStatSave> SkillTreeStatSaveList = new List<NStatSave>();

        public void ResetSkillTreeStats()
        {
            SkillTreeStatSaveList.Clear();
        }
        public NStatSave GetSkillTreeStatSave(NStatEnum statEnum)
        {
            return GetSkillTreeStatSave(statEnum.GetStatKey());
        }
        public NStatSave GetSkillTreeStatSave(string id)
        {
            var t = SkillTreeStatSaveList.Find(s => s.key == id) ?? AddSkillTreeStatSave(id);
            return t;
        }
        
        public NStatSave AddSkillTreeStatSave(string key)
        {
            var newStatSave = new NStatSave
            {
                key = key
            };
            SkillTreeStatSaveList.Add(newStatSave);
            return newStatSave;
        }
        public bool IsSkillUnlocked(string id)
        {
            var t = nodeSaveList.Find(s => s.id == id);
            if (t == null)
            {
                return false;
            }
            return t is { isUnlocked: true };
        }

        public bool IsSkillUnlocked(SkillData data)
        {
            return IsSkillUnlocked(data.GetID());
        }
        #endregion
    }
}