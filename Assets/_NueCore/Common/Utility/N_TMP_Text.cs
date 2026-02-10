using System.Globalization;
using _NueCore.NStatSystem;
using NueGames.NTooltip._Keyword;
using TMPro;
using UnityEngine;

namespace _NueCore.Common.Utility
{
    [RequireComponent(typeof(TMP_Text))]
    public class N_TMP_Text : MonoBehaviour
    {
        public const string VALUE = "#value";
        private bool _isSet;
        private string _baseText;

        public TMP_Text TMP
        {
            get
            {
                if (!_tmp)
                {
                    _tmp = GetComponent<TMP_Text>();
                }
                return _tmp;
            }
        }

        private TMP_Text _tmp;
        public string BaseText
        {
            get
            {
                if (_isSet) 
                    return _baseText;
                
                _isSet = true;
                _baseText = TMP.text;
                
                return _baseText;
            }
        }

        public void SetCurrentMax(int current, int max)
        {
            var str = Replace("#current", current.ToString());
            str = str.Replace("#max", max.ToString());
            TMP.SetText(str);
        }
        
        public void SetCurrentMax(float current, float max)
        {
            var str = Replace("#current", current.ToString("F2",CultureInfo.InvariantCulture));
            str = str.Replace("#max", max.ToString("F2",CultureInfo.InvariantCulture));
            TMP.SetText(str);
        }

        public string Replace(string key, string value)
        {
            return BaseText.Replace(key, value);
        }

        public void Set(string key, string value)
        {
            TMP.SetText(Replace(key, value));
        }

        public string ReplaceValue(string valueText)
        {
            return BaseText.Replace(VALUE, valueText);
        }

        public void SetValue(string valueText,bool applyKeywords = false)
        {
            var t = ReplaceValue(valueText);
            if (applyKeywords)
                t =t.ApplyKeywords();
            TMP.SetText(t);
        }
        
        public void ConvertNStatStr(NStatList statList)
        {
            var t = BaseText.ConvertNStatStr(statList);
            TMP.SetText(t);
        }

        public void SetText(string text)
        {
            TMP.SetText(text);
        }
    }
}