using __Project.Systems.ChoiceSystem._Cards;
using UnityEngine;

namespace __Project.Systems.ChoiceSystem._Slots
{
    public class ChoiceSlotDirect : MonoBehaviour
    {
        [SerializeField] private Transform snapRoot;
        [SerializeField] private RectTransform rect;
        public ChoiceCardDirect PlacedCard { get; private set; }
        public bool IsEmpty => !PlacedCard;
        public Transform SnapRoot => snapRoot;

        public RectTransform Rect => rect;

        public void Place(ChoiceCardDirect card)
        {
            PlacedCard = card;
            card.transform.SetParent(SnapRoot);
            Snap(card);
            card.PlaceToSlot(this);
        }
       
        public void TempPlace(ChoiceCardDirect card)
        {
            card.transform.SetParent(SnapRoot);
            Snap(card);
        }

        public void Snap(ChoiceCardDirect card)
        {
            card.transform.localPosition = Vector3.zero;
            card.transform.localRotation = Quaternion.identity;
            card.transform.localScale = Vector3.one;
            card.Rect.offsetMax = Vector2.zero;
            card.Rect.offsetMin = Vector2.zero;
        }
        public void Clear()
        {
            PlacedCard = null;
        }
    }
}