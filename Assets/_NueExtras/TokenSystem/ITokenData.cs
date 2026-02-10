using _NueExtras.RaritySystem;
using _NueExtras.TokenSystem._TokenCollection;
using NueGames.NTooltip;
using UnityEngine;

namespace _NueExtras.TokenSystem
{
    public interface ITokenData
    {
        public string GetTokenID();
        public string GetTokenName();
        public int GetRequiredTokenCount();
        public string GetTokenDescription();
        public Sprite GetTokenIcon();
        public NRarity GetTokenRarity();
        public TokenCategory GetTokenCategory();
        public NTooltipInfo GetTokenTooltipInfo();
        public bool IsTokenUnlockedByDefault();
        public bool IsTokenUnlocked();
        public bool IsTokenBlocked();
    }
}