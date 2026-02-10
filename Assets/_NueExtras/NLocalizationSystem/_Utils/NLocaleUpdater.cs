using System;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace _NueExtras.NLocalizationSystem._Utils
{
    public class NLocaleUpdater : IDisposable
    {
        public Action OnUpdateAction { get; private set; }
        public NLocaleUpdater(Action onUpdate)
        {
            OnUpdateAction += onUpdate;
            LocalizationSettings.SelectedLocaleChanged += LocalizationSettingsOnSelectedLocaleChanged;

        }

        public void Dispose()
        {
            LocalizationSettings.SelectedLocaleChanged -= LocalizationSettingsOnSelectedLocaleChanged;
        }

        private void LocalizationSettingsOnSelectedLocaleChanged(Locale obj)
        {
            OnUpdateAction?.Invoke();
        }
    }
}