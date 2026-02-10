using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace __Project.Systems.ChoiceSystem._Cards
{
    public class ChoiceCardNodePurchased : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text levelTextField;
        

        public IChoiceItem Data { get; private set; }
        public void Build(IChoiceItem choiceData)
        {
            if (Data != null)
                return;
          
            Data = choiceData;
            // var level = choiceData.GetChoiceCount();
            iconImage.sprite = choiceData.GetChoiceSprite();
            //levelTextField.SetText("Lv." + level);
        }
    }
}