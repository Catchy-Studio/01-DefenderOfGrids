using System.Collections.Generic;
using _NueCore.Common.NueLogger;
using _NueCore.Common.ReactiveUtils;
using _NueCore.SaveSystem;
using UnityEngine;
using Color = UnityEngine.Color;

namespace _NueExtras.AchievementSystem
{
    public static class AchStatic
    {
        public static AchievementDataCatalog Catalog { get; private set; }

        public static Dictionary<AchEnum, AchievementData> AchDataDict { get; private set; } =
            new Dictionary<AchEnum, AchievementData>();

        public static void SetCatalog(AchievementDataCatalog catalog)
        {
            Catalog = catalog;
            foreach (var data in Catalog.AchievementDataList)
            {
                if (data == null)
                {
                    "Achievement data is null".NLog(Color.red);
                    continue;
                }
                if (AchDataDict.ContainsKey(data.AchType))
                    continue;
                AchDataDict.Add(data.AchType, data);
            }
        }
        public static void OpenAchievementPopup()
        {
            RBuss.Publish(new AchievementREvents.OpenREvent());
        }
        
        public static void CloseAchievementPopup()
        {
            RBuss.Publish(new AchievementREvents.CloseREvent());
        }

        public static void Achieve(AchEnum achType)
        {
            var data = GetAchievementData(achType);
            if (data == null)
            {
                "Achievement data is null".NLog(Color.red);
                return;
            }
            Achieve(data);
        }

        public static void Achieve(AchievementData data)
        {
            if (data == null)
            {
                "Achievement data is null".NLog(Color.red);
                return;
            }
            RBuss.Publish(new AchievementREvents.PreAchievedREvent(data));

            if (IsAchievementAchieved(data.GetID()))
                return;
            Achieve(data.ID);
            RBuss.Publish(new AchievementREvents.AchievedREvent(data));
        }

        public static void Achieve(string id)
        {
            var achievementSave = NSaver.GetSaveData<AchievementSave>();
            if (!achievementSave.IsAchievementAchieved(id))
            {
                achievementSave.SetAchievementProgress(id, true);
                achievementSave.Save();
                $"Achievement achieved: {id}".NLog(Color.yellow);
            }
        }
        
        public static bool IsAchievementAchieved(string id)
        {
            var achievementSave = NSaver.GetSaveData<AchievementSave>();
            return achievementSave.IsAchievementAchieved(id);
        }
        
        public static bool IsAchievementAchieved(AchEnum achEnum)
        {
            var data = GetAchievementData(achEnum);
            if (data == null)
            {
                "Achievement data is null".NLog(Color.red);
                return false;
            }
            return IsAchievementAchieved(data.GetID());
        }
        
        public static AchievementData GetAchievementData(AchEnum achEnum)
        {
            if (Catalog == null)
            {
                Debug.LogError("AchievementDataCatalog is not assigned.");
                return null;
            }
            if (AchDataDict.TryGetValue(achEnum,out var value))
            {
                return value;
            }

            return null;
        }

        public static void ClearAllAch()
        {
            var achievementSave = NSaver.GetSaveData<AchievementSave>();
            if (achievementSave == null)
            {
                "Achievement save data is null".NLog(Color.red);
                return;
            }
            foreach (var ach in AchDataDict)
            {
                achievementSave.SetAchievementProgress(ach.Value.GetID(), false);
            }
            achievementSave.Save();
            "All achievements cleared".NLog(Color.yellow);
            RBuss.Publish(new AchievementREvents.AllAchievementsClearedREvent());
        }
    }
}