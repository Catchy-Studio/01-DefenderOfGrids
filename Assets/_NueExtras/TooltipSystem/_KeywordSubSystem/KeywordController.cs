using UnityEngine;

namespace NueGames.NTooltip._Keyword
{
    public class KeywordController : MonoBehaviour
    {
        [SerializeField] private KeywordCatalog catalog;

        private void Awake()
        {
            KeywordStatic.SetKeywordCatalog(catalog);
        }
    }
}