using System.Collections.Generic;
using _NueCore.Common.KeyValueDict;
using _NueCore.Common.NueLogger;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _NueExtras.TokenSystem._TokenCollection
{
    [CreateAssetMenu(fileName = "Token Collection", menuName = "TokenSystem/TokenCollection", order = 0)]
    public class TokenCollectionCatalog : ScriptableObject
    {
        [SerializeField] private List<Object> tokenList;
        public List<Object> TokenList => tokenList;

        public List<ITokenData> GetTokenByCategory(TokenCategory category)
        {
            List<ITokenData> result = new List<ITokenData>();
            foreach (var item in TokenDict)
            {
                if (item.Value == null)
                {
                    "Token data is null".NLog(Color.red);
                    continue;
                }

                if (item.Value.GetTokenCategory() == category)
                {
                    result.Add(item.Value);
                }
            }

            return result;
        }
        [ShowInInspector,ReadOnly]public KeyValueDict<Object, ITokenData> TokenDict { get; private set; } =
            new KeyValueDict<Object, ITokenData>();

        public List<ITokenData> GetAvailableTokens()
        {
            List<ITokenData> result = new List<ITokenData>();
            foreach (var item in TokenDict)
            {
                if (item.Value == null)
                {
                    "Token data is null".NLog(Color.red);
                    continue;
                }

                if (item.Value.IsTokenBlocked())
                {
                    continue;
                }

                if (!item.Value.IsTokenUnlocked())
                {
                    result.Add(item.Value);
                }
            }

            return result;
        }
        public void Build()
        {
            SetTokenDict();
        }
        [Button,HideInPlayMode]
        private void SetTokenDict()
        {
            TokenDict.Clear();
            TokenList.RemoveAll(item => item == null);
            TokenList.RemoveAll(item => item is not ITokenData);
            
            foreach (var item in TokenList)
            {
                if (item == null)
                {
                    "Item is null".NLog(Color.red);
                    continue;
                }

                if (TokenDict.ContainsKey(item))
                    continue;

                if (item is not ITokenData cast)
                {
                    "Item is not matched".NLog(Color.red);
                    TokenList.Remove(item);
                    continue;
                }

                TokenDict.Add(item, cast);
            }

        }
        
    }
}