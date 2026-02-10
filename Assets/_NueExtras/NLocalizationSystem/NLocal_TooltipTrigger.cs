using NueGames.NTooltip;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Localization;

namespace _NueExtras.NLocalizationSystem
{
    [RequireComponent(typeof(TooltipTriggerBase))]
    public class NLocal_TooltipTrigger : MonoBehaviour
    {
        [SerializeField] private LocalizedString localTitle;
        [SerializeField] private LocalizedString localDesc;
        
        [SerializeField,ReadOnly] private TooltipTriggerBase tooltipTrigger;

        #region Cache
        public TooltipTriggerBase TooltipTrigger
        {
            get
            {
                if (tooltipTrigger == null)
                    tooltipTrigger = GetComponent<TooltipTriggerBase>();
                return tooltipTrigger;
            }
        }

        private TooltipTriggerBase _tooltipTrigger;
        #endregion

        #region Setup
        private void OnEnable()
        {
            TooltipTrigger.OnModifyTooltipInfoFunc += OnModifyTooltipInfoFunc;
        }

        private void OnDisable()
        {
            TooltipTrigger.OnModifyTooltipInfoFunc -= OnModifyTooltipInfoFunc;
        }
        #endregion

        #region Methods
        private NTooltipInfo OnModifyTooltipInfoFunc(NTooltipInfo info)
        {
            if (localTitle is { IsEmpty: false })
            {
                info.SetStringVariable(NTooltipKeys.Title, localTitle.GetLocalizedString());
            }

            if (localDesc is { IsEmpty: false })
            {
                info.SetStringVariable(NTooltipKeys.Description, localDesc.GetLocalizedString());
            }
            return info;
        }
        #endregion
        
        #region Editor
#if UNITY_EDITOR
        [Button]
        private void SetDefault()
        {
            if (TooltipTrigger == null)
                return;
            NLocalStatic.SetTableDefault(localTitle,TooltipTrigger.GetTitleText());
            NLocalStatic.SetTableDefault(localDesc,TooltipTrigger.GetDescText());
        }
#endif
        #endregion

    }
}