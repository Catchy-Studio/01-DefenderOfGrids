using System;
using _NueCore.Common.KeyValueDict;
using _NueCore.Common.ReactiveUtils;
using _NueCore.Common.Sandbox;
using _NueCore.ManagerSystem.Core;
using _NueCore.SceneSystem;
using UniRx;
using UnityEngine;

namespace _NueCore.AudioSystem
{
    public class AudioManager : NManagerBase
    {

    
        [SerializeField] private AudioCatalog allAudioCatalog;
        [SerializeField] private KeyValueDict<DefaultAudioDataTypes,AudioData> defaultAudioClipDict;
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioClip mainMenuClip;
        [SerializeField] private AudioClip gameMusicClip;
        
        
        #region Cache
        public static AudioManager Instance { get; private set; }
        
        public bool IsAudioDisabled { get; private set; }
        
        //public AudioCatalog AllAudioCatalog => allAudioCatalog;
        
        #endregion
        
        #region Setup
        public override void NAwake()
        {
            Instance = InitSingleton<AudioManager>();
            base.NAwake();
            RBuss.OnEvent<SceneREvents.SceneChangeFinishedREvent>().TakeUntilDestroy(gameObject)
                .Subscribe(
                    ev => { CheckMusic(ev.NewSceneName); });
            
            CheckMusic("Main Menu");
        }
        
        private void CheckMusic(string sceneName)
        {
            var source =GetAudioSource(AudioSourceTypes.Music);
            if (sceneName == SceneStatic.LobbyScene)
                source.clip = mainMenuClip;
            else if (sceneName == SceneStatic.GameScene)
                source.clip = gameMusicClip;
            else
                return;
           
            source.time = 0;
            PlayMusic(AudioSourceTypes.Music);
        }
        #endregion
        
        #region Methods
        
        public AudioData GetDefaultAudioData(DefaultAudioDataTypes dataType)
        {
            if (!defaultAudioClipDict.ContainsKey(dataType))
            {
                return null;
            }
            return defaultAudioClipDict[dataType];
        }
        public void PlayOneShot(AudioClip clip,AudioSourceTypes sourceType)
        {
            var sfxSource =GetAudioSource(sourceType);
            if (sfxSource == null)
            {
                return;
            }
            sfxSource.PlayOneShot(clip);
        }
        public void PlayMusic(AudioSourceTypes sourceType)
        {
            var source = GetAudioSource(sourceType);
            
            musicSource.Play();
        }
        public void StopMusic(AudioSourceTypes sourceType)
        {
            var source = GetAudioSource(sourceType,AudioSourceTypes.Music);
        
            musicSource.Stop();
        }
        public AudioSource GetAudioSource(AudioSourceTypes sourceType, AudioSourceTypes defaultSource = AudioSourceTypes.Sfx)
        {
            return sourceType switch
            {
                AudioSourceTypes.Sfx => sfxSource,
                AudioSourceTypes.Music => musicSource,
                _ => throw new ArgumentOutOfRangeException(nameof(sourceType), sourceType, null)
            };
        }
        #endregion
    }
}