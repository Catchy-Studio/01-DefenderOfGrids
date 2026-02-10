using System;
using System.Collections.Generic;
using _NueCore.Common.ReactiveUtils;
using _NueCore.ManagerSystem.Core;
using _NueExtras.PopupSystem.PopupDataSub;
using _NueExtras.TokenSystem._TokenCollection;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

namespace _NueExtras.TokenSystem
{
    public class TokenManager : NManagerBase
    {
        [SerializeField] private List<int> thresholdList = new List<int>();
        [SerializeField] private PopupDataDisplay tokenProgressPopup;
        [SerializeField] private PopupDataDisplay tokenCollectionPopup;
        [SerializeField] private PopupDataDisplay showroomPopupData;
        [SerializeField] private TokenCollectionCatalog catalog;
        
        public static TokenManager Instance { get; private set; }
        #region Setup

        public override void NAwake()
        {
            Instance = InitSingleton<TokenManager>();
            TokenStatic.SetThresholdList(thresholdList);
            TokenStatic.SetTokenCollectionCatalog(catalog);
            TokenStatic.Catalog.Build();
            base.NAwake();
            
            RegisterREvents();
        }

        public override void NStart()
        {
            base.NStart();
           
        }

        #endregion

        #region Reactive

        private void RegisterREvents()
        {
            
        }


        #endregion

        #region Methods

        public async UniTask ShowShowroomPopup()
        {
            var pop = showroomPopupData.OpenPopup();
            if (pop.TryGetComponent<TokenShowroom>(out var showroom))
            {
                showroom.Build();
                var data =TokenStatic.UnlockRandomTokenData();
                if (data != null)
                {
                    await showroom.ShowAsync(data);
                }
               
            }
        }
        
        [Button,HideInEditorMode]
        public void SpendToken(int amount)
        {
            TokenStatic.SpendToken(amount);
        }
        
        [Button,HideInEditorMode]
        public void EarnToken(int amount)
        {
            TokenStatic.EarnToken(amount);
        }
        
        [Button,HideInEditorMode]
        public void EarnExp(int amount)
        {
            TokenStatic.EarnExp(amount);
        }

        private PopupDisplay _pop;
        [Button,HideInEditorMode]
        public void ShowTokenProgressPopup()
        {
            if (_pop)
            {
                return;
            }
            var pop =tokenProgressPopup.OpenPopup();
            _pop = pop;
            pop.PopupClosedAction += () =>
            {
                _pop = null;
            };
            if (pop.TryGetComponent<TokenProgressPopup>(out var tokenProgress))
            {
                tokenProgress.Build((() =>
                {
                    pop.ClosePopup();
                }),true);
            }
        }
        [Button,HideInEditorMode]
        public void ShowTokenCollectionPopup()
        {
            var pop = tokenCollectionPopup.OpenPopup();
            if (pop.TryGetComponent<TokenCollectionPopup>(out var tokenCollection))
            {
                tokenCollection.Build();
            }
        }

        #endregion
        
        
    }
}