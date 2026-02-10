using System;
using System.Collections.Generic;
using System.Linq;
using _NueCore.AudioSystem;
using _NueCore.Common.Extensions;
using _NueCore.Common.NueLogger;
using _NueCore.Common.ReactiveUtils;
using _NueCore.SaveSystem;
using _NueExtras.TokenSystem._TokenCollection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _NueExtras.TokenSystem
{
    public static class TokenStatic
    {
        public static TokenCollectionCatalog Catalog { get; private set; }
        public static void SetTokenCollectionCatalog(TokenCollectionCatalog catalog)
        {
            Catalog = catalog;
        }
        public static List<int> ThresholdList { get; private set; } = new List<int>();

        public static ITokenData UnlockRandomTokenData()
        {
            var availableTokens = Catalog.GetAvailableTokens();
            if (availableTokens.Count<=0)
            {
                Debug.Log("No available tokens to unlock.");
                return null;
            }
            AudioStatic.PlayFx(DefaultAudioDataTypes.Claim);

            var data = availableTokens.RandomItem();
            TokenStatic.SpendToken(1);
            TokenStatic.Unlock(data);
            return data;
        }
        public static void SetThresholdList(List<int> thresholdList)
        {
            ThresholdList = thresholdList.ToList();
        }
        public static int GetTokenThresholdExp()
        {
            if (ThresholdList.Count<=0)
            {
                return 1000;
            }
            var totalToken = GetTotalCollectedToken();
            var index = totalToken>=ThresholdList.Count ? ThresholdList.Count - 1 : totalToken;
            return ThresholdList[index];
        }
        
        public static void EarnExp(int amount)
        {
            var tokenSave = NSaver.GetSaveData<TokenSave>();
            tokenSave.CollectedExp += amount;
            tokenSave.TotalExp += amount;
            tokenSave.Save();
        }

        public static void SetExp(int amount)
        {
            var tokenSave = NSaver.GetSaveData<TokenSave>();
            tokenSave.CollectedExp = amount;
            tokenSave.Save();
        }
        
        public static int GetExp()
        {
            var tokenSave = NSaver.GetSaveData<TokenSave>();
            return tokenSave.CollectedExp;
        }
        
        #region Token

        public static void EarnToken(int count)
        {
            var tokenSave = NSaver.GetSaveData<TokenSave>();
            tokenSave.TotalCollectedToken += count;
            tokenSave.AvailableToken += count;
            tokenSave.Save();
            RBuss.Publish(new TokenREvents.TokenValueChangedREvent(count));

        }

        public static void SpendToken(int count)
        {
            var tokenSave = NSaver.GetSaveData<TokenSave>();
            var before = tokenSave.AvailableToken;
            tokenSave.AvailableToken -= count;
            if (tokenSave.AvailableToken<0)
                tokenSave.AvailableToken = 0;
            var diff = tokenSave.AvailableToken - before;
            tokenSave.Save();
            RBuss.Publish(new TokenREvents.TokenValueChangedREvent(diff));
        }

        public static int GetTotalCollectedToken()
        {
            var tokenSave = NSaver.GetSaveData<TokenSave>();
            return tokenSave.TotalCollectedToken;
        }

        public static int GetAvailableToken()
        {
            var tokenSave = NSaver.GetSaveData<TokenSave>();
            return tokenSave.AvailableToken;
        }

        public static bool TrySpendToken(int requiredToken, Action successCallback = null,Action failureCallback = null)
        {
            var tokenSave = NSaver.GetSaveData<TokenSave>();
            if (tokenSave.AvailableToken >= requiredToken)
            {
                SpendToken(requiredToken);
                successCallback?.Invoke();
                return true;
            }
            failureCallback?.Invoke();
            return false;
        }

        #endregion

        public static void Unlock(ITokenData tokenData)
        {
            var tokenSave = NSaver.GetSaveData<TokenSave>();
            tokenSave.UnlockToken(tokenData.GetTokenID());
            tokenSave.Save();
            RBuss.Publish(new TokenREvents.TokenUnlockedREvent(tokenData));
        }

        public static bool IsUnlocked(Object item)
        {
            if (item is ITokenData tokenData)
            {
                if (tokenData.IsTokenUnlockedByDefault())
                {
                    return true;
                }
                var tokenSave = NSaver.GetSaveData<TokenSave>();
                return tokenSave.IsTokenUnlocked(tokenData.GetTokenID());
            }

            return false;
        }
    }
}