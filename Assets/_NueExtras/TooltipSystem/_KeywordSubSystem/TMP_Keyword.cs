using TMPro;
using UnityEngine;

namespace NueGames.NTooltip._Keyword
{
    [RequireComponent(typeof(TMP_Text))]
    public class TMP_Keyword : MonoBehaviour
    {
        private void Awake()
        {
            var tmp = GetComponent<TMP_Text>();
            if (tmp == null)
            {
                Debug.LogError("TMP_Keyword requires a TMP_Text component on the same GameObject.");
                return;
            }
            ApplyKeyword(tmp);
        }

        public void ApplyKeyword(TMP_Text text)
        {
            text.SetText(KeywordStatic.ConvertKeywords(text.text));
        }
    }
}