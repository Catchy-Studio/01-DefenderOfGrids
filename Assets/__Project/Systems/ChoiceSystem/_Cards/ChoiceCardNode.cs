using __Project.Systems.ChoiceSystem._Display;
using __Project.Systems.ChoiceSystem._Slots;
using _NueCore.Common.ReactiveUtils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace __Project.Systems.ChoiceSystem._Cards
{
    public class ChoiceCardNode : MonoBehaviour
    {
        [SerializeField] private Image optionImage;
        [SerializeField] private TMP_Text optionLevelText;
        [SerializeField] private Transform activeRoot;
        
        public IChoiceItem Data { get; private set; }
        public ChoiceSlotNode Slot { get; private set; }
        public void Build(IChoiceItem choiceData,ChoiceSlotNode slot)
        {
            if (Data != null)
                return;
            
            Data = choiceData;
            Slot = slot;
            optionImage.sprite = choiceData.GetChoiceSprite();
            // var level = choiceData.GetChoiceCount();
            // if (level<=0)
            // {
            //     optionLevelText.SetText("Unlock");
            // }
            // else
            // {
            //     optionLevelText.SetText("Lvl "+ (level));
            // }
            slot.Build(this);
        }

        public void Select()
        {
            activeRoot.gameObject.SetActive(true);
        }

        public void DeSelect()
        {
            activeRoot.gameObject.SetActive(false);
        }

        public void Choose()
        {
            Data.OnChoose();
            RBuss.Publish(new ChoiceREvents.ChoiceChosenREvent(Data));
        }
    }
}