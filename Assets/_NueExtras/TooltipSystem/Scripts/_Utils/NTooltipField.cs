using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NueGames.NTooltip
{
    [Serializable]
    public class NTooltipField<T>
    {
        [SerializeField] private string key;
        [SerializeField] private T field;

        public virtual void Init()
        {
        }
        public string Key => key;

        public T Field => field;
    }
    
    [Serializable]
    public class NTooltipField_TMP : NTooltipField<TMP_Text>
    {
        public string BaseText { get; private set; }
        public override void Init()
        {
            BaseText = Field.text;
            base.Init();
        }
    }
    
    [Serializable]
    public class NTooltipField_Image : NTooltipField<Image>
    {
        public Sprite BaseSprite { get; private set; }
        public override void Init()
        {
            BaseSprite = Field.sprite;
            base.Init();
        }
    }
}