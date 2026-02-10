using System.Collections.Generic;
using __Project.Systems.ChoiceSystem._Cards;
using __Project.Systems.ChoiceSystem._Selection;
using __Project.Systems.ChoiceSystem._Slots;
using _NueExtras.PopupSystem.PopupDataSub;
using UnityEngine;

namespace __Project.Systems.ChoiceSystem._Display
{
    public class DisplayDefaultChoice : MonoBehaviour
    {
        [SerializeField] private List<ChoiceCardDirect> optionCardList;
        [SerializeField] private List<ChoiceSlotNodePurchased> slotList;
        [SerializeField] private ChoiceCardNodePurchased prefab;
        

        public ChoiceSelectionData ChoiceData { get; private set; }
        public static List<string> ShownChoiceIdList { get; private set; } = new List<string>();
        public static List<string> ShownGroupIdList { get; private set; } = new List<string>();

        public List<ChoiceSlotNodePurchased> SlotList => slotList;
        private List<ChoiceCardNodePurchased> SpawnedCardList { get; set; } = new List<ChoiceCardNodePurchased>();

        public void Build(ChoiceSelectionData choiceData, PopupDisplay popupDisplay,bool isBonus)
        {
            if (ChoiceData)
                return;
            ChoiceData = choiceData;
            ShownChoiceIdList.Clear();
            ShownGroupIdList.Clear();
            
            ChoiceSelectionOption.AvailableChoiceList.Clear();
            
            popupDisplay.PopupClosedAction += () =>
            {
                ShownChoiceIdList.Clear();
                ShownGroupIdList.Clear();
            };
          
            foreach (var card in optionCardList)
            {
                card.gameObject.SetActive(false);
            }
            for (var i = 0; i < 3; i++)
            {
                var option = ChoiceData.OptionList[0];
                var data = option.GetRandomChoiceData();
              
                if (data == null)
                    continue;
               
                if (i>=optionCardList.Count)
                    break;
                var card = optionCardList[i];
                card.gameObject.SetActive(true);
                card.Build(data);
                ShownChoiceIdList.Add(data.GetChoiceUniqueID());
                ShownGroupIdList.Add(data.GetChoiceGroupID());
            }
        }
    }
}