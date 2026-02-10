using System;
using System.Collections.Generic;
using _NueCore.Common.KeyValueDict;
using Sirenix.OdinInspector;

namespace _NueExtras.NTrackerSystem
{
    [Serializable]
    public class TrackInfo
    {
        [ShowInInspector,ReadOnly]public KeyValueDict<string, string> StringDict = new KeyValueDict<string, string>();
        [ShowInInspector,ReadOnly]public KeyValueDict<string, int> IntDict = new KeyValueDict<string, int>();
        [ShowInInspector,ReadOnly]public KeyValueDict<string, float> FloatDict = new KeyValueDict<string, float>();
        [ShowInInspector,ReadOnly]public KeyValueDict<string, double> DoubleDict = new KeyValueDict<string, double>();
        [ShowInInspector,ReadOnly]public KeyValueDict<string, decimal> DecimalDict = new KeyValueDict<string, decimal>();

        #region Getters
        public int GetInt(string key)
        {
            return IntDict.ContainsKey(key) ? IntDict[key] : 0;
        }
        public string GetString(string key)
        {
            return StringDict.ContainsKey(key) ? StringDict[key] : string.Empty;
        }
        public float GetFloat(string key)
        {
            return FloatDict.ContainsKey(key) ? FloatDict[key] : 0f;
        }
        public double GetDouble(string key)
        {
            return DoubleDict.ContainsKey(key) ? DoubleDict[key] : 0d;
        }
        public decimal GetDecimal(string key)
        {
            return DecimalDict.ContainsKey(key) ? DecimalDict[key] : 0m;
        }
        
        public void SetInt(string key, int value)
        {
            if (IntDict.ContainsKey(key))
                IntDict[key] = value;
            else
                IntDict.Add(key, value);
        }
        
        public void SetString(string key, string value)
        {
            if (StringDict.ContainsKey(key))
                StringDict[key] = value;
            else
                StringDict.Add(key, value);
        }
        
        public void SetFloat(string key, float value)
        {
            if (FloatDict.ContainsKey(key))
                FloatDict[key] = value;
            else
                FloatDict.Add(key, value);
        }
        
        public void SetDouble(string key, double value)
        {
            if (DoubleDict.ContainsKey(key))
                DoubleDict[key] = value;
            else
                DoubleDict.Add(key, value);
        }
        public void SetDecimal(string key, decimal value)
        {
            if (DecimalDict.ContainsKey(key))
                DecimalDict[key] = value;
            else
                DecimalDict.Add(key, value);
        }
        public void SetValue(string key, object value)
        {
            switch (value)
            {
                case int intValue:
                    SetInt(key, intValue);
                    break;
                case string strValue:
                    SetString(key, strValue);
                    break;
                case float floatValue:
                    SetFloat(key, floatValue);
                    break;
                case double doubleValue:
                    SetDouble(key, doubleValue);
                    break;
                case decimal decimalValue:
                    SetDecimal(key, decimalValue);
                    break;
                default:
                    throw new ArgumentException($"Unsupported type: {value.GetType()} for key: {key}");
            }
        }
        
        public object GetValue(string key)
        {
            if (IntDict.ContainsKey(key))
                return IntDict[key];
            if (StringDict.ContainsKey(key))
                return StringDict[key];
            if (FloatDict.ContainsKey(key))
                return FloatDict[key];
            if (DoubleDict.ContainsKey(key))
                return DoubleDict[key];
            if (DecimalDict.ContainsKey(key))
                return DecimalDict[key];

            throw new KeyNotFoundException($"Key '{key}' not found in StatsInfo.");
        }
        #endregion

        #region Operators

