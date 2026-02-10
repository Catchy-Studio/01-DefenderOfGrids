using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _NueCore.NStatSystem
{
    [Serializable]
    public class NStatList
    {
        [SerializeField, BoxGroup("STATS"), TableList(AlwaysExpanded = true,
             NumberOfItemsPerPage = 30,
             ShowPaging = true,
             ShowIndexLabels = true), OnValueChanged(nameof(Validate))]
        private List<NStatField> statList = new List<NStatField>();

        public List<NStatField> GetStatList()
        {
            return statList;
        }

        private void Validate()
        {
            statList.RemoveAll(s => s == null || string.IsNullOrEmpty(s.Key));
            var seenKeys = new HashSet<string>();
            statList.RemoveAll(s => !seenKeys.Add(s.Key+"_"+s.GetStatCategory().ToString()));
        }

        #region Getters
        public NStatField GetStat(string key,NStatCategory statCategory = NStatCategory.Flat)
        {
            return statList.Find(s => s.Key == key && s.GetStatCategory() == statCategory);
        }
        public bool TryGetStat(string key, out NStatField stat,NStatCategory statCategory = NStatCategory.Flat)
        {
            stat = GetStat(key,statCategory);
            return stat != null;
        }
        public bool TryGetStat(NStatEnum statEnum, out NStatField stat,NStatCategory statCategory = NStatCategory.Flat)
        {
            stat = TryGetStat(statEnum.GetStatKey(), out stat,statCategory) ? stat : null;
            return stat != null;
        }
        #endregion

        #region Add Remove Replace
        
        [Button(ButtonSizes.Large), TabGroup("ADD", TextColor = "green"), GUIColor(0.4f, 1f, 0.4f)]
        public void AddStat(NStatEnum statEnum, float value,NStatCategory statCategory = NStatCategory.Flat)
        {
            if (statEnum == NStatEnum.None)
            {
                return;
            }

            if (TryGetStat(statEnum,out var tStat,statCategory))
            {
                tStat.IncreaseValue(value);
                return;
            }
            var stat = new NStatField();
            stat.Initialize(statEnum, "", null, value);
            stat.SetCategory(statCategory);
            statList.Add(stat);
        }

        public void AddStat(string key, float value,NStatCategory category = NStatCategory.Flat)
        {
            if (TryGetStat(key,out var tStat,category))
            {
                tStat.IncreaseValue(value);
                return;
            }
            var stat = new NStatField();
            stat.Initialize(NStatEnum.None,key, null, value);
            stat.SetCategory(category);
            statList.Add(stat);
        }
        
        [Button(ButtonSizes.Large), TabGroup("ADD"), GUIColor(0.4f, 1f, 0.4f)]
        public void AddStatCustom(NStatField stat)
        {
            if (TryGetStat(stat.Key,out var tStat,stat.GetStatCategory()))
            {
                tStat.IncreaseValue(stat.GetValue());
                return;
            }
            statList.Add(stat);
        }
        
        [Button(ButtonSizes.Large), TabGroup("ADD"), GUIColor(0.4f, 1f, 0.4f)]
        public void AddStatCustom(string key, float value, NStatCategory category = NStatCategory.Flat)
        {
            if (TryGetStat(key,out var tStat,category))
            {
                tStat.IncreaseValue(value);
                return;
            }
            var stat = new NStatField();
            stat.Initialize(NStatEnum.None,key, null, value);
            stat.SetCategory(category);
            statList.Add(stat);
        }
        
        [Button(ButtonSizes.Large), TabGroup("REMOVE", TextColor = "red"), GUIColor(1f, 0.18f, 0.54f)]
        public void RemoveStat(NStatEnum statEnum, NStatCategory statCategory = NStatCategory.Flat)
        {
            statList.RemoveAll(s => s.StatEnum == statEnum && s.GetStatCategory() == statCategory);
        }

        [Button(ButtonSizes.Large), TabGroup("REMOVE"), GUIColor(1f, 0.18f, 0.54f)]
        public void RemoveStatCustom(NStatField stat)
        {
            statList.RemoveAll(s => s.Key == stat.Key && s.GetStatCategory() == stat.GetStatCategory());
        }
        
        [Button(ButtonSizes.Large), TabGroup("REPLACE",TextColor = "blue"), GUIColor(0.32f, 0.47f, 1f)]
        public void ReplaceStat(NStatEnum statEnum, float value,NStatCategory statCategory = NStatCategory.Flat)
        {
            var stat = new NStatField();
            stat.Initialize(statEnum, "", null, value);
            stat.SetCategory(statCategory);
            RemoveStat(statEnum,statCategory);
            statList.Add(stat);
        }
        [Button(ButtonSizes.Large), TabGroup("REPLACE"), GUIColor(0.32f, 0.47f, 1f)]
        public void ReplaceStatCustom(NStatField stat)
        {
            RemoveStatCustom(stat);
            statList.Add(stat);
        }
        
        [Button(ButtonSizes.Large), TabGroup("REMOVE"), GUIColor(1f, 0.4f, 0.4f)]
        public void ClearStats(bool apply)
        {
            if (!apply)
            {
                return;
            }
            statList.Clear();
        }
        #endregion
        
        #region Utils
        public void UpdateDict(Dictionary<string, float> dict)
        {
            foreach (var stat in statList)
                stat.ApplyToLocal(dict);
        }
        public void Clear()
        {
            statList.Clear();
        }
        #endregion
        
        
    }
}