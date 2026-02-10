using System;
using __Project.Systems.ChoiceSystem._Slots;
using _NueCore.AudioSystem;
using _NueCore.Common.KeyValueDict;
using _NueCore.Common.ReactiveUtils;
using _NueExtras.RaritySystem;
using NueGames.NTooltip;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace __Project.Systems.ChoiceSystem._Cards
{
    public class ChoiceCardDirect : MonoBehaviour,IPointerClickHandler,ITooltipInfo
    {
        [SerializeField] private RectTransform rect;
        
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text nameTextField;
        [SerializeField] private Transform newRoot;
        [SerializeField] private TooltipTrigger_UI tooltip;
        [SerializeField] private KeyValueDict<NRarity,Transform> rarityDict;
        
        public IChoiceItem Data { get; private set; }
        public ChoiceSlotDirect Slot { get; private set; }
        public RectTransform Rect => rect;

        public Action onChooseAction;
        private bool _canChoose;
        public void Build(IChoiceItem choiceItem,Action chooseAction = null)
        {
            if (Data != null)
                return;
            _canChoose = false;
            foreach (var keyValue in rarityDict)
            {
                keyValue.Value.gameObject.SetActive(false);
            }

            var rarity = choiceItem.GetRarity();
            if (rarityDict.TryGetValue(rarity, out Transform t))
            {
                t.gameObject.SetActive(true);
            }
            onChooseAction += chooseAction;
            DOVirtual.DelayedCall(1.25f, () =>
            {
                _canChoose = true;
            },true).SetLink(gameObject);
            
            Data = choiceItem;
            Data.BuildChoice(gameObject);
            iconImage.sprite = choiceItem.GetChoiceSprite();
          
            newRoot.gameObject.SetActive(false);
            
            var title = choiceItem.GetChoiceTitle();
            title = choiceItem.ConvertText(title);
            nameTextField.SetText(title);
            tooltip.AddTooltipInfo(this);
            
        }
        public void Choose()
        {
            AudioStatic.PlayFx(DefaultAudioDataTypes.Purchase);
            Data.OnChoose();
            RBuss.Publish(new ChoiceREvents.ChoiceChosenREvent(Data));
            onChooseAction?.Invoke();
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_canChoose)
                return;
            Choose();
        }

        public void PlaceToSlot(ChoiceSlotDirect choiceSlotDirect)
        {
            Slot = choiceSlotDirect;
        }

        public string GetID()
        {
            return Data.GetChoiceUniqueID();
        }

        public NTooltipInfo GetTooltipInfo(Transform followRoot = null)
        {
            var context = Data.GetChoiceDesc();
            context =Data.ConvertText(context);
            var title = Data.GetChoiceTitle();
            title = Data.ConvertText(title);
            
            
            var info = new NTooltipInfo
            {
                Source = GetID(),
                NTooltipType = NTooltipTypes.Product
            };
            info.SetStringVariable(NTooltipKeys.Title, title);
            info.SetStringVariable(NTooltipKeys.Description, context);
            return info;
        }
    }
}