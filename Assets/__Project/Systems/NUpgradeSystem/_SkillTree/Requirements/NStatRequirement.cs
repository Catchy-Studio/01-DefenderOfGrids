using System;
using _NueCore.NStatSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace __Project.Systems.NUpgradeSystem._SkillTree.Requirements
{
    [Serializable]
    public class NStatRequirement
    {
        [SerializeField, HorizontalGroup("Main")] 
        private NStatEnum statEnum;
        
        [SerializeField, HorizontalGroup("Main")]
        private ComparisonType comparison = ComparisonType.GreaterThan;
        
        [SerializeField, HorizontalGroup("Main")] 
        private float requiredValue = 0;

        public NStatEnum StatEnum => statEnum;
        public ComparisonType Comparison => comparison;
        public float RequiredValue => requiredValue;

        public bool IsSatisfied()
        {
            var currentValue = UpgradeStatic.GetTotalStat(statEnum);
            
            return comparison switch
            {
                ComparisonType.GreaterThan => currentValue > requiredValue,
                ComparisonType.GreaterOrEqual => currentValue >= requiredValue,
                ComparisonType.Equal => Mathf.Approximately(currentValue, requiredValue),
                ComparisonType.LessOrEqual => currentValue <= requiredValue,
                ComparisonType.LessThan => currentValue < requiredValue,
                ComparisonType.NotEqual => !Mathf.Approximately(currentValue, requiredValue),
                _ => false
            };
        }

        public string GetDescription()
        {
            var compSymbol = comparison switch
            {
                ComparisonType.GreaterThan => ">",
                ComparisonType.GreaterOrEqual => "≥",
                ComparisonType.Equal => "=",
                ComparisonType.LessOrEqual => "≤",
                ComparisonType.LessThan => "<",
                ComparisonType.NotEqual => "≠",
                _ => "?"
            };

            return $"{statEnum} {compSymbol} {requiredValue}";
        }
    }

    public enum ComparisonType
    {
        GreaterThan,
        GreaterOrEqual,
        Equal,
        LessOrEqual,
        LessThan,
        NotEqual
    }
}

