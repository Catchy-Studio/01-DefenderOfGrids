using System;
using System.Collections.Generic;
using _NueCore.Common.KeyValueDict;
using _NueCore.Common.NueLogger;
using _NueCore.ManagerSystem.Core;
using _NueCore.NStatSystem;
using Sirenix.OdinInspector;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.TextCore.Text;

namespace _NueExtras.NLocalizationSystem
{
    public class NLocalizationManager : NManagerBase
    {
        [SerializeField] private KeyValueDict<Locale,LocaleInfo> localeInfoDict = new KeyValueDict<Locale, LocaleInfo>();
       
        #region Cache
        [ShowInInspector,ReadOnly]private static List<NLocal_Text> ActiveTextsList { get; set; }= new List<NLocal_Text>(); 
        public static NLocalizationManager Instance;

        

        #endregion
        #region Setup
        public static void Register(NLocal_Text obj)
        {
            if(obj == null) 
                return;

            ActiveTextsList.Add(obj);
        }
        public static void Unregister(NLocal_Text obj)
        {
            if(obj == null) 
                return;

            ActiveTextsList.Remove(obj);
        }
        public override void NAwake()
        {
            Instance = InitSingleton<NLocalizationManager>();
            base.NAwake();
        }
        #endregion

        #region Methods
        public bool TryCheckLocalizedFont(Locale locale,TMP_Text tmp)
        {
            if (localeInfoDict.TryGetValue(locale,out var foundedInfo))
            {
                if (foundedInfo.TrySwitchFont(tmp.font,out var newFont))
                {
                    tmp.font = newFont;
                    return true;
                }
            }

            return false;
        }


        #endregion

        #region Class
        [Serializable]
        public class LocaleInfo
        {
            [SerializeField] private KeyValueDict<TMP_FontAsset,TMP_FontAsset> fontSwitchDict = new KeyValueDict<TMP_FontAsset, TMP_FontAsset>();

            public bool TrySwitchFont(TMP_FontAsset font, out TMP_FontAsset newFont)
            {
                return fontSwitchDict.TryGetValue(font,out newFont);
            }
            
        }
        
        [Serializable]
        public class LocaleFontSwitcher
        {
            
        }
        #endregion

        #region NStat
        [SerializeField] private KeyValueDict<NStatEnum,NLocal_Field_String> statLocalDict = new KeyValueDict<NStatEnum, NLocal_Field_String>();
        [SerializeField] private KeyValueDict<NStatEnum,NLocal_Field_String> statDisplayLocalDict = new KeyValueDict<NStatEnum, NLocal_Field_String>();

        public string GetLocalString(NStatEnum stat)
        {
            if (statLocalDict.TryGetValue(stat,out var t))
            {
                return t.GetString();
            }

            return stat.GetStatKey();
        }
        public string GetDisplayLocalString(NStatEnum stat)
        {
            if (statDisplayLocalDict.TryGetValue(stat,out var t))
            {
                return t.GetString();
            }

            return stat.GetStatDisplayName();
        }
#if UNITY_EDITOR
        [Button]
        private void SetDefaultNStats(bool apply)
        {
            if (!apply)
            {
                return;
            }

            var enums = Enum.GetNames(typeof(NStatEnum));
            for (int i = 0; i < enums.Length; i++)
            {
                if (i >= enums.Length)
                {
                    break;
                }

                var tEnum = (NStatEnum)i;
                if (!statLocalDict.ContainsKey(tEnum))
                {
                    var newLocal = new NLocal_Field_String();
                    statLocalDict.Add(tEnum,newLocal);
                }
                var statLocal =  statLocalDict[tEnum];
                statLocal.SetFieldLocal(new NLocalParams(
                    statLocal, tEnum.GetStatKey(), $"NSTAT/{tEnum}", NTables.NStatTable, false, true));

                if (!statDisplayLocalDict.ContainsKey(tEnum))
                {
                    var newLocal = new NLocal_Field_String();
                    statDisplayLocalDict.Add(tEnum,newLocal);
                }
                var tLocal =  statDisplayLocalDict[tEnum];
                tLocal.SetFieldLocal(new NLocalParams(
                    tLocal, tEnum.GetStatDisplayName(), $"NSTAT/Display/{tEnum}", NTables.NStatTable, false, true));

            }
            
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
        }

#endif
       
        #endregion
    }
}