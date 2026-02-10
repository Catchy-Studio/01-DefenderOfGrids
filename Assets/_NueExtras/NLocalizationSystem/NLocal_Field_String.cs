using System;
using _NueCore.Common.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Localization;

namespace _NueExtras.NLocalizationSystem
{
    [Serializable]
    public class NLocal_Field_String
    {
        [SerializeField] private LocalizedString localString;
        [TextArea(3,5),SerializeField]private string defaultString;

        public void SetDefault(string value)
        {
            defaultString = value;
        }
        public string GetDefaultString()
        {
            return defaultString;
        }
        public string GetString()
        {
            if (localString.IsEmpty)
            {
                return defaultString;
            }
            return localString.GetLocalizedString();
        }

        public LocalizedString GetLocalizedString()
        {
            return localString;
        }

        #region Editor

#if UNITY_EDITOR
        [Button]
        public void CreateEntry(string identifier,NTables nTable = NTables.NTable, bool includeBaseID = true)
        {
            if (StringHelper.IsNull(identifier))
            {
                return;
            }
            
            if (NLocalStatic.TryCreateEntry(this,identifier,nTable.GetTableName(),includeBaseID))
            {
                
            }
        }
#endif
       

        #endregion

    }
}