using System.Collections.Generic;
using System.Linq;
using _NueCore.Common.NueLogger;
using UnityEngine;

namespace _NueExtras.TokenSystem._TokenCollection
{
    public class TokenCollectionTab : MonoBehaviour
    {
        [SerializeField] private List<TokenCollectionSlot> slotList = new List<TokenCollectionSlot>();
        [SerializeField] private TokenCollectionCard cardPrefab;
        
        public List<ITokenData> TokenDataList = new List<ITokenData>();

        public List<TokenCollectionSlot> SlotList => slotList;

        public void Build(TokenCategory category,TokenCollectionCatalog catalog)
        {
            TokenDataList =catalog.GetTokenByCategory(category).ToList();
           TokenDataList.Sort((a, b) =>
            {
                // Primary sort: unlocked items first
                var aUnlocked = a.IsTokenUnlocked();
                var bUnlocked = b.IsTokenUnlocked();
                
                // if (aUnlocked != bUnlocked)
                //     return aUnlocked ? -1 : 1;
                
                // Secondary sort: by rarity
                var aRarity = (int)a.GetTokenRarity();
                var bRarity = (int)b.GetTokenRarity();
                
                return aRarity.CompareTo(bRarity); // Higher rarity first
            });
            for (var i = 0; i < TokenDataList.Count; i++)
            {
                if (i>=SlotList.Count)
                {
                    $"Not enough slots for token data: {TokenDataList[i].GetTokenName()} Category: {category.ToString()}".NLog(Color.red);
                    break;
                }
                var tokenData = TokenDataList[i];
                var slot = SlotList[i];
                var clone = Instantiate(cardPrefab,slot.SnapRoot);
                clone.Build(tokenData);
                slot.PlaceCard(clone);
            }
        }

        public void UpdateTab()
        {
            foreach (var slot in SlotList)
            {
                if (slot.IsEmpty)
                    continue;

                slot.UpdateCard();
            }
        }
    }
}