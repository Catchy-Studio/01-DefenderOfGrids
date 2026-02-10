using System.Text;
using _NueCore.Common.NueLogger;
using UnityEngine;

namespace _NueCore.NStatSystem
{
    public static class NStatStatic
    {
        public static StringBuilder ConvertNStatStr(this StringBuilder str,NStatHandler nStatHandler)
        {
            foreach (var kvp in nStatHandler.LocalNStatDict)
                str.Replace($"#{kvp.Key}", nStatHandler.GetTotalStatValueRounded(kvp.Key).ToString());
            return str;
        }
        
        public static string ConvertNStatStr(this string str,NStatHandler nStatHandler)
        {
            foreach (var kvp in nStatHandler.LocalNStatDict)
                str = str.Replace($"#{kvp.Key}", nStatHandler.GetTotalStatValueRounded(kvp.Key).ToString());
            return str;
        }

        public static string ConvertNStatStr(this string str, NStatList statList)
        {
            foreach (var stat in statList.GetStatList())
                str = str.Replace($"#{stat.Key}", stat.GetValue().ToString("0.0"));
            return str;
        }
        
        public static string ConvertNStatStrWithCustomPrefix(this string str, string customPrefix,NStatList statList)
        {
            foreach (var stat in statList.GetStatList())
            {
                var t = $"#{customPrefix}{stat.Key}";
                str = str.Replace(t, stat.GetValue().ToString("0.0"));
            }
            return str;
        }
    }
}