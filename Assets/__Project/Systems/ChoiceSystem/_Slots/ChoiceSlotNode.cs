using System;
using __Project.Systems.ChoiceSystem._Cards;
using UnityEngine;
using UnityEngine.EventSystems;

namespace __Project.Systems.ChoiceSystem._Slots
{
    public class ChoiceSlotNode : MonoBehaviour,IPointerClickHandler
    {
        [SerializeField] private Transform spawnRoot;

        public Transform SpawnRoot => spawnRoot;
        public ChoiceCardNode Card { get; private set; }
        public Action SelectAction { get;  set; }
        public Action DeSelectAction { get;  set; }
        
        public bool IsSelected { get; private set; }
        public void Build(ChoiceCardNode card)
        {
            if (Card)
                return;

            Card = card;
            IsSelected = false;
            Card.DeSelect();
            DeSelectAction?.Invoke();
        }

        public void Select()
        {
            if (IsSelected)
                return;

            IsSelected = true;
            Card.Select();
            SelectAction?.Invoke();
        }
        
        public void DeSelect()
        {
            if (!IsSelected)
                return;

            IsSelected = false;
            Card.DeSelect();
            DeSelectAction?.Invoke();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (IsSelected)
            {
                return;
            }
            Select();
        }
    }
}