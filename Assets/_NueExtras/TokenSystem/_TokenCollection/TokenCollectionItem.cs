using System;
using System.Collections.Generic;
using __Project.Systems.ChoiceSystem;
using _NueCore.Common.NueLogger;
using UnityEngine;
using Object = System.Object;

namespace _NueExtras.TokenSystem._TokenCollection
{
    [Serializable]
    public class TokenCollectionItem
    {
        [SerializeField] private List<Object> tokenList;
        public List<Object> TokenList => tokenList;
        public static Dictionary<Object, IChoiceItem> TokenDict { get; private set; } =
            new Dictionary<Object, IChoiceItem>();
       
        private void SetTokenDict()
        {
            foreach (var item in TokenList)
            {
                if (item == null)
                {
                    "Item is null".NLog(Color.red);
                    continue;
                }
                if (TokenDict.ContainsKey(item))
                    continue;

                if (item is not IChoiceItem cast)
                {
                    "Item is not matched".NLog(Color.red);
                    TokenList.Remove(item);
                    continue;
                }
                TokenDict.Add(item,cast);
            }
        }
    }
}