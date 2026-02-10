using _NueCore.Common.Utility;
using _NueExtras.Attributes;
using _NueExtras.NLocalizationSystem;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace NueGames.NTooltip._Keyword
{
    [CreateAssetMenu(fileName = "Keyword", menuName = "NueGames/NTooltip/Keyword/Data", order = 0)]
    public class KeywordData : ScriptableObject,ITooltipInfo
    {
        [SerializeField] private string keywordID;
        [SerializeField,RichText] private string keywordName;
        [SerializeField] private NLocal_Field_String localKeywordName;
        [SerializeField,TextArea(5,7)] private string keywordDescription;
        [SerializeField] private NLocal_Field_String localKeywordDescription;
        [SerializeField] private Sprite keywordIcon;
        [SerializeField] private bool ignoreTooltip;
        [SerializeField] private NTooltipTypes tooltipType = NTooltipTypes.RInfo;
        [SerializeField] private NTooltipLayout tooltipLayout = NTooltipLayout.Right;

        #region Cache
        public bool IgnoreTooltip => ignoreTooltip;

        

        #endregion

        #region Methods

        public virtual NTooltipTypes GetTooltipType()
        {
            return tooltipType;
        }

        public virtual NTooltipLayout GetTooltipLayout()
        {
            return tooltipLayout;
        }
        public virtual string GetKeywordID()
        {
            return keywordID;
        }
        
        public virtual string GetKeywordName()
        {
            return keywordName;
        }
        
        public virtual string GetKeywordDescription()
        {
            return keywordDescription;
        }
        
        public virtual Sprite GetKeywordIcon()
        {
            return keywordIcon;
        }

        public NTooltipInfo GetTooltipInfo(Transform followRoot = null)
        {
            var info = new NTooltipInfo
            {
                NTooltipType = GetTooltipType(),
                Layout = GetTooltipLayout(),
                FollowTarget = followRoot
            };
            
            info.SetStringVariable(NTooltipKeys.Title,GetKeywordName());
            var desc = GetKeywordDescription();
            if (!StringHelper.IsNull(desc))
                info.SetStringVariable(NTooltipKeys.Description,GetKeywordDescription());
            var icon = GetKeywordIcon();
            if (icon)
                info.SetSpriteVariable(NTooltipKeys.Icon,icon);
            return info;
        }

        #endregion

        #region Editor

#if UNITY_EDITOR
        [Button]
        private void SetDefaultLocals(bool apply)
        {
            if (!apply)
            {
                return;
            }

            localKeywordName.SetFieldLocal(new NLocalParams(
                localKeywordName, keywordName, $"DATA/KW/Title/{GetKeywordID()}", NTables.KWTable, false, true));
            localKeywordDescription.SetFieldLocal(new NLocalParams(
                localKeywordDescription,keywordDescription,$"DATA/KW/Desc/{GetKeywordID()}",NTables.KWTable,false,true));
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
        }
#endif


        #endregion

    }
}