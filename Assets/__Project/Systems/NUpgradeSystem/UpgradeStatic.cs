using System.Collections.Generic;
using _NueCore.NStatSystem;
using _NueCore.SaveSystem;
using UnityEngine;

namespace __Project.Systems.NUpgradeSystem
{
    public static class UpgradeStatic
    {
       
        public static float GetTotalStat(NStatEnum statEnum)
        {
            return GetTotalStat(statEnum.GetStatKey());
        }
        
        public static float GetTotalStat(string customKey)
        {
            var save = NSaver.GetSaveData<UpgradeSave>();
            var permanentStat = save.GetPermanentTreeStatSave(customKey);
            var skillTreeStat = save.GetSkillTreeStatSave(customKey);
            var totalFlat = permanentStat.flatValue + skillTreeStat.flatValue;
            var percentTotal = permanentStat.percentValue + skillTreeStat.percentValue;
            return totalFlat + (totalFlat * percentTotal / 100f);

        }

        public static int GetTotalStatRounded(NStatEnum statEnum)
        {
            return Mathf.RoundToInt(GetTotalStat(statEnum));
        }
        
        public static bool TryGetTotalStat(NStatEnum statEnum, out float value)
        {
            value =GetTotalStat(statEnum);
            if (value == 0)
                return false;
            return true;
        }
        
        public static bool TryGetTotalStat(string customKey, out float value)
        {
            value =GetTotalStat(customKey);
            if (value == 0)
                return false;
            return true;
        }
        
        public static bool TryGetTotalRoundedStat(NStatEnum statEnum, out int value)
        {
            value =GetTotalStatRounded(statEnum);
            if (value == 0)
                return false;
            return true;
        }

        public static bool HasStat(NStatEnum statEnum)
        {
            if (TryGetTotalStat(statEnum, out var value))
            {
                if (value != 0)
                    return true;
            }
            return false;
        }
        
        public static bool HasStat(string customKey)
        {
            if (TryGetTotalStat(customKey, out var value))
            {
                if (value != 0)
                    return true;
            }
            return false;
        }
        
        public static void IncreasePermStat(NStatEnum statEnum, float amount, NStatCategory category)
        {
            var save = NSaver.GetSaveData<UpgradeSave>();
            var permanentStat = save.GetPermanentTreeStatSave(statEnum.GetStatKey());
            permanentStat.AddNStat(amount,category);
            save.Save();
        }
        
        public static void IncreasePermStat(NStatEnum statEnum, int amount, NStatCategory category)
        {
            var save = NSaver.GetSaveData<UpgradeSave>();
            var permanentStat = save.GetPermanentTreeStatSave(statEnum.GetStatKey());
            permanentStat.AddNStat(amount,category);
            save.Save();
        }
        
    }
}