        // Operator overloading for StatsInfo objects
        public static TrackInfo operator +(TrackInfo a, TrackInfo b)
        {
            var result = new TrackInfo();
            
            // Combine String dictionaries
            foreach (var kvp in a.StringDict)
                result.StringDict[kvp.Key] = kvp.Value;
            foreach (var kvp in b.StringDict)
                result.StringDict[kvp.Key] = kvp.Value;
                
            // Combine Int dictionaries
            foreach (var kvp in a.IntDict)
                result.IntDict[kvp.Key] = kvp.Value;
            foreach (var kvp in b.IntDict)
                result.IntDict[kvp.Key] = b.IntDict[kvp.Key] + (result.IntDict.ContainsKey(kvp.Key) ? result.IntDict[kvp.Key] : 0);
                
            // Combine Float dictionaries
            foreach (var kvp in a.FloatDict)
                result.FloatDict[kvp.Key] = kvp.Value;
            foreach (var kvp in b.FloatDict)
                result.FloatDict[kvp.Key] = b.FloatDict[kvp.Key] + (result.FloatDict.ContainsKey(kvp.Key) ? result.FloatDict[kvp.Key] : 0f);
                
            // Combine Double dictionaries
            foreach (var kvp in a.DoubleDict)
                result.DoubleDict[kvp.Key] = kvp.Value;
            foreach (var kvp in b.DoubleDict)
                result.DoubleDict[kvp.Key] = b.DoubleDict[kvp.Key] + (result.DoubleDict.ContainsKey(kvp.Key) ? result.DoubleDict[kvp.Key] : 0d);
                
            // Combine Decimal dictionaries
            foreach (var kvp in a.DecimalDict)
                result.DecimalDict[kvp.Key] = kvp.Value;
            foreach (var kvp in b.DecimalDict)
                result.DecimalDict[kvp.Key] = b.DecimalDict[kvp.Key] + (result.DecimalDict.ContainsKey(kvp.Key) ? result.DecimalDict[kvp.Key] : 0m);
            return result;
        }

        public static TrackInfo operator -(TrackInfo a, TrackInfo b)
        {
            var result = new TrackInfo();
            // Combine String dictionaries
            foreach (var kvp in a.StringDict)
                result.StringDict[kvp.Key] = kvp.Value;
            foreach (var kvp in b.StringDict)
                result.StringDict[kvp.Key] = kvp.Value;
            
            // Combine Int dictionaries
            foreach (var kvp in b.IntDict)
                result.IntDict[kvp.Key] = (result.IntDict.ContainsKey(kvp.Key) ? result.IntDict[kvp.Key] : 0) - b.IntDict[kvp.Key];
            
            // Combine Float dictionaries
            foreach (var kvp in a.FloatDict)
                result.FloatDict[kvp.Key] = kvp.Value;
            foreach (var kvp in b.FloatDict)
                result.FloatDict[kvp.Key] = (result.FloatDict.ContainsKey(kvp.Key) ? result.FloatDict[kvp.Key] : 0f) - b.FloatDict[kvp.Key];
            
            // Combine Double dictionaries
            foreach (var kvp in a.DoubleDict)
                result.DoubleDict[kvp.Key] = kvp.Value;
            foreach (var kvp in b.DoubleDict)
                result.DoubleDict[kvp.Key] = (result.DoubleDict.ContainsKey(kvp.Key) ? result.DoubleDict[kvp.Key] : 0d) - b.DoubleDict[kvp.Key];
            
            // Combine Decimal dictionaries
            foreach (var kvp in a.DecimalDict)
                result.DecimalDict[kvp.Key] = kvp.Value;
            foreach (var kvp in b.DecimalDict)
                result.DecimalDict[kvp.Key] = (result.DecimalDict.ContainsKey(kvp.Key) ? result.DecimalDict[kvp.Key] : 0m) - b.DecimalDict[kvp.Key];
            
            return result;
        }

        public static TrackInfo operator *(TrackInfo a, int multiplier)
        {
            var result = new TrackInfo();
            
            // Copy String dictionary (strings don't multiply)
            foreach (var kvp in a.StringDict)
                result.StringDict[kvp.Key] = kvp.Value;
            
            // Multiply Int dictionary
            foreach (var kvp in a.IntDict)
                result.IntDict[kvp.Key] = kvp.Value * multiplier;
            
            // Multiply Float dictionary
            foreach (var kvp in a.FloatDict)
                result.FloatDict[kvp.Key] = kvp.Value * multiplier;
            
            // Multiply Double dictionary
            foreach (var kvp in a.DoubleDict)
                result.DoubleDict[kvp.Key] = kvp.Value * multiplier;
            
            // Multiply Decimal dictionary
            foreach (var kvp in a.DecimalDict)
                result.DecimalDict[kvp.Key] = kvp.Value * multiplier;
                
            return result;
        }

        #endregion
       
    }
}