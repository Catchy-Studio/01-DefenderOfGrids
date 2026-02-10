using System;
using System.Collections.Generic;
using _NueCore.Common.NueLogger;
using _NueCore.Common.Utility;
using DG.Tweening;
using NueGames.NTooltip._Keyword;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NueGames.NTooltip
{
    public abstract class TooltipTriggerBase : MonoBehaviour
    {
        [SerializeField] protected NTooltipData customTooltipData;
        [SerializeField] private NTooltipTypes nTooltipType;
        [SerializeField] private NTooltipLayout nTooltipLayout;
        [SerializeField] protected Transform followTarget;
        [SerializeField,TextArea] protected string titleText;
        [SerializeField,TextArea] protected string contentText;
        [SerializeField] private List<NTooltipContent> extraContentList = new List<NTooltipContent>();
        
        #region Cache
        [ShowInInspector,ReadOnly]public Dictionary<string, Func<NTooltipInfo>> ReadyInfoSourceList { get; private set; } = new();
        [ShowInInspector,ReadOnly]public Dictionary<string, Func<NTooltipInfo>> TempInfoSourceList { get; private set; } = new();

        public Transform FollowTarget => followTarget;

        private bool _isShown;
        private bool _isBlocked;

        public Func<NTooltipInfo,NTooltipInfo> OnModifyTooltipInfoFunc { get; set; }
        public Action OnShowAction { get; set; }
        public Action OnHideAction { get; set; }
        
        #endregion

        #region Setup

        private void OnEnable()
        {
           
        }

        private void OnDisable()
        {
            HideTooltipInfo();
        }

        #endregion

        #region Tooltip Data

        public string GetTitleText()
        {
            return titleText;
        }

        public string GetDescText()
        {
            return contentText;
        }
        public void AddTooltipInfo(Func<NTooltipInfo> infoFunc)
        {
            if (infoFunc == null)
                return;
            var info = infoFunc.Invoke();
            var source = info.Source;
            if (StringHelper.IsNull(source))
                return;
            if (ReadyInfoSourceList.ContainsKey(source)) 
                return;
            
            ReadyInfoSourceList.Add(source,infoFunc);
        }
        public void AddTooltipInfo(ITooltipInfo tooltip)
        {
            if (tooltip == null)
                return;
            AddTooltipInfo(() => tooltip.GetTooltipInfo());
        }

        
       
        public void RemoveTooltipInfo(ITooltipInfo tooltip)
        {
            if (tooltip == null)
                return;
            RemoveTooltipInfo(tooltip.GetTooltipInfo());
        }
        
        public void RemoveTooltipInfo(NTooltipInfo info)
        {
            var source = info.Source;
            if (StringHelper.IsNull(source))
                return;
            if (!ReadyInfoSourceList.ContainsKey(source)) 
                return;
            ReadyInfoSourceList.Remove(source);
        }

        public void RemoveTooltipInfo(string source)
        {
            if (!ReadyInfoSourceList.ContainsKey(source)) return;
            ReadyInfoSourceList.Remove(source);
            
        }
        
        public void AddTempTooltipInfo(Func<NTooltipInfo> infoFunc)
        {
            if (infoFunc == null)
                return;
            var info = infoFunc.Invoke();
            var source = info.Source;
            if (StringHelper.IsNull(source))
                return;
            if (TempInfoSourceList.ContainsKey(source)) 
                return;
            
            TempInfoSourceList.Add(source,infoFunc);
        }
        
        public void AddTempTooltipInfo(ITooltipInfo tooltip)
        {
            if (tooltip == null)
                return;
            AddTempTooltipInfo(() => tooltip.GetTooltipInfo());
        }

        public void ClearTempTooltipInfo()
        {
            TempInfoSourceList.Clear();
        }
        #endregion

        #region Tooltip Methods
        public void SetBlock(bool status)
        {
            _isBlocked = status;
            if (_isBlocked)
            {
                HideTooltipInfo();
            }
        }
        public void ChangeTitle(string text)
        {
            titleText = text;
        }
        public void ChangeDescription(string text)
        {
            contentText = text;
        }

        public virtual string GetSource()
        {
            return name;
        }

        public virtual string GetTooltipID()
        {
            if (customTooltipData)
                return customTooltipData.GetID();
            return nTooltipType.ToString();
        }
        
        public virtual NTooltipInfo GetTooltipStruct()
        {
            var tooltipStruct = new NTooltipInfo
            {
                CustomTooltipID = customTooltipData ?customTooltipData.GetID() : "",
                NTooltipType = nTooltipType,
                Source = GetSource(),
                BlockTooltip = _isBlocked,
                FollowTarget = FollowTarget,
                Is3D = false,
                Layout = nTooltipLayout,
                SourceGo = gameObject
            };
            tooltipStruct.SetStringVariable(NTooltipKeys.Title, titleText);
            var ct = contentText;
            //ct = KeywordStatic.ConvertKeywords(ct);
            tooltipStruct.SetStringVariable(NTooltipKeys.Description, ct);
            if (OnModifyTooltipInfoFunc != null)
            {
                tooltipStruct = OnModifyTooltipInfoFunc.Invoke(tooltipStruct);
            }
            return tooltipStruct;
        }

        public void ShowTooltipInfo()
        {
            if (_isBlocked)
                return;
            if (_isShown) return;
            _isShown = true;

            if (extraContentList.Count>0)
            {
                foreach (var ct in extraContentList)
                {
                    if (ct == null)
                        continue;
                    var info = ct.GetTooltipStruct();
                    
                    PrepareTooltip(info);
                }
            }

            if (TempInfoSourceList.Count>0)
            {
                foreach (var kvp in TempInfoSourceList)
                {
                    if (kvp.Value == null)
                        continue;
                    PrepareTooltip(kvp.Value.Invoke());
                }
            }
            
            if (ReadyInfoSourceList.Count>0)
            {
                foreach (var kvp in ReadyInfoSourceList)
                {
                    if (kvp.Value == null)
                        continue;
                    PrepareTooltip(kvp.Value.Invoke());
                }
                return;
            }
            PrepareTooltip(GetTooltipStruct());
        }

        private void PrepareTooltip(NTooltipInfo info)
        {
            if (NTooltipManager.Instance.ShownSourceList.Contains(info.Source)) 
                return;
            if (info.BlockTooltip)
            {
                NTooltipManager.Instance.HideTooltip();
                return;
            }
            NTooltipManager.Instance.ShownSourceList.Add(info.Source + "_" + $"{NTooltipManager.Instance.ShownSourceList.Count}");
            OnShowAction?.Invoke();
            KeywordStatic.SetTooltip(info);
            NTooltipManager.Instance.ShowTooltip(info);
        }

        public void HideTooltipInfo()
        {
            if (!_isShown) return;
            _isShown = false;
            OnHideAction?.Invoke();
            ClearTempTooltipInfo();
            NTooltipManager.Instance.HideTooltip();
            
        }
        #endregion

        public void Refresh()
        {
            if (_isShown)
            {
                HideTooltipInfo();
                DOVirtual.DelayedCall(0.02f, () =>
                {
                    ShowTooltipInfo();
                },false).SetLink(gameObject);
            }
        }
    }
}