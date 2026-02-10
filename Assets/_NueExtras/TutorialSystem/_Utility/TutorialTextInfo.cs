using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _NueExtras.TutorialSystem._Utility
{
    [Serializable]
    public class TutorialTextInfo
    {
        [TextArea(5,10)]public string Text;
        [TextArea(3,5)]public string SpeakerName;
        [PreviewField]public Sprite SpeakerSprite;
    }
}