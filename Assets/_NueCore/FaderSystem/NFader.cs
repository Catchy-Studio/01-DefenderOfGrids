using System;
using _NueCore.Common.NueLogger;
using DG.Tweening;
using UnityEngine;

namespace _NueCore.FaderSystem
{
    public static class NFader
    {
        #region Cache
        private static FaderManager FaderManager => FaderManager.Instance;
        private static Sequence _fadeSeq;
        private static readonly int FadeInAnimKey = Animator.StringToHash("FadeIn");
        private static readonly int FadeOutAnimKey = Animator.StringToHash("FadeOut");
        private static readonly int MotionKey = Animator.StringToHash("Motion");

        #endregion

        #region Fades
        public static void Fade(FaderTypes faderType, FaderParams @params = default)
        {
            if (FaderManager.TransitionInfoDict.TryGetValue(faderType, out var transitionInfo))
            {
                transitionInfo.SetDuration(FaderManager.FadeStep.FadeIn,@params.fadeInDuration);
                transitionInfo.SetDuration(FaderManager.FadeStep.FadeOut,@params.fadeOutDuration);
                transitionInfo.SetDuration(FaderManager.FadeStep.Wait,@params.waitDuration);
            }
            _fadeSeq?.Kill();
            _fadeSeq.SetUpdate(UpdateType.Normal, true);
            _fadeSeq = DOTween.Sequence();
            _fadeSeq.SetEase(Ease.Linear);
            _fadeSeq.Append(FadeIn(faderType, @params).OnComplete(() =>
            {
                OpenWait(faderType);
                DOVirtual.DelayedCall(@params.waitDuration, () =>
                {
                    @params.waitFinishedAction?.Invoke();
                    FadeOut(faderType, @params);
                }, false).SetEase(Ease.Linear).SetUpdate(UpdateType.Normal, true);
            }).SetEase(Ease.Linear));
        }
        public static void OpenWait(FaderTypes faderType)
        {
            "Wait".NLog(Color.magenta);
            if (FaderManager.TransitionInfoDict.TryGetValue(faderType, out var transitionInfo))
            {
                transitionInfo.SetActive(FaderManager.FadeStep.Wait);
            }
          
        }
        public static Tween FadeIn(FaderTypes faderType, FaderParams @params = default)
        {
            if (FaderManager.TransitionInfoDict.TryGetValue(faderType, out var transitionInfo))
            {
                transitionInfo.SetActive(FaderManager.FadeStep.FadeIn);
                transitionInfo.SetDuration(FaderManager.FadeStep.FadeIn,@params.fadeInDuration);
                transitionInfo.Play(FaderManager.FadeStep.FadeIn);
            }
            FaderManager.FaderRoot.gameObject.SetActive(true);
            return DOVirtual.DelayedCall(@params.fadeInDuration, () =>
            {
                @params.fadeInFinishedAction?.Invoke();
            }, false).SetEase(Ease.Linear).SetLink(FaderManager.gameObject).SetUpdate(UpdateType.Normal, true);
        }
        public static Tween FadeOut(FaderTypes faderType,FaderParams @params = default)
        {
            if (FaderManager.TransitionInfoDict.TryGetValue(faderType, out var transitionInfo))
            {
                transitionInfo.SetActive(FaderManager.FadeStep.FadeOut);
                transitionInfo.SetDuration(FaderManager.FadeStep.FadeOut,@params.fadeOutDuration);
                transitionInfo.Play(FaderManager.FadeStep.FadeOut);
            }
            @params.fadeOutDuration.NLog(Color.magenta);
            FaderManager.FaderRoot.gameObject.SetActive(true);
            return DOVirtual.DelayedCall(@params.fadeOutDuration, () =>
            {
                "Fade out finished".NLog(Color.yellow);
                //transitionInfo.DisableAll();
                @params.fadeOutFinishedAction?.Invoke();
                //FaderManager.FaderRoot.gameObject.SetActive(false);
            }, false).SetEase(Ease.Linear).SetLink(FaderManager.gameObject).SetUpdate(UpdateType.Normal, true);
        }
        #endregion
        
        #region Structs
        public struct FaderParams
        {
            public float fadeInDuration;
            public float fadeOutDuration;
            public float waitDuration;
            public Action fadeInFinishedAction;
            public Action fadeOutFinishedAction;
            public Action waitFinishedAction;
            public FaderTypes fadeInType;
            public FaderTypes fadeOutType;

            public static FaderParams Default => new FaderParams()
            {
                fadeInDuration = 0.5f,
                fadeOutDuration = 0.5f,
                waitDuration = 0.5f,
                fadeInType = FaderTypes.Default,
                fadeOutType = FaderTypes.Default
            };
            public FaderParams(float fadeInDuration = 1f, float fadeOutDuration = 1f)
            {
                this.fadeInDuration = fadeInDuration;
                this.fadeOutDuration = fadeOutDuration;
                waitDuration = 0;
                fadeInFinishedAction = null;
                fadeOutFinishedAction = null;
                waitFinishedAction = null;
                fadeInType = FaderTypes.Default;
                fadeOutType = FaderTypes.Default;
            }
        }
        #endregion
    }
}