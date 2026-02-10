using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _NueCore.NStatSystem
{
    [Serializable]
    public class NStatField
    {
        [SerializeField,HideIf("@container != null || key.Length>0")] private NStatEnum statEnum;
        [SerializeField,HideIf("@container != null || statEnum != NStatEnum.None")] private string key = "";
        [SerializeField,HideIf("@key.Length>0 || statEnum != NStatEnum.None")] private NStatContainer container;
        [SerializeField] private float value;
        [SerializeField] private NStatCategory statCategory;


        #region Cache
        public virtual string Key
        {
            get
            {
                if (StatEnum != NStatEnum.None)
                {
                    return StatEnum.GetStatKey();
                }
                return Container ? Container.Key : key;
            }
        }

        public NStatEnum StatEnum => statEnum;

        public NStatContainer Container => container;

        #endregion
        
        #region Setup
        public void Initialize(NStatEnum s, string k, NStatContainer c, float v)
        {
            statEnum = s;
            key = k;
            container = c;
            value = v;
        }
        

        #endregion

        #region Methods
        public string GetRawKey()
        {
            return key;
        }
        public void ApplyToLocal(Dictionary<string,float> dict)
        {
            if (!dict.TryAdd(Key, value))
            {
                dict[Key] += value;
            }
        }
        public float GetValue()
        {
            return value;
        }

        public int GetValueRounded()
        {
            return Mathf.RoundToInt(GetValue());
        }
        
        public NStatCategory GetStatCategory()
        {
            return statCategory;
        }
        public void SetCategory(NStatCategory stat)
        {
            statCategory = stat;
        }
        public void SetValue(float v)
        {
            this.value = v;
        }
        public void SetKey(string k)
        {
            this.key = k;
        }
        public void SetKey(NStatEnum e)
        {
            this.statEnum = e;
        }
        public void IncreaseValue(float v)
        {
            this.value += v;
        }
        #endregion
       
    }
}