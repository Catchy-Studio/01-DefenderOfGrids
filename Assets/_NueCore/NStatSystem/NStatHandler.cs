using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _NueCore.NStatSystem
{
    [ShowInInspector,ReadOnly,Serializable]
    public class NStatHandler
    {
        #region Actions
        public Action OnStatsUpdatedAction;
        public Action<string> OnStatsUpdatedSpecificAction;
        

        #endregion
        
        #region Cache
        [ShowInInspector, ReadOnly,TableList(ShowPaging = true,ShowIndexLabels = true,NumberOfItemsPerPage = 30)]
        public Dictionary<string, NStatSave> LocalNStatDict { get; set; } = new Dictionary<string, NStatSave>();
        

        #endregion

        #region Setup
        public void ResetStats()
        {
            LocalNStatDict.Clear();
        }
        public void InitStats(NStatList statList)
        {
            InitStats(statList.GetStatList());
        }
        public void InitStats(List<NStatField> statList)
        {
            LocalNStatDict.Clear();
            var baseSave = statList;

            foreach (var nStatField in baseSave)
                IncreaseStat(nStatField.Key,nStatField.GetValue(),nStatField.GetStatCategory(),false);
        }
        
        public void InitStats(List<NStatSave> statList)
        {
            LocalNStatDict.Clear();
            var baseSave = statList;

            foreach (var nStatField in baseSave)
            {
                IncreaseStat(nStatField.key,nStatField.flatValue,NStatCategory.Flat,false);
                IncreaseStat(nStatField.key,nStatField.percentValue,NStatCategory.Percent,false);
            }
        }
        #endregion

        #region Methods
        public float GetTotalStatValue(string key)
        {
            var stat = GetStat(key);
            if (stat == null)
                return 0;

            if (stat.flatValue == 0)
                return stat.percentValue;
            var calculatedStat = stat.flatValue + (stat.flatValue * stat.percentValue / 100f);
            return calculatedStat;
        }
        public float GetTotalStatValue(NStatEnum statEnum) => GetTotalStatValue(statEnum.GetStatKey());
        public int GetTotalStatValueRounded(string key)
        {
            return Mathf.RoundToInt(GetTotalStatValue(key));
        }

        public int GetTotalStatValueRounded(NStatEnum statEnum) => Mathf.RoundToInt(GetTotalStatValue(statEnum));
        public float GetStatValue(string key, NStatCategory increaseStatCategory)
        {
            var stat = GetStat(key);
            if (stat == null)
                return 0;

            if (increaseStatCategory == NStatCategory.Flat)
                return stat.flatValue;
            return stat.percentValue;
        }
        public float GetStatValue(NStatEnum statEnum, NStatCategory increaseStatCategory) =>
            GetStatValue(statEnum.GetStatKey(), increaseStatCategory);

        public void IncreaseStat(string key, float value, NStatCategory increaseStatCategory = NStatCategory.Flat, bool update = true)
        {
            if (!LocalNStatDict.ContainsKey(key))
            {
                var newStat = new NStatSave
                {
                    key = key
                };
                if (increaseStatCategory == NStatCategory.Flat)
                    newStat.flatValue += value;
                else
                    newStat.percentValue += value;
                LocalNStatDict.Add(key, newStat);
            }
            else
            {
                if (increaseStatCategory == NStatCategory.Flat)
                    LocalNStatDict[key].flatValue += value;
                else
                    LocalNStatDict[key].percentValue += value;
            }

            if (update)
            {
                UpdateStats(key);
            }
        }
        public void SetStat(string key, float value, NStatCategory increaseStatCategory = NStatCategory.Flat, bool update = true)
        {
            if (!LocalNStatDict.ContainsKey(key))
            {
                var newStat = new NStatSave
                {
                    key = key
                };
                if (increaseStatCategory == NStatCategory.Flat)
                    newStat.flatValue = value;
                else
                    newStat.percentValue = value;
                LocalNStatDict.Add(key, newStat);
            }
            else
            {
                if (increaseStatCategory == NStatCategory.Flat)
                    LocalNStatDict[key].flatValue = value;
                else
                    LocalNStatDict[key].percentValue = value;
            }

            if (update)
            {
                UpdateStats(key);
            }
        }
        public void SetStat(NStatEnum statEnum, float value, NStatCategory increaseStatCategory = NStatCategory.Flat, bool update = true)
        {
            var key = statEnum.GetStatKey();
            if (!LocalNStatDict.ContainsKey(key))
            {
                var newStat = new NStatSave
                {
                    key = key
                };
                if (increaseStatCategory == NStatCategory.Flat)
                    newStat.flatValue = value;
                else
                    newStat.percentValue = value;
                LocalNStatDict.Add(key, newStat);
            }
            else
            {
                if (increaseStatCategory == NStatCategory.Flat)
                    LocalNStatDict[key].flatValue = value;
                else
                    LocalNStatDict[key].percentValue = value;
            }

            if (update)
            {
                UpdateStats(key);
            }
        }

        public void IncreaseStat(NStatList statList, bool update = false)
        {
            foreach (var nStatField in statList.GetStatList())
                IncreaseStat(nStatField.Key,nStatField.GetValue(),nStatField.GetStatCategory(), false);


            if (update)
                UpdateStats();
        }
        
        public void IncreaseStat(NStatEnum statEnum, float value, NStatCategory increaseStatCategory = NStatCategory.Flat,
            bool update = false) =>
            IncreaseStat(statEnum.GetStatKey(), value, increaseStatCategory, update);
        private NStatSave GetStat(string key)
        {
            return LocalNStatDict.GetValueOrDefault(key);
        }
        private NStatSave GetStat(NStatEnum key)
        {
            return GetStat(key.GetStatKey());
        }
        public virtual void UpdateStats(string key)
        {
            OnStatsUpdatedSpecificAction?.Invoke(key);
            OnStatsUpdatedAction?.Invoke();
        }
        public virtual void UpdateStats()
        {
            //OnStatsUpdatedSpecificAction?.Invoke(key);
            OnStatsUpdatedAction?.Invoke();
        }
        public bool HasStat(NStatEnum statEnum)
        {
            return GetTotalStatValueRounded(statEnum) != 0;
        }
        public bool HasStat(string statEnum)
        {
            return GetTotalStatValueRounded(statEnum) != 0;
        }
        public bool TryStat(NStatEnum statEnum, out int totalValue)
        {
            totalValue = 0;
            if (HasStat(statEnum))
            {
                totalValue = GetTotalStatValueRounded(statEnum);
                return true;
            }

            return false;
        }
        public bool TryStat(string statEnum, out int totalValue)
        {
            totalValue = 0;
            if (HasStat(statEnum))
            {
                totalValue = GetTotalStatValueRounded(statEnum);
                return true;
            }

            return false;
        }
        #endregion
    }
}