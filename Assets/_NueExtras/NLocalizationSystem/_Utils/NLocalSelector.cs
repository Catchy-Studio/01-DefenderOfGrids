using System.Collections;
using System.Collections.Generic;
using _NueCore.SaveSystem;
using _NueCore.SettingsSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace _NueExtras.NLocalizationSystem._Utils
{
    public class NLocalSelector : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown dropdown;

        private List<Locale> _locales = new List<Locale>();
        public void Init(List<Locale> activeLocaleList)
        {
            _locales.Clear();
            _locales.AddRange(activeLocaleList);
            // Generate list of available Locales
            var options = new List<TMP_Dropdown.OptionData>();
            int selected = 0;
            
            for (int i = 0; i < _locales.Count; ++i)
            {
                var locale = _locales[i];
                if (LocalizationSettings.SelectedLocale == locale)
                    selected = i;
                options.Add(new TMP_Dropdown.OptionData(locale.LocaleName));
            }
            dropdown.options = options;

            dropdown.value = selected;
            dropdown.onValueChanged.AddListener(LocaleSelected);
            
        }
        private void LocaleSelected(int index)
        {
            if (index < 0 || index >= _locales.Count)
            {
                Debug.LogWarning($"Index {index} is out of range for locales list.");
                return;
            }
            var locale = _locales[index];
            LocalizationSettings.SelectedLocale = locale;
            var save = NSaver.GetSaveData<SettingsSave>();
            var localeID = locale.Identifier.Code;
            save.localeID = localeID;
            save.Save();
        }
    }
}