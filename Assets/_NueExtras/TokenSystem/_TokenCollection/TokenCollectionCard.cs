using NueGames.NTooltip;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _NueExtras.TokenSystem._TokenCollection
{
    public class TokenCollectionCard : MonoBehaviour
    {
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text descText;
        [SerializeField] private Image iconImage;
        [SerializeField] private TooltipTrigger_UI tooltipTrigger;
        [SerializeField] private Transform tooltipRoot;
        
        public ITokenData TokenData { get; private set; }
        public TokenCollectionSlot Slot { get; private set; }
        public void Build(ITokenData tokenData)
        {
            TokenData = tokenData;
            titleText.SetText(tokenData.GetTokenName());
            if (descText)
            {
                descText.SetText(tokenData.GetTokenDescription());
            }
            iconImage.sprite = tokenData.GetTokenIcon();
            UpdateCard();

            SetTooltip(tokenData);
        }

        public void PlaceToSlot(TokenCollectionSlot slot)
        {
            Slot = slot;
        }

        public void UpdateCard()
        {
            var isUnlocked = TokenData.IsTokenUnlocked();
            if (!isUnlocked)
            {
                // titleText.color = Color.gray;
                // descText.color = Color.gray;
                iconImage.color = Color.gray;
            }
            else
            {
                // titleText.color = Color.white;
                // descText.color = Color.white;
                iconImage.color = Color.white;
            }
        }

        private void SetTooltip(ITokenData tokenData)
        {
            if (tooltipTrigger)
            {
                tooltipTrigger.AddTooltipInfo((() =>
                {
                    if (tokenData.IsTokenUnlocked())
                    {
                        var tokenInfo =tokenData.GetTokenTooltipInfo();
                        tokenInfo.Source = gameObject.name;
                        tokenInfo.SourceGo = gameObject;
                        tokenInfo.FollowTarget = tooltipRoot;
                        return tokenInfo;
                    }

                    var lockInfo = new NTooltipInfo
                    {
                        NTooltipType = NTooltipTypes.Default,
                        Layout = NTooltipLayout.Default,
                        Source = gameObject.name,
                        SourceGo = gameObject,
                        Is3D = false,
                        FollowTarget = tooltipRoot
                    };
                    lockInfo.SetStringVariable(NTooltipKeys.Title, tokenData.GetTokenName());
                    lockInfo.SetStringVariable(NTooltipKeys.Description, "LOCKED");
                    return lockInfo;
                }));
            }
        }
    }
}