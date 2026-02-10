using System;
using _NueCore.Common.KeyValueDict;
using _NueExtras.RaritySystem;
using UnityEngine;

namespace _NueExtras.TokenSystem._TokenCollection
{
    public class TokenCollectionSlot : MonoBehaviour
    {
        [SerializeField] private Transform snapRoot;
        [SerializeField] private KeyValueDict<NRarity,Transform> rarityRootDict = new KeyValueDict<NRarity, Transform>();
        [SerializeField] private Transform lockRoot;
        [SerializeField] private Transform willBeComingRoot;
        
        public bool IsEmpty => !PlacedCard;
        public TokenCollectionCard PlacedCard { get; private set; }

        public Transform SnapRoot => snapRoot;

        private void Awake()
        {
            lockRoot.gameObject.SetActive(false);
            if (willBeComingRoot)
                willBeComingRoot.gameObject.SetActive(true);
        }

        public void PlaceCard(TokenCollectionCard card)
        {
            if (card == null)
            {
                return;
            }
            if (willBeComingRoot)
                willBeComingRoot.gameObject.SetActive(false);
            PlacedCard = card;
            card.PlaceToSlot(this);
            card.transform.SetParent(SnapRoot);
            card.transform.localPosition = Vector3.zero;
            card.transform.localRotation = Quaternion.identity;
            card.transform.localScale = Vector3.one;
            
            var rarity =card.TokenData.GetTokenRarity();
            
            foreach (var kvp in rarityRootDict)
                kvp.Value.gameObject.SetActive(false);
            if (rarityRootDict.TryGetValue(rarity,out var rarityRoot))
                rarityRoot.gameObject.SetActive(true);
        }

        public void UpdateCard()
        {
            if (PlacedCard)
            {
                PlacedCard.UpdateCard();
                lockRoot.gameObject.SetActive(!PlacedCard.TokenData.IsTokenUnlocked());
            }
            else
            {
                lockRoot.gameObject.SetActive(false);
            }
        }
    }
}