using _NueExtras.PopupSystem.PopupDataSub;
using Cysharp.Threading.Tasks;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace __Project.Systems.RunSystem._UI
{
    public class RunHUD : MonoBehaviour
    {
        [SerializeField] private Transform infoRoot;
        [SerializeField] private PopupDataDisplay winPopup;
        [SerializeField] private PopupDataDisplay losePopup;
        [SerializeField] private MMF_Player countdownPlayer;
        
        
        
        
        private bool _isFinished;
        public void Build()
        {
            infoRoot.gameObject.SetActive(false);
        }
        public void FinishRun(RunState state)
        {
            if (_isFinished)
                return;
            _isFinished = true;
            if (state is RunState.Win)
            {
                var pop =winPopup.OpenPopup();
                if (pop.TryGetComponent<Display_RunCompleted>(out var runCompleted))
                    runCompleted.Build();
            }

            // if (state is RunState.Fail)
            // {
            //     var pop = losePopup.OpenPopup();
            //     if (pop.TryGetComponent<Display_RunCompleted>(out var runFailed))
            //         runFailed.Build();
            // }
        }
        
        public void Quit()
        {
            Application.Quit();
        }

        public async UniTask Countdown()
        {
            await countdownPlayer.PlayFeedbacksTask();
        }
    }
}