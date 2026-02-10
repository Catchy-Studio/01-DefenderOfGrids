using _NueCore.AudioSystem;
using _NueCore.Common.ReactiveUtils;
using _NueCore.FaderSystem;
using _NueCore.SaveSystem;
using _NueCore.SceneSystem;
using _NueExtras.StockSystem;
using UnityEngine;

namespace _NueExtras.MainMenuSystem
{
    public class MainMenuUIController : MonoBehaviour
    {
        [SerializeField] private MenuButton newGameButton;
        [SerializeField] private MenuButton continueButton;
        [SerializeField] private MenuButton collectionButton;
        [SerializeField] private MenuButton exitButton;
        [SerializeField] private MenuButton discordButton;

        #region Setup
        private void Awake()
        {
            newGameButton.Build(OnNewGameButtonClicked);
            continueButton.Build(OnContinueButtonClicked);
            exitButton.Build(OnExitButtonClicked);
            discordButton.Build(OnDiscordButtonClicked);
            collectionButton.Build(OnCollectionButtonClicked);
        }

        private void Start()
        {
            ShowButtons();
        }

        #endregion

        #region Methods

        private void OnCollectionButtonClicked()
        {
            AudioStatic.PlayFx(DefaultAudioDataTypes.Click);
            RBuss.Publish(new MainMenuREvents.CollectionButtonClickedREvent());
        }

        private void ShowButtons()
        {
            newGameButton.Show();
            if (SaveStatic.HasSave)
                continueButton.Show();
            
            exitButton.Show();
            discordButton.Show();
            collectionButton.Show();
        }

        private void HideButtons()
        {
            newGameButton.Hide();
            if (SaveStatic.HasSave)
                continueButton.Hide();
            
            exitButton.Hide();
            discordButton.Hide();
            collectionButton.Hide();
        }
        private void OnNewGameButtonClicked()
        {
            RBuss.Publish(new MainMenuREvents.PreNewGameButtonClickedREvent());

            NSaver.ResetSaveByType(SaveTypes.InGame);
            SaveStatic.SetHasSave(true);
            SaveStatic.SetActiveSaveIndex(0);
            StockStatic.ResetStocks();
            AudioStatic.PlayFx(DefaultAudioDataTypes.Click);
            RBuss.Publish(new MainMenuREvents.NewGameButtonClickedREvent());
            var faderParams = new NFader.FaderParams
            {
                fadeInDuration = 0.5f,
                fadeOutDuration = 0.5f,
                waitDuration = 0.5f
            };
            faderParams.fadeInFinishedAction += () =>
            {
                
            };
            SceneStatic.ChangeSceneAsyncWithFader(SceneStatic.UpgradeScene,faderParams);
        }
        private void OnContinueButtonClicked()
        {
            var faderParams = new NFader.FaderParams
            {
                fadeInDuration = 0.5f,
                fadeOutDuration = 0.5f,
                waitDuration = 0.5f
            };
            faderParams.fadeInFinishedAction += () =>
            {
                
            };
            AudioStatic.PlayFx(DefaultAudioDataTypes.Click);
            RBuss.Publish(new MainMenuREvents.ContinueButtonClickedREvent());
            SceneStatic.ChangeSceneAsyncWithFader(SceneStatic.UpgradeScene,faderParams);
        }
        private void OnExitButtonClicked()
        {
            AudioStatic.PlayFx(DefaultAudioDataTypes.Click);
            HideButtons();
            NSaver.SaveAll();
            Application.Quit();
        }
        private void OnDiscordButtonClicked()
        {
            AudioStatic.PlayFx(DefaultAudioDataTypes.Click);
           
        }
        #endregion
    }
}