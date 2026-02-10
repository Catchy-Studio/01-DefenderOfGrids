using System;
using _NueCore.Common.KeyValueDict;
using _NueCore.ManagerSystem.Core;
using Sirenix.OdinInspector;
using TransitionsPlus;
using UnityEngine;

namespace _NueCore.FaderSystem
{
    public class FaderManager : NManagerBase
    {
        [SerializeField] private Animator faderAnimator;
        [SerializeField] private Transform faderRoot;

        [SerializeField]
        private KeyValueDict<FaderTypes, TransitionInfo> transitionInfoDict =
            new KeyValueDict<FaderTypes, TransitionInfo>();
        
        [Serializable]
        public class TransitionInfo
        {
            public TransitionAnimator inAnimator;
            public TransitionAnimator outAnimator;
            public Transform waitRoot;

            public void Play(FadeStep fadeStep)
            {
                switch (fadeStep)
                {
                    case FadeStep.FadeIn:
                        inAnimator.Play();
                        break;
                    case FadeStep.FadeOut:
                        outAnimator.Play();
                        break;
                    case FadeStep.Wait:
                        //waitRoot.Play();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(fadeStep), fadeStep, null);
                }
            }
            public void SetDuration(FadeStep fadeStep ,float duration)
            {
                switch (fadeStep)
                {
                    case FadeStep.FadeIn:
                        inAnimator.profile.duration = duration;
                        break;
                    case FadeStep.FadeOut:
                        outAnimator.profile.duration = duration;
                        break;
                    case FadeStep.Wait:
                        //waitRoot.profile.duration = duration;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(fadeStep), fadeStep, null);
                }
            }
            public void SetActive(FadeStep step)
            {
                switch (step)
                {
                    case FadeStep.FadeIn:
                        inAnimator.gameObject.SetActive(true);
                        outAnimator.gameObject.SetActive(false);
                        waitRoot.gameObject.SetActive(false);
                        break;
                    case FadeStep.FadeOut:
                        inAnimator.gameObject.SetActive(false);
                        outAnimator.gameObject.SetActive(true);
                        waitRoot.gameObject.SetActive(false);
                        break;
                    case FadeStep.Wait:
                        inAnimator.gameObject.SetActive(false);
                        outAnimator.gameObject.SetActive(false);
                        waitRoot.gameObject.SetActive(true);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(step), step, null);
                }
            }

            public void DisableAll()
            {
                inAnimator.gameObject.SetActive(false);
                outAnimator.gameObject.SetActive(false);
                waitRoot.gameObject.SetActive(false);
            }
            
        }

        public enum FadeStep
        {
            FadeIn,
            FadeOut,
            Wait
        }
        
        public static FaderManager Instance { get; private set; }

        public Animator FaderAnimator => faderAnimator;

        public Transform FaderRoot => faderRoot;

        public KeyValueDict<FaderTypes, TransitionInfo> TransitionInfoDict => transitionInfoDict;

        #region Setup
        public override void NAwake()
        {
            Instance = InitSingleton<FaderManager>();
            base.NAwake();
        }

        public override void NStart()
        {
            base.NStart();
            var fadeParams = new NFader.FaderParams
            {
                fadeInDuration = 0f,
                fadeOutDuration = 1f,
                waitDuration = 0.25f
            };
            NFader.Fade(FaderTypes.Default,fadeParams);
        }

        #endregion

        [Button]
        public void Fade(FaderTypes faderType, NFader.FaderParams faderParams)
        {
            //transitionInfoDict[faderType].inAnimator.play
            
            NFader.Fade(faderType,faderParams);
        }

        public void SetFaderLayer(FaderTypes faderType)
        {
            for (int i = 0; i < faderAnimator.layerCount; i++)
                faderAnimator.SetLayerWeight(i,0);
            faderAnimator.SetLayerWeight((int)faderType,1);
        }
    }
}