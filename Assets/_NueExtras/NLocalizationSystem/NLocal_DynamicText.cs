using System.Collections.Generic;
using _NueCore.Common.NueLogger;
using TMPro;
using UnityEngine;

namespace _NueExtras.NLocalizationSystem
{
    //If NLocal_Text dynamicly changes, add this component
    [RequireComponent(typeof(TMP_Text),typeof(NLocal_Text))]
    public class NLocal_DynamicText : MonoBehaviour
    {
        #region Cache
        public const string VALUE = "#value";
        private bool _isSet;
        private string _baseText;
        private Dictionary<string,string> _cacheDict = new Dictionary<string, string>();
        public TMP_Text TMP
        {
            get
            {
                if (!_tmp)
                    _tmp = GetComponent<TMP_Text>();
                return _tmp;
            }
        }
        private TMP_Text _tmp;
        public string BaseText
        {
            get
            {
                if (_localText)
                {
                    var tRef = _localText.LocalizedStringEvent.StringReference;
                    if (tRef != null)
                    {
                        return tRef.GetLocalizedString();
                    }
                }
                if (_isSet) 
                    return _baseText;
                
                _isSet = true;
                _baseText = TMP.text;
                
                return _baseText;
            }
        }
        private NLocal_Text _localText;
        #endregion

        #region Setup
        private void Awake()
        {
            if (gameObject.TryGetComponent<NLocal_Text>(out var localString))
            {
                _localText = localString;
                _localText.OnUpdateAction += () =>
                {
                    foreach (var kvp in _cacheDict)
                    {
                        var t =TMP.text;
                        $"Key: {kvp.Key} <> Text: {t} <> Replace: {kvp.Value}".NLog(Color.yellow);
                        t =t.Replace(kvp.Key, kvp.Value);
                        TMP.SetText(t);
                    }
                };
            }
        }
        #endregion
        
        #region Methods
        private void AddCache(string key, string value)
        {
            if (_cacheDict.ContainsKey(key))
            {
                _cacheDict[key] = value;
            }
            else
            {
                _cacheDict.Add(key, value);
            }
        }
        public void SetCurrentMax(int current, int max)
        {
            var c = current.ToString();
            var m = max.ToString();
            var cKey = "#current";
            var mKey = "#max";
            AddCache(cKey,c);
            AddCache(mKey,m);
            var str = Replace(cKey, current.ToString());
            str = str.Replace(mKey, max.ToString());
            TMP.SetText(str);
        }

        public string Replace(string key, string value)
        {
            return BaseText.Replace(key, value);
        }

        public void Set(string key, string value)
        {
            AddCache(key,value);
            TMP.SetText(Replace(key, value));
        }

        public string ReplaceValue(string valueText)
        {
            AddCache(VALUE,valueText);
            return BaseText.Replace(VALUE, valueText);
        }

        public void SetValue(string valueText)
        {
            TMP.SetText(ReplaceValue(valueText));
        }
        #endregion
    }
}