using System;
using System.Collections.Generic;
using System.Text;
using _NueCore.Common.NueLogger;
using _NueCore.Common.Utility;
using NueGames.NTooltip._Keyword;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

namespace _NueExtras.NLocalizationSystem
{
    public static class NLocalStatic
    {
        #region Extensions
        public static string NApplyLocal(this LocalizedString localizedString, Func<string,string> converterFunc = null)
        {
            return Convert(localizedString.GetLocalizedString(), converterFunc);
        }
        
        public static string NApplyLocal(this string localizedString, Func<string,string> converterFunc = null)
        {
            return Convert(localizedString, converterFunc);
        }

        private static string Convert(string text, Func<string,string> converterFunc = null)
        {
            if (converterFunc != null)
                text = converterFunc.Invoke(text);
            text =text.ApplyKeywords();
            return text;
        }

        

        #endregion
#if UNITY_EDITOR
        #region Table
        public static void SetTableDefault(LocalizedString localizedString, string defaultText)
        {
            var tableRef =localizedString.TableReference;
            var table = LocalizationSettings.StringDatabase.GetTable(tableRef,
                LocalizationSettings.AvailableLocales.GetLocale(new LocaleIdentifier("en")));
            if (table == null)
                return;
            var entryRef = localizedString.TableEntryReference;
            
            var t = table.GetEntryFromReference(entryRef);
            var title = defaultText;
            if (t == null)
                table.AddEntryFromReference(entryRef, title);
            else
                t.Value = title;
            EditorUtility.SetDirty(table);
            EditorUtility.SetDirty(table.SharedData);
            AssetDatabase.SaveAssetIfDirty(table);
            AssetDatabase.SaveAssetIfDirty(table.SharedData);
        }
          public static void KeySwap(string oldKey, string newKey)
        {
            var table = LocalizationSettings.StringDatabase.GetTable("NTable",
                LocalizationSettings.AvailableLocales.GetLocale(new LocaleIdentifier("en")));
            if (!table)
            {
                return;
            }

            var entry = table.GetEntry(oldKey);
            if (entry == null)
                return;
            var value = entry.Value;
            table.RemoveEntry(oldKey);
            table.SharedData.RemoveKey(oldKey);
            table.SharedData.AddKey(newKey);
            table.AddEntry(newKey, value);
            
            EditorUtility.SetDirty(table);
            EditorUtility.SetDirty(table.SharedData);
            AssetDatabase.SaveAssetIfDirty(table);
            AssetDatabase.SaveAssetIfDirty(table.SharedData);
        }
        public static StringTableEntry ChangeEntry(string id, string text, string tableName = "NTable")
        {
            var table = LocalizationSettings.StringDatabase.GetTable(tableName,
                LocalizationSettings.AvailableLocales.GetLocale(new LocaleIdentifier("en")));
            if (!table)
            {
                return null;
            }

            var entry = table.GetEntry(id);
            if (entry != null)
                entry.Value = text;
            else
                AddEntry(id,text,tableName);
            
            EditorUtility.SetDirty(table);
            EditorUtility.SetDirty(table.SharedData);
            AssetDatabase.SaveAssetIfDirty(table);
            AssetDatabase.SaveAssetIfDirty(table.SharedData);
            return entry;
        }
        public static StringTableEntry AddEntry(string id,string text,string tableName = "NTable")
        {
            var table = LocalizationSettings.StringDatabase.GetTable(tableName,
                LocalizationSettings.AvailableLocales.GetLocale(new LocaleIdentifier("en")));
            if (!table)
            {
                return null;
            }

            var entry = table.GetEntry(id);
            if (entry == null)
                entry = table.AddEntry(id, text);
            else
                entry.Value = text;
            
            EditorUtility.SetDirty(table);
            EditorUtility.SetDirty(table.SharedData);
            AssetDatabase.SaveAssetIfDirty(table);
            AssetDatabase.SaveAssetIfDirty(table.SharedData);
            return entry;
        }



        #endregion

        #region Creation

        public static bool TryCreateEntry(LocalizedString localizedString,
            string defaultValue,
            string identifier = "DATA",
            string tableKey = "NTable",
            bool includeBaseID = true)
        {
            var localString = localizedString;
            if (localString == null)
            {
                return false;
            }
            if (!localString.IsEmpty)
            {
                $"Entry {localString.TableEntryReference.Key} has key! <> {identifier} <> {tableKey}".NLog(Color.red);
                return false;
            }
            var str = new StringBuilder();
            var defaultText = defaultValue;
            str.Append(identifier);
            if (includeBaseID)
            {
                str.Append("/").Append(defaultText);
            }
            var id = str.ToString();
            if (!localString.IsEmpty)
            {
                return false;
            }
            var tableEntry = AddEntry(id, defaultText,tableKey);
            if (tableEntry == null)
            {
                return false;
            }
            localString.SetReference(tableKey,id);
            SetTableDefault(localString, defaultText);
            return true;
        }
        public static bool TryCreateEntry(NLocal_Field_String localField, string identifier = "DATA", string tableKey ="NTable",bool includeBaseID = true)
        {
            return TryCreateEntry(localField.GetLocalizedString(), localField.GetDefaultString(), identifier, tableKey, includeBaseID);
        }
        
        
        public static void SetFieldLocal(this NLocal_Field_String localField, NLocalParams nLocalParams)
        {
            nLocalParams.NLocalField = localField;
            SetFieldLocal(nLocalParams);
        }
        public static void SetFieldLocal(NLocalParams nLocalParams)
        {
            var defaultText = nLocalParams.DefaultValue;
            if (StringHelper.IsNull(defaultText))
                return;
            nLocalParams.NLocalField.SetDefault(nLocalParams.DefaultValue);
            if (nLocalParams.CreateEntry)
            {
                nLocalParams.NLocalField.CreateEntry(nLocalParams.Identifier,
                    nLocalParams.NTable,
                    nLocalParams.IncludeBaseID);
            }
        }
        #endregion
#endif
    }
}