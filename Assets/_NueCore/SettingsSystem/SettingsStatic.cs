using _NueCore.SaveSystem;

namespace _NueCore.SettingsSystem
{
    public static class SettingsStatic
    {
        public static bool IsCRTOn()
        {
            var settingsSave = NSaver.GetSaveData<SettingsSave>();
            return settingsSave.isCrtOn;
        }
    }
}