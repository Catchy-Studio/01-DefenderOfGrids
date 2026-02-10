using System;
using _NueCore.AudioSystem;
using _NueCore.Common.ReactiveUtils;
using _NueCore.Common.Utility;
using _NueExtras.PopupSystem.PopupDataSub;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace _NueExtras.TokenSystem._TokenCollection
{
    public class TokenCollectionButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private N_TMP_Text tokenCountText;
        [SerializeField] private Transform warningRoot;
        [SerializeField] private PopupDataDisplay collectionPopupData;


        private PopupDisplay _pop;

        public Button Button => button;

        private void Start()
        {
            RBuss.OnEvent<TokenREvents.TokenValueChangedREvent>().Subscribe(ev =>
            {
                UpdateTokenWarning();
            }).AddTo(gameObject);
            
            Build();
        }

        public void UpdateTokenWarning()
        {
            var tokenCount =TokenStatic.GetAvailableToken();
            if (tokenCount>0)
                warningRoot.gameObject.SetActive(true);

            else
                warningRoot.gameObject.SetActive(false);
            tokenCountText.SetValue(tokenCount.ToString());
        }

        public void Build()
        {
            Button.onClick.AddListener((() =>
            {
                AudioStatic.PlayFx(DefaultAudioDataTypes.Click);

                if (_pop)
                    _pop.ClosePopup();
                _pop =collectionPopupData.OpenPopup();
                
            }));
            UpdateTokenWarning();
        }
        
    }
}