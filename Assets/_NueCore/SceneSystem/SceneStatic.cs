using System;
using _NueCore.Common.NueLogger;
using _NueCore.Common.ReactiveUtils;
using _NueCore.FaderSystem;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _NueCore.SceneSystem
{
    public static class SceneStatic
    {
        public const string LobbyScene = "Main Menu";
        public const string GameScene = "Game Scene";
        public const string LoadingScene = "LoadingScene";
        public const string TransitionScene = "TransitionScene";
        public const string UpgradeScene = "Upgrade Scene";
        
        public static SceneEnums TransitionTarget { get; private set; }

        public static Action<SceneEnums> OnSceneChangedAction;
        #region Commons
        private static string GetSceneName(SceneEnums targetEnum)
        {
            return targetEnum switch
            {
                SceneEnums.LobbyScene => LobbyScene,
                SceneEnums.GameScene => GameScene,
                SceneEnums.LoadingScene => LoadingScene,
                SceneEnums.TransitionScene => TransitionScene,
                SceneEnums.UpgradeScene => UpgradeScene,
                _ => null
            };
        }
        
        public static bool CheckActiveScene(string sceneName)
        {
            return SceneManager.GetActiveScene().name == sceneName;
        }
        #endregion
        
        #region Direct
        public static void ChangeScene(string sceneName,SceneChangeParams @params = default)
        {
            RBuss.Publish(new SceneREvents.SceneChangeStartedREvent(sceneName));
            SceneManager.LoadScene(sceneName,@params.loadSceneMode);
            RBuss.Publish(new SceneREvents.SceneChangeFinishedREvent(sceneName));
        }
        public static void ChangeScene(SceneEnums sceneEnum,SceneChangeParams @params = default)
        {
            var sceneName = GetSceneName(sceneEnum);
            ChangeScene(sceneName,@params);
        }
        #endregion

        #region Async
        public static AsyncOperation ChangeSceneAsync(string sceneName,
            SceneChangeParams @params = default)
        {
            RBuss.Publish(new SceneREvents.SceneChangeStartedREvent(sceneName));
            var asyncOps =SceneManager.LoadSceneAsync(sceneName,@params.loadSceneMode);
            if (asyncOps == null) return null;
            asyncOps.completed += operation =>
            {
                if (operation.isDone)
                {
                    @params.onSceneChangedAction?.Invoke();
                    RBuss.Publish(new SceneREvents.SceneChangeFinishedREvent(sceneName));
                }
                else
                    "Scene Load Failed!".NLog(Color.red);
              
            };
            return asyncOps;
        }
        public static AsyncOperation ChangeSceneAsync(SceneEnums sceneEnum,
            SceneChangeParams @params = default)
        {
            var sceneName = GetSceneName(sceneEnum);
            return ChangeSceneAsync(sceneName, @params);
        }
        public static AsyncOperation ChangeSceneAsyncWithTransition(SceneEnums targetScene,
            SceneChangeParams @params = default)
        {
            TransitionTarget = targetScene;
            var asyncOps = ChangeSceneAsync(SceneEnums.TransitionScene,@params);
            return asyncOps;
        }

        #endregion

        #region Fader
        public static void ChangeSceneAsyncWithFader(SceneEnums sceneEnum,
            NFader.FaderParams faderParams,
            SceneChangeParams sceneParams = default)
        {
            var sceneName = GetSceneName(sceneEnum);
            ChangeSceneAsyncWithFader(sceneName,faderParams,sceneParams);
        }
        public static void ChangeSceneAsyncWithFader(string sceneName,
            NFader.FaderParams faderParams,
            SceneChangeParams sceneParams = default)
        {
            faderParams.fadeInFinishedAction += () =>
            {
                NFader.OpenWait(faderParams.fadeInType);
                DOVirtual.DelayedCall(faderParams.waitDuration, () =>
                {
                    var ops =ChangeSceneAsync(sceneName, sceneParams);
                    ops.completed += operation =>
                    {
                        if (operation.isDone)
                            NFader.FadeOut(faderParams.fadeOutType, faderParams);
                        else
                            "Scene Load2 Failed!".NLog(Color.red);
                    };
                }, false);
            };
            NFader.FadeIn(faderParams.fadeInType,faderParams);
            
        }

        #region Obsolute

        // public static void ChangeSceneWithFader(string sceneName,
        //     NFader.FaderParams faderParams = default,
        //     SceneChangeParams sceneParams = default)
        // {
        //     faderParams.fadeInFinishedAction += () =>
        //     {
        //         ChangeScene(sceneName, sceneParams);
        //     };
        //     NFader.Fade(FaderTypes.Default,faderParams);
        //     
        // }
        
        // public static void ChangeSceneWithFader(SceneEnums sceneEnum,
        //     NFader.FaderParams faderParams = default,
        //     SceneChangeParams sceneParams = default)
        // { 
        //     if (faderParams.fadeInDuration <=0 || faderParams.fadeOutDuration <=0)
        //         faderParams = NFader.FaderParams.Default;
        //     var sceneName = GetSceneName(sceneEnum);
        //    ChangeSceneAsyncWithFader(sceneName,faderParams,sceneParams);
        // }

        #endregion
       
        #endregion
        
        #region Structs
        public struct SceneChangeParams
        {
            public LoadSceneMode loadSceneMode;
            public Action onSceneChangedAction;

            public SceneChangeParams(LoadSceneMode loadSceneMode = LoadSceneMode.Single)
            {
                this.loadSceneMode = loadSceneMode;
                onSceneChangedAction = null;
            }
        }
        #endregion
    }
}