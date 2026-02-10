using System.Collections.Generic;
using __Project.Systems.ChoiceSystem._Cards;
using __Project.Systems.ChoiceSystem._Selection;
using __Project.Systems.ChoiceSystem._Slots;
using _NueCore.AudioSystem;
using _NueCore.Common.Extensions;
using _NueCore.Common.NueLogger;
using _NueExtras.PopupSystem.PopupDataSub;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace __Project.Systems.ChoiceSystem._Display
{
    public class DisplayChoice3 : MonoBehaviour
    {
        [SerializeField] private ChoiceCardDirect cardPrefab;
        [SerializeField] private List<ChoiceSlotDirect> slotList;
        [SerializeField] private TMP_Text titleTextField;
        [SerializeField] private Button skipButton;
        
        #region Cache
        public ChoiceSelectionData ChoiceData { get; private set; }
        public static List<string> ShownChoiceIdList { get; private set; } = new List<string>();
        public static List<string> ShownGroupIdList { get; private set; } = new List<string>();
        private List<ChoiceCardDirect> SpawnedCardList { get; set; } = new List<ChoiceCardDirect>();
        #endregion

        #region Setup
        public void Build(ChoiceSelectionData choiceData, PopupDisplay popupDisplay)
        {
            if (ChoiceData)
                return;
            ChoiceData = choiceData;
            ShownChoiceIdList.Clear();
            ShownGroupIdList.Clear();
            skipButton.onClick.AddListener((() =>
            {
                AudioStatic.PlayFx(DefaultAudioDataTypes.Click);
                popupDisplay.ClosePopup();
            }));
            
            foreach (var optionSlot in slotList)
            {
                optionSlot.gameObject.SetActive(false);
            }
            
            ChoiceSelectionOption.AvailableChoiceList.Clear();

            popupDisplay.PopupClosedAction += () =>
            {
                ShownChoiceIdList.Clear();
                ShownGroupIdList.Clear();
            };

            var choiceList = ChoiceData.OptionList.RandomItem().GetRandomChoiceDataList(3);
            for (var i = 0; i < choiceList.Count; i++)
            {
                var data = choiceList[i];
                if (data == null)
                    continue;
                
                var slot = slotList[SpawnedCardList.Count % slotList.Count];
                slot.gameObject.SetActive(true);
                var card = Instantiate(cardPrefab, slot.SnapRoot);
                slot.Place(card);
                card.Build(data, () =>
                {
                    popupDisplay.ClosePopup();
                });
                SpawnedCardList.Add(card);
                ShownChoiceIdList.Add(data.GetChoiceUniqueID());
                ShownGroupIdList.Add(data.GetChoiceGroupID());
            }
            
            titleTextField.SetText(ChoiceData.Title);
        }
        #endregion
       
    }
}