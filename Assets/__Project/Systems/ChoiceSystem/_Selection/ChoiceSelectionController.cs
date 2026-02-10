using __Project.Systems.ChoiceSystem._Display;
using _NueCore.Common.ReactiveUtils;
using _NueExtras.CheatSystem;
using _NueExtras.PopupSystem.PopupDataSub;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

namespace __Project.Systems.ChoiceSystem._Selection
{
    public class ChoiceSelectionController : MonoBehaviour
    {
        [SerializeField] private PopupDataDisplay choicePopupData;
        [SerializeField] private ChoiceSelectionCatalog choiceSelectionCatalog;

        #region Cache

        private PopupDisplay _pop;

        #endregion

        #region Setup

        private void Awake()
        {
            RegisterREvents();
        }

        #endregion

        #region Reactive

        private void RegisterREvents()
        {
            
            RBuss.OnEvent<CheatREvents.CheatOpenChoiceREvent>()
                .TakeUntilDisable(gameObject)
                .Subscribe(ev =>
                {
                    ShowChoice();
                });
        }

        #endregion

        #region Methods
        [Button]
        private void ShowChoice()
        {
            RBuss.Publish(new ChoiceREvents.ChoiceShownREvent());
            ClosePopup();
            var choice = choiceSelectionCatalog.GetChoiceSelectionData(0);
            if (choice == null)
                return;
            
            _pop = choicePopupData.OpenPopup();
            
            _pop.PopupClosedAction += () =>
            {
            };

            if (_pop.TryGetComponent<DisplayChoice3>(out var cont))
            {
                cont.Build(choice,_pop);
            }
            else
            {
                ClosePopup();
            }
        }

        private void ClosePopup()
        {
            if (_pop)
            {
                _pop.ClosePopup();
                _pop = null;
            }
        }

        #endregion
    }
}