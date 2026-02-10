using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _NueExtras.AchievementSystem
{
    public class AchPop : MonoBehaviour
    {
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text descText;
        [SerializeField] private Image iconImage;
        [SerializeField] private Transform root;
        
        
        public void Build(string title, string desc,Sprite icon)
        {
            titleText.text = title;
            descText.text = desc;
            iconImage.sprite = icon;
        }

        public void Show(float duration)
        {
            root.gameObject.SetActive(true);
            root.localScale = Vector3.zero;
            root.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).OnComplete((() =>
            {
                root.localScale = Vector3.one;
                DOVirtual.DelayedCall(duration, (() =>
                {
                    Hide();
                })).SetLink(gameObject);
            })).SetLink(gameObject);
        }

        public void Hide()
        {
            root.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete((() =>
            {
                if (gameObject)
                    Destroy(gameObject);
            })).OnKill((() =>
            {
                if (gameObject)
                    Destroy(gameObject);
            })).SetLink(gameObject);
        }
        
    }
}