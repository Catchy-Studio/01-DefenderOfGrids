using __Project.Systems.ChoiceSystem._Cards;
using UnityEngine;

namespace __Project.Systems.ChoiceSystem._Slots
{
    public class ChoiceSlotNodePurchased : MonoBehaviour
    {
        [SerializeField] private Transform snapRoot;

        public Transform SnapRoot => snapRoot;
        
        public ChoiceCardNodePurchased Card { get; private set; }

        public void Build(ChoiceCardNodePurchased choiceCard)
        {
            if (Card)
                return;

            Card = choiceCard;
        }
    }
}