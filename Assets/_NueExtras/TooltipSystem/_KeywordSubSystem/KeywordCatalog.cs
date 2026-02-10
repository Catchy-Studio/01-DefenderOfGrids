using System.Collections.Generic;
using _NueCore.Common.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NueGames.NTooltip._Keyword
{
    [CreateAssetMenu(fileName = "Keyword", menuName = "NueGames/NTooltip/Keyword/Catalog", order = 0)]
    public class KeywordCatalog : ScriptableObject
    {
        [SerializeField] private List<KeywordData> keywordList = new List<KeywordData>();
        [SerializeField, FolderPath] private List<string> relatedPathList = new List<string>();

        public List<KeywordData> KeywordList => keywordList;

        public KeywordData GetKeyword(string key)
        {
            foreach (var keyword in KeywordList)
            {
                if (keyword.GetKeywordID() == key)
                {
                    return keyword;
                }
            }
            Debug.LogWarning($"Keyword with ID '{key}' not found in catalog.");
            return null;
        }

#if UNITY_EDITOR
        [Button,TabGroup("Editor")]
        private void FindAll()
        {
            EditorHelper.FindAll(relatedPathList,keywordList);
           
        }
#endif
    }
}