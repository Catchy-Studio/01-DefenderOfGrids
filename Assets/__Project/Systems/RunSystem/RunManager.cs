using __Project.Systems.NUpgradeSystem;
using __Project.Systems.RunSystem._UI;
using _NueCore.Common.Extensions;
using _NueCore.Common.NueLogger;
using _NueCore.Common.ReactiveUtils;
using _NueCore.FaderSystem;
using _NueCore.ManagerSystem.Core;
using _NueCore.SaveSystem;
using _NueCore.SceneSystem;
using _NueExtras.StockSystem;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;


namespace __Project.Systems.RunSystem
{
    public class RunManager : NManagerBase
    {
        [SerializeField] private RunHUD runHUD;
        
        public static RunManager Instance { get; private set; }
        
        #region Setup

        public override void NAwake()
        {
            Instance = InitSingleton<RunManager>();
            RunStatic.ResetTemp();
            var save = NSaver.GetSaveData<UpgradeSave>();
            var st =RunStatic.Temp.GetStatHandler();
            st.InitStats(save.SkillTreeStatSaveList);
            base.NAwake();
        }

        public override void NStart()
        {
            base.NStart();
            //CameraStatic.ChangeCamera(CameraEnum.Idle);
            RunStatic.Temp.TimeScale.Value = 1;
            Build();
        }

        public void Build()
        {
            RunStatic.Temp.SetState(RunState.Idle);
            RegisterButtons();

            RegisterREvents();
            runHUD.Build();
            StartRunAsync().AttachExternalCancellation(gameObject.GetCancellationTokenOnDestroy()).Forget();
        }

        private async UniTask StartRunAsync()
        {
            var temp = RunStatic.Temp;
            temp.SetState(RunState.Game);
        }

        private void RegisterButtons()
        {
        }

     
        #endregion

        #region Reactive

        private void RegisterREvents()
        {
            var temp = RunStatic.Temp;
            temp.RunState.Subscribe(state =>
            {
                $"CURRENT STATE: {state.ToString().Colorize(state.GetColor())}".NLog(Color.white);
                if (state is RunState.Transition)
                {
                    var faderParams = new NFader.FaderParams
                    {
                        fadeInDuration = 0.5f,
                        waitDuration = 0.5f,
                        fadeOutDuration = 0.5f
                    };
                    
                    NFader.Fade(FaderTypes.Default, faderParams);
                    SceneStatic.ChangeSceneAsyncWithFader(SceneEnums.UpgradeScene,faderParams);
                }

                if (state is RunState.Fail)
                {
                    var faderParams = new NFader.FaderParams
                    {
                        fadeInDuration = 0.5f,
                        waitDuration = 0.5f,
                        fadeOutDuration = 0.5f
                    };
                   
                    NFader.Fade(FaderTypes.Default, faderParams);
                    SceneStatic.ChangeSceneAsyncWithFader(SceneEnums.GameScene,faderParams);
                }
            }).AddTo(gameObject);

            RBuss.OnEvent<StockREvents.StockValueChangedREvent>().Subscribe(ev =>
            {
                temp.IncreaseStock(ev.StockType,ev.RoundedDelta);
            }).AddTo(gameObject);
        }


        #endregion

        #region Methods

        public void SetSpeed(float speed)
        {
            RunStatic.Temp.TimeScale.Value = speed;
        }

        #endregion
    }
}