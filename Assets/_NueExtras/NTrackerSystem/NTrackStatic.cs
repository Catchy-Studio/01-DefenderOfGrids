using System.Collections.Generic;
using _NueCore.SaveSystem;

namespace _NueExtras.NTrackerSystem
{
    public static class NTrackStatic
    {
        //Active: Holds in-game stats, should be saved manually
        //Session: Holds last active save, do not modify this manually
        //Global: Holds permanent save
        #region Active
        private static TrackInfo ActiveTrack { get; set; }

        public static TrackInfo GetActiveStats()
        {
            if (ActiveTrack == null)
                LoadActiveStats();
            return ActiveTrack;
        }
        public static void SaveActiveStatsToSession()
        {
            var save = NSaver.GetSaveData<TrackSave_Global>();
            save.sessionTrack = ActiveTrack;
            save.Save();
        }
        
        public static void LoadActiveStats()
        {
            var save = NSaver.GetSaveData<TrackSave_Global>();
            if (save.sessionTrack == null)
                ResetSession();
            ActiveTrack = save.sessionTrack;
        }
        
        public static void SetStat<T>(string key, T value)
        {
            if (ActiveTrack == null)
                ActiveTrack = new TrackInfo();
            
            switch (value)
            {
                case int intValue:
                    ActiveTrack.SetInt(key, intValue);
                    break;
                case string strValue:
                    ActiveTrack.SetString(key, strValue);
                    break;
                case float floatValue:
                    ActiveTrack.SetFloat(key, floatValue);
                    break;
                case double doubleValue:
                    ActiveTrack.SetDouble(key, doubleValue);
                    break;
                case decimal decimalValue:
                    ActiveTrack.SetDecimal(key, decimalValue);
                    break;
                default:
                    throw new System.NotSupportedException($"Type {typeof(T)} is not supported for stats.");
            }
        }
        
        public static void SetStat<T>(TrackType key, T value)
        {
            SetStat(key.GetKey(), value);
        }
        
        public static T GetStat<T>(string key)
        {
            if (ActiveTrack == null)
            {
                LoadActiveStats();
            }

            try
            {
                return ActiveTrack?.GetValue(key) is T value ? value : default;
            }
            catch (KeyNotFoundException)
            {
                return default;
            }
        }
        
        public static T GetStat<T>(TrackType key)
        {
            return GetStat<T>(key.GetKey());
        }
        #endregion
        
        #region Session
        public static void ResetSession()
        {
            ActiveTrack = new TrackInfo();
            var save = NSaver.GetSaveData<TrackSave_Global>();
            save.sessionTrack = ActiveTrack;
            save.Save();
        }
        
        public static void SaveLastSession()
        {
            var save = NSaver.GetSaveData<TrackSave_Global>();
            if (save.sessionTrack == null)
                return;
            save.globalTrack += save.sessionTrack;
            save.sessionTrack = null;
            save.Save();
        }
        #endregion

        #region Global
        public static T GetGlobalStat<T>(string key)
        {
            var save = NSaver.GetSaveData<TrackSave_Global>();
            if (save?.globalTrack == null)
                return default;
            
            object value;
            try
            {
                value = save.globalTrack.GetValue(key);
            }
            catch (KeyNotFoundException)
            {
                return default;
            }
            
            if (value is T typedValue)
                return typedValue;
            
            return default;
        }
        public static T GetGlobalStat<T>(TrackType key)
        {
            return GetGlobalStat<T>(key.GetKey());
        }
        public static void SetGlobalStat<T>(string key, T value)
        {
            var save = NSaver.GetSaveData<TrackSave_Global>();
            if (save?.globalTrack == null)
                return;

            save.globalTrack.SetValue(key, value);
            save.Save();
        }

        public static void SetGlobalStat<T>(TrackType key, T value)
        {
            SetGlobalStat(key.GetKey(), value);
        }
        
        

        #endregion
    }
}