using System;
using _NueExtras.PopupSystem.PopupDataSub;
using _NueExtras.RaritySystem;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NueGames.NTooltip._Keyword;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _NueExtras.TokenSystem._TokenCollection
{
    public class TokenShowroom : MonoBehaviour
    {
        [SerializeField] private PopupDisplay pop;
        
        [SerializeField] private Transform root;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text descText;
        [SerializeField] private TMP_Text rarityText;
        [SerializeField] private Image iconImage;
        [SerializeField] private Animator animator;

        private bool _isClicked;
        public void Build()
        {
            root.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _isClicked = true;
            }
        }

        public async UniTask ShowAsync(ITokenData tokenData)
        {
            _isClicked = false;
            root.gameObject.SetActive(true);
            var tName = tokenData.GetTokenName();
            var tDesc = tokenData.GetTokenDescription();
            // var keywords =KeywordStatic.ExtractKeywords(tDesc);
            // KeywordStatic.ExtractKeywords(tName, ref keywords);
            tName = KeywordStatic.ConvertKeywords(tName);
            tDesc = KeywordStatic.ConvertKeywords(tDesc);
            titleText.SetText(tName);
            descText.SetText(tDesc);
            iconImage.sprite = tokenData.GetTokenIcon();
            rarityText.gameObject.SetActive(false);
            rarityText.SetText(tokenData.GetTokenRarity().GetRarityKeyword().ApplyKeywords());
            
            //await root.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce);
            animator.SetTrigger("Show");
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            var clickTask = UniTask.WaitUntil(() => _isClicked);
            var t = UniTask.Delay(TimeSpan.FromSeconds(5));
            await UniTask.WhenAny(t,clickTask);
            
            animator.SetTrigger("Hide");
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            
            //await root.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBounce);
            root.gameObject.SetActive(false);
            pop.ClosePopup();

        }
    }
}