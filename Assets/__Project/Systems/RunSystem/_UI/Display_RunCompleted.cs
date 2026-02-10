using __Project.Systems.NUpgradeSystem;
using _NueCore.Common.Utility;
using _NueCore.FaderSystem;
using _NueCore.SaveSystem;
using _NueCore.SceneSystem;
using _NueExtras.StockSystem;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.UI;

namespace __Project.Systems.RunSystem._UI
{
    public class Display_RunCompleted : MonoBehaviour
    {
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private N_TMP_Text totalCoinText;
        [SerializeField] private MMF_Player openPlayer;
        [SerializeField] private bool isWin;
        [SerializeField] private bool ignoreSaveReset;
        
        
        public void Build()
        {
            continueButton.onClick.AddListener(() =>
            {
                var temp = RunStatic.Temp;
                temp.SetState(RunState.Transition);
            });
            mainMenuButton.onClick.AddListener((() =>
            {
                if (!ignoreSaveReset)
                {
                    NSaver.ResetSaveByType(SaveTypes.InGame);
                    SaveStatic.SetHasSave(false);
                }
                SceneStatic.ChangeSceneAsyncWithFader(SceneEnums.LobbyScene, NFader.FaderParams.Default);
            }));
            OnBuilt();
            openPlayer?.PlayFeedbacks();

            var totalPoint = 0;
            var upgradeSave = NSaver.GetSaveData<UpgradeSave>();
            
            totalPoint += upgradeSave.TotalPurchasedSkillPoints * 3;
            totalPoint += StockStatic.GetStockRounded(StockTypes.Coin);
            totalPoint += StockStatic.GetStockRounded(StockTypes.Gem) * 5;
            totalPoint += StockStatic.GetStockRounded(StockTypes.Emerald) * 10;
            
            totalCoinText.SetValue(totalPoint.ToString());
        }
        
        protected virtual void OnBuilt()
        {
            
        }
    }
}