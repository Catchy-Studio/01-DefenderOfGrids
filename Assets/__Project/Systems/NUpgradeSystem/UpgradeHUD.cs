using System;
using System.Text;
using __Project.Systems.NUpgradeSystem._SkillTree;
using _NueCore.Common.ReactiveUtils;
using _NueCore.Common.Utility;
using _NueCore.FaderSystem;
using _NueCore.NStatSystem;
using _NueCore.SaveSystem;
using _NueCore.SceneSystem;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace __Project.Systems.NUpgradeSystem
{
    public class UpgradeHUD : MonoBehaviour
    {
        [SerializeField] private Button playButton;
        [SerializeField] private CanvasGroup statsCanvasGroup;
        [SerializeField] private TMP_Text statsText;
        [SerializeField] private TMP_Text warningText;
        [SerializeField] private SkillData ariseData;
        
        private bool _isStatsVisible = false;
        private IDisposable _disposable;
        public void Build()
        {
            statsCanvasGroup.alpha = 0;
            statsCanvasGroup.gameObject.SetActive(false);
            warningText.gameObject.SetActive(false);
            var save = NSaver.GetSaveData<UpgradeSave>();
            var nodeSave = save.GetNodeSave(ariseData.GetID());
            if (!nodeSave.isUnlocked)
            {
                playButton.interactable = false;
                warningText.gameObject.SetActive(true);
                warningText.SetText("Unlock 'Hero' to Play!");
                _disposable =RBuss.OnEvent<SkillTreeREvents.SkillNodePurchasedEvent>().DelayFrame(1).Subscribe(ev =>
                {
                    if (ev.Node.Data.GetID() == ariseData.GetID())
                    {
                        if (UpgradeStatic.HasStat(NStatEnum.Unlock_Game))
                        {
                            warningText.gameObject.SetActive(false);
                            playButton.interactable = true;
                            _disposable?.Dispose();
                        }
                        else
                        {
                            playButton.interactable = false;
                        }
                    }
                }).AddTo(gameObject);
            }
            playButton.onClick.AddListener((() =>
            {
                SceneStatic.ChangeSceneAsyncWithFader(SceneEnums.GameScene,NFader.FaderParams.Default); 
            }));
        }
        

        private Tween _canvasTween;

        public void ShowStats()
        {
            if (_isStatsVisible)
            {
                return;
            }
            //statsCanvasGroup.gameObject.SetActive(true);
            var str = new StringBuilder();
            str.AppendLine("STATS:");
            var save = NSaver.GetSaveData<UpgradeSave>();
            foreach (var statSave in save.SkillTreeStatSaveList)
            {
                if (statSave.GetTotal() ==0)
                    continue;
                
                var statName = statSave.key;
                
                if (statSave.key.TryConvertToNStatEnum(out NStatEnum nStatEnum))
                    statName = nStatEnum.GetStatDisplayName();
                
                str.AppendLine($"{statName}: {statSave.GetTotal()}");
            }
            statsText.SetText(str.ToString());
            _isStatsVisible = true;
            _canvasTween?.Kill();
            _canvasTween =statsCanvasGroup.DOFade(1,0.5f).SetEase(Ease.InOutSine);
        }

        public void HideStats()
        {
            // if (!_isStatsVisible)
            // {
            //     return;
            // }

            _isStatsVisible = false;
            _canvasTween?.Kill();
            _canvasTween =statsCanvasGroup.DOFade(0,0.5f).SetEase(Ease.InOutSine);
        }
        
    }
}