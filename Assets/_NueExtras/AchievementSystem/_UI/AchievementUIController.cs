using _NueCore.Common.ReactiveUtils;
using _NueExtras.PopupSystem.PopupDataSub;
using UniRx;
using UnityEngine;

namespace _NueExtras.AchievementSystem
{
    public class AchievementUIController : MonoBehaviour
    {
        [SerializeField] private PopupDataDisplay achievementPopupData;

        private void Awake()
        {
            RBuss.OnEvent<AchievementREvents.OpenREvent>().TakeUntilDisable(gameObject)
                .Subscribe(ev =>
                {
                    OpenPopup();
                });
            
            RBuss.OnEvent<AchievementREvents.CloseREvent>().TakeUntilDisable(gameObject)
                .Subscribe(ev =>
                {
                    ClosePopup();
                });
        }

        private PopupDisplay _popup;
        public void OpenPopup()
        {
            ClosePopup();
            _popup = achievementPopupData.OpenPopup();
            if (_popup.TryGetComponent<AchCollectionPopup>(out var controller))
            {
                _popup.PopupClosedAction += () =>
                {
                    RBuss.Publish(new AchievementREvents.PopupClosedREvent());
                };
                controller.Build();
                RBuss.Publish(new AchievementREvents.PopupOpenedREvent());
            }
        }

        private void ClosePopup()
        {
            if (_popup)
            {
                _popup.ClosePopup();
                _popup = null;
            }
        }
    }
}