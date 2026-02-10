using System;
using System.Collections.Generic;
using UnityEngine;

namespace NueGames.NTooltip
{
    [Serializable]
    public class NTooltipContent : ITooltipInfo
    {
        public NTooltipData customTooltipData;
        public NTooltipTypes nTooltipType;
        public NTooltipLayout layoutType;
        public Transform followTarget;
        public List<TooltipText> tooltipTextList = new List<TooltipText>();
        
        [Serializable]
        public class TooltipText
        {
            public string Key;
            [TextArea(5,10)]public string Text;
        }

        public virtual NTooltipInfo GetTooltipStruct()
        {
            var tooltipStruct = new NTooltipInfo
            {
                CustomTooltipID = customTooltipData ?customTooltipData.GetID() : "",
                NTooltipType = nTooltipType,
                FollowTarget = followTarget,
                Is3D = false,
                Layout = layoutType
            };
            foreach (var text in tooltipTextList)
            {
                if (string.IsNullOrEmpty(text.Key) || string.IsNullOrEmpty(text.Text))
                    continue;
                tooltipStruct.SetStringVariable(text.Key, text.Text);
            }
            return tooltipStruct;
        }

        public NTooltipInfo GetTooltipInfo(Transform followRoot = null)
        {
            if (followRoot != null)
            {
                followTarget = followRoot;
            }
            return GetTooltipStruct();
        }
    }
}