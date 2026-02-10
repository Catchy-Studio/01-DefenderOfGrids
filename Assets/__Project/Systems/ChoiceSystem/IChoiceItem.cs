using _NueExtras.RaritySystem;
using UnityEngine;

namespace __Project.Systems.ChoiceSystem
{
    public interface IChoiceItem
    {
        //Info
        public string GetChoiceUniqueID();
        public string GetChoiceGroupID();
        //Visuals
        public string GetChoiceTitle();
        public string GetChoiceDesc();
        public Sprite GetChoiceSprite();
        
        //Actions
        public void BuildChoice(GameObject root);
        public bool IsAvailableToChoose();
        public void OnChoose();
        public string ConvertText(string context);
        public NRarity GetRarity();
    }
}