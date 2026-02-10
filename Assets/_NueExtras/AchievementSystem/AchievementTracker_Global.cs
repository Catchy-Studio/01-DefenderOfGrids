using _NueCore.Common.Extensions;
using _NueCore.Common.ReactiveUtils;
using _NueExtras.RaritySystem;
using _NueExtras.TokenSystem;
using _NueExtras.TokenSystem._TokenCollection;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

namespace _NueExtras.AchievementSystem
{
    public class AchievementTracker_Global : MonoBehaviour
    {
        [SerializeField] private AchievementDataCatalog achievementDataCatalog;
        [SerializeField] private AchPop popPrefab;
        [SerializeField] private Transform popRoot;
        
        private void Awake()
        {
            AchStatic.SetCatalog(achievementDataCatalog);
            RegisterREvents();
        }

        private void RegisterREvents()
        {
            RBuss.OnEvent<AchievementREvents.AchievedREvent>().Subscribe(ev =>
            {
                var data = ev.Data;
                if (data == null) return;
                
                var pop = Instantiate(popPrefab, popRoot);
                pop.Build(data.Title, data.Description, data.Icon);
                pop.Show(3f);
            }).AddTo(gameObject);
            
            RBuss.OnEvent<TokenREvents.TokenUnlockedREvent>().Subscribe(ev =>
            {
                if (ev.Data.GetTokenCategory() is TokenCategory.Sticker)
                {
                    AchStatic.Achieve(AchEnum.Sticker_Collector);
                }

                if (ev.Data.GetTokenCategory() is TokenCategory.Relic)
                {
                    AchStatic.Achieve(AchEnum.Relic_Hunter);
                }

                if (ev.Data.GetTokenRarity() is NRarity.Legendary)
                {
                    AchStatic.Achieve(AchEnum.Lucky_Pull);
                }
            }).AddTo(gameObject);
        }

        [Button,HideInEditorMode]
        private void TestAchievementPop()
        {
            var pop = Instantiate(popPrefab, popRoot);
            var randAch = achievementDataCatalog.AchievementDataList.RandomItem();
            pop.Build(randAch.Title, randAch.Description, randAch.Icon);
            pop.Show(3f);
        }
        
    }
}