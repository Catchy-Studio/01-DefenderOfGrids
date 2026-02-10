using System.Collections.Generic;
using __Project.Systems.ChoiceSystem._Cards;
using __Project.Systems.ChoiceSystem._Selection;
using __Project.Systems.ChoiceSystem._Slots;
using _NueExtras.PopupSystem.PopupDataSub;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace __Project.Systems.ChoiceSystem._Display
{
    public class DisplayDetailedChoice : MonoBehaviour
    {
        [SerializeField] private List<ChoiceSlotNodePurchased> purchasedSlotList;
        [SerializeField] private ChoiceCardNodePurchased prefab;
        [SerializeField] private ChoiceCardNode cardNodePrefab;
        [SerializeField] private List<ChoiceSlotNode> nodeSlotList;
        [SerializeField] private TMP_Text titleTextField;
        [SerializeField] private TMP_Text descTextField;
        [SerializeField] private Button confirmButton;
        

        public ChoiceSelectionData ChoiceData { get; private set; }
        public static List<string> ShownChoiceIdList { get; private set; } = new List<string>();
        public static List<string> ShownGroupIdList { get; private set; } = new List<string>();

        public List<ChoiceSlotNodePurchased> PurchasedSlotList => purchasedSlotList;
        private List<ChoiceCardNodePurchased> SpawnedPurchasedCardList { get; set; } = new List<ChoiceCardNodePurchased>();
        private List<ChoiceCardNode> SpawnedNodeCardList { get; set; } = new List<ChoiceCardNode>();
        public ChoiceSlotNode Selected { get; private set; }
        public void Build(ChoiceSelectionData choiceData, PopupDisplay popupDisplay, bool isBonus)
        {
            if (ChoiceData)
                return;
            ChoiceData = choiceData;
            ShownChoiceIdList.Clear();
            ShownGroupIdList.Clear();

            confirmButton.onClick.AddListener(() =>
            {
                if (Selected)
                {
                    Selected.Card.Choose();
                }
            });
            
            foreach (var optionSlot in nodeSlotList)
            {
                optionSlot.SelectAction += () =>
                {
                    foreach (var slot in nodeSlotList)
                    {
                        if (slot != optionSlot)
                            slot.DeSelect();
                    }

                    Selected = optionSlot;
                    UpdateMainDisplay();
                };
                optionSlot.gameObject.SetActive(false);
                
            }
            
            ChoiceSelectionOption.AvailableChoiceList.Clear();

            popupDisplay.PopupClosedAction += () =>
            {
                ShownChoiceIdList.Clear();
                ShownGroupIdList.Clear();
            };

            for (var i = 0; i < ChoiceData.OptionList.Count; i++)
            {
                var option = ChoiceData.OptionList[i];
                var data = option.GetRandomChoiceData();
                if (data == null)
                    continue;
                
                var slot = nodeSlotList[SpawnedPurchasedCardList.Count % nodeSlotList.Count];
                slot.gameObject.SetActive(true);
                var card = Instantiate(cardNodePrefab, slot.SpawnRoot);
                card.Build(data,slot);
                SpawnedNodeCardList.Add(card);
                ShownChoiceIdList.Add(data.GetChoiceUniqueID());
                ShownGroupIdList.Add(data.GetChoiceGroupID());
            }
            
            nodeSlotList[0].Select();

            var index = 0;
            // foreach (var kvp in RunStatic.ActiveRunner.AppliedModsDict)
            // {
            //     var sp = SpawnedPurchasedCardList.Find(x => x.Data.GetChoiceGroupID() == kvp.Key.GetChoiceGroupID());
            //     if (sp)
            //         continue;
            //
            //     var slot = PurchasedSlotList[index];
            //     var clone = Instantiate(choiceCardPrefab, slot.SnapRoot);
            //     clone.Build(kvp.Key);
            //     SpawnedPurchasedCardList.Add(clone);
            //     index++;
            // }

        }
        
        public void UpdateMainDisplay()
        {
            if (!Selected)
                return;
            
            titleTextField.SetText(Selected.Card.Data.GetChoiceTitle());
            descTextField.SetText(Selected.Card.Data.GetChoiceDesc());
        }
    }
}