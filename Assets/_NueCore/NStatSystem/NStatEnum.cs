using System;
using NueGames.NTooltip._Keyword;

namespace _NueCore.NStatSystem
{
    public enum NStatEnum
    {
        None = 0,
        
        N1 = 1,
        N2 = 2,
        N3 = 3,
        N4 = 4,
        
        Coin = 9000,
        Gem =9001,
        Emerald =9002,
       
        Unlock_Game = 1001,
        
    }
    public static class NCustomStats
    {
        

    }
    public static class NStatEnumExtension
    {
        public static bool IsValidToDisplay(this NStatEnum nStatEnum)
        {
            if (nStatEnum 
                is NStatEnum.N1
                or NStatEnum.N2
                or NStatEnum.N3
                or NStatEnum.N4
                or NStatEnum.None
                ){
                return false;
            }
            return true;
        }
        
        public static string GetTotalKey(this NStatEnum statEnum)
        {
            return GetCustomKey(statEnum, "t_");
        }
        
        public static string GetCurrentKey(this NStatEnum statEnum)
        {
            return GetCustomKey(statEnum, "c_");
        }
        
        
        public static string GetCustomKey(this NStatEnum statEnum,string prefix)
        {
            return statEnum switch
            {
                _ => $"#{prefix}{statEnum}"
            };
        }

        public static bool TryConvertToNStatEnum(this string stringKey,out NStatEnum stat)
        {
            return Enum.TryParse(stringKey, out stat);
        }
        public static string GetStatKey(this NStatEnum statEnum)
        {
            return statEnum switch
            {
                _ => statEnum.ToString()
            };
        }
        public static string GetStatDisplayName(this NStatEnum statEnum)
        {
            return statEnum switch
            {
                NStatEnum.Coin => "<COIN>".ApplyKeywords(),
                NStatEnum.Gem => "<GEM>".ApplyKeywords(),
                NStatEnum.Emerald => "<EMERALD>".ApplyKeywords(),
                _ => statEnum.ToString()
            };
        }
    }
}