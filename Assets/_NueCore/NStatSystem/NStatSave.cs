using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _NueCore.NStatSystem
{
    [Serializable]
    public class NStatSave
    {
        public string key;
        public float flatValue;
        public float percentValue;
        
        [ShowInInspector,ReadOnly]private float Total
        {
            get
            {

                if (flatValue ==0 && percentValue != 0)
                {
                    return percentValue;
                }
                
                return flatValue + (percentValue * flatValue / 100f);
            }
        }

        public float GetTotal() => Total;
        public int GetTotalRounded() => Mathf.RoundToInt(Total);

        public void AddNStat(NStatField field)
        {
            if (field.GetStatCategory() is NStatCategory.Flat)
            {
                flatValue += field.GetValue();
            }
            else if (field.GetStatCategory() is NStatCategory.Percent)
            {
                percentValue += field.GetValue();
            }
        }

        public void AddNStat(float value, NStatCategory category)
        {
            if (category is NStatCategory.Flat)
            {
                flatValue += value;
            }
            else if (category is NStatCategory.Percent)
            {
                percentValue += value;
            }
        }
    }
}