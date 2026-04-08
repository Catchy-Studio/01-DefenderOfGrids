using _NueCore.Common.NueLogger;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NueGames.NTooltip._Keyword;
using Sirenix.OdinInspector;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace _NueExtras.TutorialSystem._Dialog
{
    public class TutorialDialog : MonoBehaviour
    {
        [SerializeField,TabGroup("Dialog")] private Transform root;
        [SerializeField,TabGroup("Dialog")] private CanvasGroup canvasGroup;
        [SerializeField,TabGroup("Dialog")] private TMP_Text dialogText;
        [SerializeField,TabGroup("Dialog")] private Ease ease;
        [SerializeField,TabGroup("Dialog")] private float openDuration = 0.5f;

        [SerializeField, TabGroup("Dialog")] private bool useFallbackTypewriter = true;
        [SerializeField, TabGroup("Dialog"), Min(1f)] private float fallbackCharsPerSecond = 40f;
        
        [SerializeField,TabGroup("Speaker")] private Transform speakerRoot;
        [SerializeField,TabGroup("Speaker")] private TMP_Text speakerName;
        [SerializeField,TabGroup("Speaker")] private Image speakerImage;
        
        [SerializeField,TabGroup("Audio")] private AudioSource audioSource;

        #region Cache

        private Tween _speakerTween;
        private Component _typeWriter;
        private Tween _openTween;
        public bool IsFinished { get; private set; }

        #endregion

        #region Setup
        public void Play(string text)
        {
            PlayAsync(text).AttachExternalCancellation(gameObject.GetCancellationTokenOnDestroy()).Forget();
        }
       
        public async UniTask PlayAsync(string text)
        {
            root.gameObject.SetActive(true);
            dialogText.text = text.ApplyKeywords();
            Show();
            await _openTween;
            await CheckTypeWrite(dialogText, false);
            IsFinished = true;
        }
        #endregion

        #region Typewrite
        public async UniTask CheckTypeWrite(TMP_Text text,bool continueToClick = false)
        {
            text.text = text.text.ApplyKeywords();

            // If Febucci's Typewriter is installed, drive it without a compile-time dependency.
            // (GetComponent(string) keeps this file compiling even when the package is missing.)
            _typeWriter = text.GetComponent("TypewriterByCharacter");
            if (_typeWriter != null)
            {
                var isFinished = false;

                // Try to hook completion if the component exposes "onTextShowed" UnityEvent.
                var evt = _typeWriter.GetType().GetField("onTextShowed");
                if (evt != null)
                {
                    if (evt.GetValue(_typeWriter) is UnityEngine.Events.UnityEvent unityEvent)
                    {
                        unityEvent.AddListener(() => isFinished = true);
                    }
                }

                gameObject.UpdateAsObservable().Where(_ => Input.GetMouseButtonDown(0)).Take(1).Subscribe(_ =>
                {
                    _typeWriter.SendMessage("SkipTypewriter", SendMessageOptions.DontRequireReceiver);
                    isFinished = true;
                }).AddTo(gameObject);

                await UniTask.WaitUntil(() => isFinished);
            }
            else if (useFallbackTypewriter)
            {
                await RunFallbackTypewriter(text);
            }
            // else: no typewriter available, continue immediately.

            if (continueToClick)
            {
                var isClicked = false;
                Observable.EveryUpdate()
                    .Where(_ => Input.GetMouseButtonDown(0)).Take(1).Subscribe(xs => isClicked = true).AddTo(gameObject);
                await UniTask.WaitUntil(() => isClicked);
            }
        }

        private async UniTask RunFallbackTypewriter(TMP_Text text)
        {
            var full = text.text ?? string.Empty;
            if (full.Length == 0) return;

            text.maxVisibleCharacters = 0;
            var visible = 0;
            var isFinished = false;

            gameObject.UpdateAsObservable().Where(_ => Input.GetMouseButtonDown(0)).Take(1).Subscribe(_ =>
            {
                text.maxVisibleCharacters = int.MaxValue;
                isFinished = true;
            }).AddTo(gameObject);

            var delay = 1f / Mathf.Max(1f, fallbackCharsPerSecond);
            while (!isFinished && visible < full.Length)
            {
                visible++;
                text.maxVisibleCharacters = visible;
                await UniTask.Delay(System.TimeSpan.FromSeconds(delay), cancellationToken: gameObject.GetCancellationTokenOnDestroy());
            }

            text.maxVisibleCharacters = int.MaxValue;
        }
        #endregion

        #region Methods
        private void Show()
        {
            _openTween?.Kill();
            canvasGroup.alpha = 0f;
            _openTween=canvasGroup.DOFade(1, openDuration).SetEase(ease);
        }

        public void Hide()
        {
            StopAudio();
            root.gameObject.SetActive(false);
        }
        public void StopAudio()
        {
            _speakerTween?.Kill(true);
            if (audioSource)
            {
                audioSource.Stop();
                
            }
        }
        public void PlayClip(AudioClip clip)
        {
            if (!audioSource) return;
            if (clip)
            {
                audioSource.clip = clip;
                //audioSource.mute = isAudioDisabled;
                audioSource.Play();
                _speakerTween =speakerRoot.transform.DOShakeScale(clip.length,Vector3.up*0.05f,1,10).OnComplete(() =>
                {
                    speakerRoot.localScale = Vector3.one;
                });
            }
            else
            {
                audioSource.enabled = false;
            }
        }
        #endregion
    }
}