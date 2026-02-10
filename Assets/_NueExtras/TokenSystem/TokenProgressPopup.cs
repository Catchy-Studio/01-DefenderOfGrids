using System;
using _NueCore.SaveSystem;
using _NueExtras.PopupSystem.PopupDataSub;
using _NueExtras.TokenSystem._TokenCollection;
using Cysharp.Threading.Tasks;
using DamageNumbersPro;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _NueExtras.TokenSystem
{
    public class TokenProgressPopup : MonoBehaviour
    {
        [SerializeField] private PopupDisplay pop;
        [SerializeField] private TMP_Text currentProgressText;
        [SerializeField] private TMP_Text targetProgressText;
        [SerializeField] private TMP_Text collectedProgressText;
        [SerializeField] private Slider slider;
        [SerializeField] private DamageNumber notificationNumber;
        [SerializeField] private RectTransform notifyRect;
        [SerializeField] private TokenCollectionButton collectionButton;
        [SerializeField] private bool autoPopup;
        

        #region Cache
        public int CurrentExp { get; private set; }
        public int Threshold { get; private set; }
        public int CollectedExp { get; private set; }
        private string _cachedTargetProgressText;
        private string _cachedCurrentProgressText;
        private string _cachedCollectedProgressText;
        public Action OnFinishedAction { get; set; }

        public TokenCollectionButton CollectionButton => collectionButton;

        #endregion

        #region Setup
        public void Build(Action onFinishedAction,bool isCollectionDisabled = false)
        {
            slider.minValue = 0;
            slider.maxValue = 1;
            OnFinishedAction += onFinishedAction;
            _cachedTargetProgressText = targetProgressText.text;
            _cachedCurrentProgressText = currentProgressText.text;
            _cachedCollectedProgressText = collectedProgressText.text;
            var save = NSaver.GetSaveData<TokenSave>();
            CollectedExp = save.CollectedExp;
            CurrentExp = save.LastExp; 
            Threshold = TokenStatic.GetTokenThresholdExp(); 
            SetValues();
            CollectionButton.gameObject.SetActive(!isCollectionDisabled);
            CollectionButton.Build();
            CollectionButton.Button.interactable = false;
            Progress().AttachExternalCancellation(gameObject.GetCancellationTokenOnDestroy()).Forget();
        }
        #endregion

        #region Methods
        private void SetValues()
        {
          
            var inverse = Mathf.InverseLerp(0, Threshold, CurrentExp);
            slider.value = inverse;
            
            targetProgressText.SetText(_cachedTargetProgressText.Replace("#progress",Threshold.ToString()));
            currentProgressText.SetText(_cachedCurrentProgressText.Replace("#progress",CurrentExp.ToString()));
            collectedProgressText.SetText(_cachedCollectedProgressText.Replace("#progress",CollectedExp.ToString()));
            CollectionButton.UpdateTokenWarning();
        }

        

        #endregion

        #region Async
        private async UniTask Progress()
        {
            var requiredExp = Threshold - CurrentExp;
            var targetExp = 0;
            var bCollectedExp = CollectedExp;
            var bExp = CurrentExp;
            if (CollectedExp>=requiredExp)
            {
                targetExp = requiredExp;
                CollectedExp -= requiredExp;
            }
            else
            {
                targetExp = CollectedExp;
                CollectedExp = 0;
            }

            CurrentExp += targetExp;
            await UniTask.Delay(250);
            var tInverse = Mathf.InverseLerp(0, Threshold, CurrentExp);
            var seq = DOTween.Sequence();
            seq.Append(slider.DOValue(tInverse, 1f));
            seq.Join(DOVirtual.Int(bCollectedExp, CollectedExp, 1f, (value =>
            {
                collectedProgressText.SetText(_cachedCollectedProgressText.Replace("#progress", value.ToString()));
            })));
            seq.Join(DOVirtual.Int(bExp, CurrentExp, 1f, (value =>
            {
                currentProgressText.SetText(_cachedCurrentProgressText.Replace("#progress", value.ToString()));
            })));
            await seq;
            
            SetValues();

            await UniTask.Delay(200);
            
            if (CurrentExp>=Threshold)
            {
                ReProgressAsync().AttachExternalCancellation(gameObject.GetCancellationTokenOnDestroy()).Forget();
            }
            else
            {
                await FinishAsync();
            }
        }
        
        private async UniTask ReProgressAsync()
        {
            var numberClone = notificationNumber.SpawnGUI(notifyRect,Vector2.zero);
            //Set Parent and Anchored Position relative to rectParent
            numberClone.SetAnchoredPosition(notifyRect,Vector2.zero);
            TokenStatic.EarnToken(1);
            
            Threshold = TokenStatic.GetTokenThresholdExp();
            CurrentExp = 0;
            
            var save = NSaver.GetSaveData<TokenSave>();
            save.CollectedExp = CollectedExp;
            save.LastExp = CollectedExp<Threshold ? CollectedExp : 0;
            save.Save();

            if (autoPopup)
            {
                await TokenManager.Instance.ShowShowroomPopup();
            }
            await UniTask.Delay(200);
            
            SetValues();
            
            await UniTask.Delay(200);
            
            Progress().AttachExternalCancellation(gameObject.GetCancellationTokenOnDestroy()).Forget();
            
        }

        private async UniTask FinishAsync()
        {
            await UniTask.Delay(1000);
             
            var save = NSaver.GetSaveData<TokenSave>();
            save.LastExp = CurrentExp;
            save.CollectedExp = 0;
            save.Save();
            CollectionButton.Button.interactable = true;
            OnFinishedAction?.Invoke();
        }
        #endregion
    }
}