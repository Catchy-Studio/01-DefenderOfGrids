using System.Collections.Generic;
using _NueCore.Common.KeyValueDict;
using _NueCore.Common.Utility;
using _NueCore.SaveSystem;
using _NueExtras.PopupSystem.PopupDataSub;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _NueExtras.TokenSystem._TokenCollection
{
    public class TokenCollectionPopup : MonoBehaviour
    {
      
        [SerializeField,TabGroup("Collection")] private KeyValueDict<TokenCategory,TokenCollectionTab> tabDict;
        
        [SerializeField,TabGroup("Showroom")] private Button unlockRandomButton;
        [SerializeField,TabGroup("Showroom")] private PopupDataDisplay showroomPopupData;
        
        [SerializeField,TabGroup("Info")] private N_TMP_Text tokenText;
        [SerializeField,TabGroup("Info")] private N_TMP_Text stickerText;
        [SerializeField,TabGroup("Info")] private N_TMP_Text relicText;
        [SerializeField,TabGroup("Info")] private N_TMP_Text stickText;
        
        [SerializeField,TabGroup("Events")] private UnityEvent showRoomStartedEvent;
        [SerializeField,TabGroup("Events")] private UnityEvent showRoomFinishedEvent;
        
        #region Cache
        public List<TokenCollectionSlot> AllSlotList { get; private set; } = new List<TokenCollectionSlot>();

        public TokenCollectionCatalog Catalog => TokenStatic.Catalog;

        #endregion

        #region Setup

        private bool _isBuilt;
        private void Start()
        {
            Build();
        }

        public void Build()
        {
            if (_isBuilt)
            {
                return;
            }
            if (Catalog == null)
            {
                Debug.LogError("TokenCollectionCatalog is not assigned.");
                return;
            }

            _isBuilt = true;
            foreach (var kvp in tabDict)
            {
                var tab = kvp.Value;
                if (tab == null)
                    continue;
                tab.Build(kvp.Key,Catalog);
                foreach (var slot in tab.SlotList)
                    AllSlotList.Add(slot);
            }
            
            unlockRandomButton.onClick.AddListener(UnlockRandomToken);
            UpdatePopup();
        }
        #endregion

        #region Async
        private async UniTask StartShowroom(ITokenData data)
        {
            unlockRandomButton.interactable = false;
            showRoomStartedEvent?.Invoke();
            var pop =showroomPopupData.OpenPopup();
            if (pop.TryGetComponent<TokenShowroom>(out var showroom))
            {
                showroom.Build();
                await showroom.ShowAsync(data);
            }
            else
                await UniTask.Delay(100);
            showRoomFinishedEvent?.Invoke();
            UpdatePopup();
        }
        #endregion

        #region Methods
        private void UnlockRandomToken()
        {
            var data =TokenStatic.UnlockRandomTokenData();
            StartShowroom(data).AttachExternalCancellation(gameObject.GetCancellationTokenOnDestroy()).Forget();
        }
        private void UpdatePopup()
        {
           
            unlockRandomButton.interactable = CanUnlockToken();
            tokenText.Set("#count",TokenStatic.GetAvailableToken().ToString());
            
            // var unlockCount = Catalog.TokenList.Count(item => item is ProductDataSticker && TokenStatic.IsUnlocked(item));
            // var totalCount = Catalog.TokenList.Count(item => item is ProductDataSticker);
            // stickerText.SetCurrentMax(unlockCount, totalCount);
                        
            // unlockCount = Catalog.TokenList.Count(item => item is RelicData && TokenStatic.IsUnlocked(item));
            // totalCount = Catalog.TokenList.Count(item => item is RelicData);
            // relicText.SetCurrentMax(unlockCount, totalCount);
          
            stickText.SetCurrentMax(0,1);
            
            foreach (var kvp in tabDict)
            {
                var tab = kvp.Value;
                if (tab == null)
                    continue;

                tab.UpdateTab();
            }
        }
        private bool CanUnlockToken()
        {
            var save = NSaver.GetSaveData<TokenSave>();
            if (save.AvailableToken<=0)
            {
                return false;
            }
            var availableTokens = Catalog.GetAvailableTokens();
            if (availableTokens.Count<=0)
            {
                return false;
            }
            
            return true;
        }
        #endregion
    }
}