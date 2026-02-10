using System.Collections.Generic;
using System.Text;
using _NueCore.Common.Utility;
using _NueExtras.StockSystem;
using DamageNumbersPro;
using UnityEngine;

namespace __Project.Systems.DamageNumberSystem
{
    public static class NumberStatic
    {
        
        public static Dictionary<NumberTypes, DamageNumber> NumberDict { get; private set; } =
            new Dictionary<NumberTypes, DamageNumber>();
       
        public static DamageNumber GetDamageNumber(NumberTypes targetType)
        {
            return NumberDict[targetType].Spawn();
        }
        
        public static DamageNumber ShowStock(float value,StockTypes stockType,Vector3 pos)
        {
            var number = GetDamageNumber(NumberTypes.Stock).Spawn();
            number.enableLeftText = false;
            number.enableRightText = false;
            number.enableNumber = true;
            var str = new StringBuilder();
            str.Append(value <= 0 ? "" : "+").Append(value).Append(SpriteHelper.GetStockSpriteText(stockType));
            number.number = value;
            if (value<0)
            {
                number.SetColor(Color.red);
            }
            else
            {
                number.SetColor(Color.yellow);
            }
            number.SetPosition(pos);
            return number;
        }
        
        
        public static DamageNumber ShowCustom(NumberInfo info)
        {
            if (info.damage == 0)
                return null;
            var str = new StringBuilder();
            var prefab = NumberDict[info.numberType];
            var pos = info.position;
            
            var damageNumber = prefab.Spawn(pos, Mathf.CeilToInt(info.damage));
            if (info.followTransform)
                damageNumber.followedTarget = info.followTransform;
            str.Append(info.spamID).Append("_").Append(info.numberType);
            if (info.isCritical)
            {
                damageNumber.durationFadeIn = 0.4f;
                damageNumber.durationFadeOut = 0.2f;
                damageNumber.enableShakeFadeIn = true;
                damageNumber.shakeOffsetFadeIn = new Vector2(0.1f, 0);
                damageNumber.shakeFrequencyFadeIn = 30;
                damageNumber.enableRotateOverTime = true;
                damageNumber.minRotationSpeed = -5;
                damageNumber.maxRotationSpeed = 5;
                damageNumber.enableScaleOverTime = true;
                str.Append("_Critical");
            }
            damageNumber.spamGroup =str.ToString();
            return damageNumber;
        }
    }
}