using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NueGames.NTooltip
{
    public abstract class NTooltip : MonoBehaviour
    {
        [SerializeField] protected List<NTooltipField_TMP> tmpFieldList = new List<NTooltipField_TMP>();
        [SerializeField] protected List<NTooltipField_Image> imageFieldList = new List<NTooltipField_Image>();
        [SerializeField] protected LayoutElement layoutElement;
        
        #region Cache
        public NTooltipData Data { get; private set; }
        private float _cachedPreferredWidth = -1;
        private float _cachedPreferredHeight = -1;
        public string CalledID { get; private set; }
        public string CalledLastSource { get; private set; }
        public GameObject SourceGO { get; private set; }
        #endregion
        
        #region Setup
        public virtual void Init(NTooltipData tooltipData)
        {
            Data = tooltipData;
            _cachedPreferredWidth = layoutElement.preferredWidth;
            _cachedPreferredHeight = layoutElement.preferredHeight;
            foreach (var field in tmpFieldList)
                field.Init();
        }
        
        public virtual void Prepare(NTooltipInfo nTooltipInfo)
        {
            CalledLastSource = nTooltipInfo.Source;
            CalledID = nTooltipInfo.GetID();
            SourceGO = nTooltipInfo.SourceGo;

           
            if (nTooltipInfo.PreferredWidth>0)
                layoutElement.preferredWidth = nTooltipInfo.PreferredWidth;
            else
                layoutElement.preferredWidth = _cachedPreferredWidth;
            
           
            if (nTooltipInfo.PreferredHeight>0)
                layoutElement.preferredHeight = nTooltipInfo.PreferredHeight;
            else
                layoutElement.preferredHeight = _cachedPreferredHeight;
           
            foreach (var tmp in tmpFieldList)
            {
                tmp.Field.SetText(tmp.BaseText);
            }
            
            foreach (var image in imageFieldList)
            {
                image.Field.sprite = image.BaseSprite;
            }
            
          
        }
        
        public virtual void Show(NTooltipInfo nTooltipInfo)
        {
            var titleKey = NTooltipKeys.Title;
            var titleField = GetField_TMP(titleKey);
            if (titleField != null)
            {
                var titleText = nTooltipInfo.GetStringVariable(titleKey);
                titleField.Field.SetText(titleText);
            }

            var descKey = NTooltipKeys.Description;
            var contentField = GetField_TMP(descKey);
            if (contentField != null)
            {
                var contentText = nTooltipInfo.GetStringVariable(descKey);
                contentField.Field.SetText(contentText);
            }

            foreach (var kvp in nTooltipInfo.StringVariables)
            {
                if (kvp.Key == titleKey || kvp.Key == descKey)
                    continue;
                var tt = GetField_TMP(kvp.Key);
                if (tt != null)
                {
                    var contentText = nTooltipInfo.GetStringVariable(kvp.Key);
                    tt.Field.SetText(contentText);
                }
            }

            nTooltipInfo.SetTransforms(tmpFieldList);
            AdjustLayoutElementBasedOnTextContent_Height();
        }
        

        private void AdjustLayoutElementBasedOnTextContent_Width()
    {
        float maxWidth = 0;
        foreach (var tmp in tmpFieldList)
        {
            if (tmp.Field == null) continue;
            float textWidth = tmp.Field.preferredWidth;
            maxWidth = Mathf.Max(maxWidth, textWidth);
        }

        var calculatedMaxWidth = Mathf.FloorToInt(maxWidth / layoutElement.preferredWidth);
        if (calculatedMaxWidth>0)
        {
            layoutElement.preferredWidth = _cachedPreferredWidth + (calculatedMaxWidth * _cachedPreferredWidth / 2);
        }
        else
        {
            layoutElement.preferredWidth = _cachedPreferredWidth;
        }
    }

        private void AdjustLayoutElementBasedOnTextContent_Height()
    {
        float totalHeight = 0;
        foreach (var tmp in tmpFieldList)
        {
            if (tmp.Field == null) continue;
            totalHeight += tmp.Field.preferredHeight;
        }

        if (totalHeight > layoutElement.preferredHeight)
        {
            layoutElement.preferredHeight = totalHeight + 20f; // Adding padding
        }
        else
        {
            layoutElement.preferredHeight = _cachedPreferredHeight;
        }

        var doubleBase = _cachedPreferredHeight * 3f;
        if (layoutElement.preferredHeight>doubleBase)
        {
            var td = layoutElement.preferredHeight;
            var diff = td - doubleBase;
            layoutElement.preferredHeight = doubleBase;
            layoutElement.preferredWidth = _cachedPreferredWidth + (diff*0.5f);
        }
        else
        {
            layoutElement.preferredWidth = _cachedPreferredWidth;
        }
    }

        #endregion

        #region Methods
        public NTooltipField_TMP GetField_TMP(string key)
        {
            return tmpFieldList.Find(x => x.Key == key);
        }

        public NTooltipField_Image GetField_Image(string key)
        {
            return imageFieldList.Find(x => x.Key == key);
        }
        
        public List<NTooltipField_Image> GetPartialFields_Image(string part)
        {
            return imageFieldList.FindAll(x =>
            {
                if (!x.Key.Contains(part))
                {
                    return false;
                }

                return true;
            });
        }
        #endregion
        

#if UNITY_EDITOR
        [SerializeField] private bool ignoreAutoNaming;
        private void OnValidate()
        {
            if (ignoreAutoNaming)
                return;
            if (Application.isPlaying)
                return;
            return;
            foreach (var tmp in tmpFieldList)
            {
                if (tmp.Field)
                    tmp.Field.SetText(tmp.Key);
            }
        }
#endif
    }
}
