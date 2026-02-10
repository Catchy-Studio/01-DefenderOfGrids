using System;
using UnityEngine;

namespace _NueExtras.RaritySystem
{
    public static class RarityStatic
    {
        public static Color GetRarityColor(NRarity rarity)
        {
            return rarity switch
            {
                NRarity.Common => new Color(0.8f, 0.8f, 0.8f),
                NRarity.Uncommon => new Color(0.2f, 0.8f, 0.2f),
                NRarity.Rare => new Color(0.2f, 0.2f, 0.8f),
                NRarity.Epic => new Color(0.8f, 0.2f, 0.8f),
                NRarity.Legendary => new Color(0.8f, 0.4f, 0.04f),
                NRarity.Cursed => new Color(0.8f, 0.11f, 0.1f),
                _ => new Color(0.8f, 0.8f, 0.8f)
            };
        }

       
        public static string GetRarityName(NRarity rarity)
        {
            return rarity switch
            {
                NRarity.Common => "Common",
                NRarity.Uncommon => "Uncommon",
                NRarity.Rare => "Rare",
                NRarity.Epic => "Epic",
                NRarity.Legendary => "Legendary",
                NRarity.Cursed => "Cursed",
                _ => "Common"
            };
        }

        private static int MaxRarity = -1;

        public static int GetMaxRarity()
        {
            if (MaxRarity < 0)
            {
                var cnt = Enum.GetNames(typeof(NRarity)).Length;
                MaxRarity = cnt;
            }

            return MaxRarity;
        }
    }
}