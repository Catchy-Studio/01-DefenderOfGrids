using NueGames.NTooltip;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _NueExtras.AchievementSystem
{
    public class AchCollectionCard : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameTextField;
        [SerializeField] private Image iconImage;
        [SerializeField] private TooltipTrigger_UI tooltipTrigger;

        public AchievementData Data { get; private set; }
        private bool IsAchieved { get; set; }
        public void Build(AchievementData data)
        {
            if (Data)
            {
                return;
            }
            Data = data;
            nameTextField.SetText(data.Title);
            iconImage.sprite = data.Icon;
            IsAchieved =AchStatic.IsAchievementAchieved(Data.ID);
            iconImage.color = IsAchieved ? Color.white : Color.grey;
            tooltipTrigger.AddTooltipInfo(Data);
        }
        
    }
}