using System;
using System.Collections.Generic;
using System.Linq;
using _NueCore.AudioSystem;
using _NueCore.Common.NueLogger;
using _NueCore.Common.ReactiveUtils;
using _NueCore.Common.Utility;
using _NueCore.FaderSystem;
using _NueCore.SaveSystem;
using _NueCore.SceneSystem;
using _NueExtras.NLocalizationSystem._Utils;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace _NueCore.SettingsSystem
{
    public class SettingsUIController : MonoBehaviour
    {
        [SerializeField,TabGroup("Common")] private Button closeButton;
        [SerializeField,TabGroup("Common")] private Button openButton;
        [SerializeField,TabGroup("Common")] private Transform settingsPanelRoot;
        [SerializeField,TabGroup("Common")] private Transform inGamePanelRoot;
        [SerializeField,TabGroup("Common")] private Button abandonButton;
        [SerializeField,TabGroup("Common")] private Button returnToMainMenuButton;
        [SerializeField,TabGroup("Common")] private Button saveAndQuitButton;
        [SerializeField,TabGroup("Common")] private Button showTutorialButton;
        [SerializeField,TabGroup("Common")] private List<SettingsTabButton> tabButtonList = new List<SettingsTabButton>();
        [SerializeField,TabGroup("Notice")] private TMP_Text noticeVersionText;
        
        #region Cache

        private bool _isSettingsPanelOpened;
      
        #endregion

        #region Setup

        private void Awake()
        {
            SetVersion();
            showTutorialButton.onClick.AddListener(() =>
            {
                //TutorialStatic.ShowTutorial();
            });
            closeButton.onClick.AddListener(()=>
            {
                AudioStatic.PlayFx(DefaultAudioDataTypes.ClosePanel);
                SaveSettings();
                CloseSettings();
            });
            abandonButton.onClick.AddListener(() =>
            {
                AudioStatic.PlayFx(DefaultAudioDataTypes.ClosePanel);
                SaveSettings();
                CloseSettings();
                SaveStatic.SetHasSave(false);
                NSaver.ResetSaveByType(SaveTypes.InGame);
                RBuss.Publish(new SettingsREvents.AbandonButtonClickedREvent());
                SceneStatic.ChangeSceneAsyncWithFader(SceneStatic.LobbyScene,NFader.FaderParams.Default);
            });
            
            returnToMainMenuButton.onClick.AddListener(() =>
            {
                AudioStatic.PlayFx(DefaultAudioDataTypes.ClosePanel);
                SaveSettings();
                CloseSettings();
                RBuss.Publish(new SettingsREvents.ReturnToMainMenuButtonClickedREvent());
                SceneStatic.ChangeSceneAsyncWithFader(SceneStatic.LobbyScene,NFader.FaderParams.Default);
            });
            
            saveAndQuitButton.onClick.AddListener(() =>
            {
                AudioStatic.PlayFx(DefaultAudioDataTypes.ClosePanel);
                SaveSettings();
                CloseSettings();
                RBuss.Publish(new SettingsREvents.SaveAndQuitButtonClickedREvent());
                SceneStatic.ChangeSceneAsyncWithFader(SceneEnums.LobbyScene,NFader.FaderParams.Default);
            });
            openButton.onClick.AddListener(() =>
            {
                AudioStatic.PlayFx(DefaultAudioDataTypes.OpenPanel);
                OpenSettings();
            });
            openButton.gameObject.SetActive(true);

            foreach (var tabButton in tabButtonList)
            {
                tabButton.Deselect();
                tabButton.OnSelect.AddListener((() =>
                {
                    _activeTab = tabButton;
                    foreach (var button in tabButtonList)
                    {
                        if (button == tabButton)
                            continue;
                        button.Deselect();
                    }
                }));
            }
            tabButtonList[0].Select();
        }

        private SettingsTabButton _activeTab;
        
        private void SetVersion()
        {
            var versionText = "v" + Application.version;
            noticeVersionText.SetText(versionText);
        }

        #endregion

        #region Process

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_isSettingsPanelOpened)
                {
                    CloseSettings();
                    SaveSettings();
                }
                else
                {
                    OpenSettings();
                }
            }
        }

        #endregion

        #region Common Methods

        public void OpenSettings()
        {
            //ARIF Remove from asset
            openButton.gameObject.SetActive(false);
            _isSettingsPanelOpened = true;
            settingsPanelRoot.gameObject.SetActive(true);
            //InitVideoSettings();
            tabButtonList[0].Select();
            inGamePanelRoot.gameObject.SetActive(SceneStatic.CheckActiveScene(SceneStatic.GameScene));
            RBuss.Publish(new SettingsREvents.SettingsOpenedREvent());
        }
        public void CloseSettings()
        {
            openButton.gameObject.SetActive(true);
            _isSettingsPanelOpened = false;
            settingsPanelRoot.gameObject.SetActive(false);
            RBuss.Publish(new SettingsREvents.SettingsClosedREvent());
            SaveStatic.Save();
            RBuss.Publish(new SettingsREvents.SettingsChangedREvent());
        }
        
        #endregion

        #region Audio Settings
        [SerializeField,TabGroup("Settings","Audio")] private AudioMixer audioMixer;
        [SerializeField,TabGroup("Settings","Audio")] private SettingsSlider masterVolumeSlider;
        [SerializeField,TabGroup("Settings","Audio")] private SettingsSlider musicVolumeSlider;
        [SerializeField,TabGroup("Settings","Audio")] private SettingsSlider sfxVolumeSlider;
        
        private float _currentMasterVolume = 100;
        private float _currentMusicVolume = 100;
        private float _currentSfxVolume = 100;
        public void SetMasterVolume(float volume)
        {
            var convertedVolume = Mathf.InverseLerp(0, 100, volume);
            if (convertedVolume<0.0001)
                convertedVolume = 0.0001f;
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(convertedVolume) *20);
            _currentMasterVolume = volume;
        }
        
        public void SetMusicVolume(float volume)
        {
            var convertedVolume = Mathf.InverseLerp(0, 100, volume);
            if (convertedVolume<0.0001)
                convertedVolume = 0.0001f;
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(convertedVolume) *20);
            _currentMusicVolume = volume;
        }
        
        public void SetSfxVolume(float volume)
        {
            var convertedVolume = Mathf.InverseLerp(0, 100, volume);
            if (convertedVolume<0.0001)
                convertedVolume = 0.0001f;
            audioMixer.SetFloat("SfxVolume", Mathf.Log10(convertedVolume) *20);
            _currentSfxVolume = volume;
        }
        #endregion

        #region Video Settings
          
        [SerializeField,TabGroup("Settings","Video")] private TMP_Dropdown resolutionDropdown;
        [SerializeField,TabGroup("Settings","Video")] private TMP_Dropdown qualityDropdown;
        [SerializeField,TabGroup("Settings","Video")] private TMP_Dropdown refreshRateDropdown;
        [SerializeField,TabGroup("Settings","Video")] private List<double> recommendedRefreshRateList = new List<double>();
        [SerializeField,TabGroup("Settings","Video")] private List<Vector2> recommendedResolutionList = new List<Vector2>();

        [SerializeField,TabGroup("Settings","Video")] private Toggle fullscreenToggle;
        [SerializeField,TabGroup("Settings","Video")] private Toggle vsyncToggle;
        [SerializeField,TabGroup("Settings","Video")] private Toggle crtToggle;
        [SerializeField,TabGroup("Settings","Video")] private Button refreshSettingsButton;
        private List<Resolution> ResolutionList { get; set; } = new List<Resolution>();
        private List<RefreshRate> RefreshRateList { get; set; } = new List<RefreshRate>();

        private List<string> RefreshOptionTextList { get; set; } = new List<string>();
        private List<string> OptionTextList { get; set; } = new List<string>();
        private List<string> QualityTextList { get; set; } = new List<string>();

        private int _currentResolutionIndex =0;
        private int _currentRefreshRateIndex;

        private void InitVideoSettings()
        {
            refreshSettingsButton.onClick.AddListener((() =>
            {
                InitRefreshRateDropdown();
                InitResolutionDropdown();
                // _currentResolutionIndex = 0;
                // _currentRefreshRateIndex = 0;
                resolutionDropdown.value = _currentResolutionIndex;
                refreshRateDropdown.value = _currentRefreshRateIndex;
                
            }));
            InitRefreshRateDropdown();
            InitResolutionDropdown();
            InitQualityDropdown();
            vsyncToggle.onValueChanged.RemoveAllListeners();
            crtToggle.onValueChanged.RemoveAllListeners();
            vsyncToggle.onValueChanged.AddListener((arg0 =>
            {
                var saveData = NSaver.GetSaveData<SettingsSave>();
                saveData.isVSyncOn = arg0;
                if (AudioManager.Instance)
                {
                    AudioStatic.PlayFx(DefaultAudioDataTypes.Click);
                }
                QualitySettings.vSyncCount = arg0 ? 1 : 0;
                saveData.Save();
            }));
            crtToggle.onValueChanged.AddListener((arg0 =>
            {
                var saveData = NSaver.GetSaveData<SettingsSave>();
                saveData.isCrtOn = arg0;
                if (AudioManager.Instance)
                {
                    AudioStatic.PlayFx(DefaultAudioDataTypes.Click);
                }
                //CRT effect implementation needed
                saveData.Save();
            }));
        }
        
        private void InitRefreshRateDropdown()
        {
            RefreshOptionTextList.Clear();
            refreshRateDropdown.ClearOptions();
            var refreshRates = Screen.resolutions.Select(x => x.refreshRateRatio).Distinct().ToList();
            RefreshRateList.Clear();
            for (int i = 0; i < refreshRates.Count; i++)
            {
                var item = refreshRates[i];
                if (recommendedRefreshRateList.Count>0)
                    if (!recommendedRefreshRateList.Contains(item.value))
                        continue;
               
                RefreshRateList.Add(item);
            }
            for (int i = 0; i < RefreshRateList.Count; i++)
            {
                var option = RefreshRateList[i].value + "Hz";
                RefreshOptionTextList.Add(option);
                if (Math.Abs(RefreshRateList[i].value - Screen.currentResolution.refreshRateRatio.value) < 0.01f)
                    _currentRefreshRateIndex =i;
            }
            refreshRateDropdown.AddOptions(RefreshOptionTextList);
            refreshRateDropdown.RefreshShownValue();
            refreshRateDropdown.onValueChanged.RemoveAllListeners();
            refreshRateDropdown.onValueChanged.AddListener(ChooseRefresh);
         
        }

        private void ChooseRefresh(int index)
        {
            if (RefreshRateList.Count<=0)
                return;
            "RefreshRate".NLog(Color.magenta);
            var saveData = NSaver.GetSaveData<SettingsSave>();
            saveData.refreshRateIndex = index;
           
            if (index>=RefreshRateList.Count)
                index = RefreshRateList.Count - 1;
            if (index<0)
                index = 0;
            if (AudioManager.Instance)
            {
                AudioStatic.PlayFx(DefaultAudioDataTypes.Click);

            }
            _currentRefreshRateIndex = index;
            InitResolutionDropdown();
            resolutionDropdown.value = _currentResolutionIndex;
            //Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, Screen.fullScreen, RefreshRateList[index]);
            saveData.Save();
            refreshRateDropdown.RefreshShownValue();
        }

        private int GetCurrentRefreshRate()
        {
            if (RefreshRateList.Count<=0 || _currentRefreshRateIndex>=RefreshRateList.Count)
            {
                return 60;
            }
            return Mathf.RoundToInt((float)RefreshRateList[_currentRefreshRateIndex].value);
        }
        
        private void InitQualityDropdown()
        {
            qualityDropdown.ClearOptions();
            var allQualities = QualitySettings.names;
            for (int i = 0; i < allQualities.Length; i++)
            {
                var item = allQualities[i];
                QualityTextList.Add(item);
            }

            qualityDropdown.AddOptions(QualityTextList);
            qualityDropdown.RefreshShownValue();

            qualityDropdown.onValueChanged.AddListener(SetQuality);
         
        }
        
        private void InitResolutionDropdown()
        {
            OptionTextList.Clear();
            resolutionDropdown.ClearOptions();
            ResolutionList.Clear();
            var resolutions = Screen.resolutions.Where(x => Math.Abs(x.refreshRateRatio.value - GetCurrentRefreshRate()) < 0.01f).ToList();
            for (int i = 0; i < resolutions.Count; i++)
            {
                var item = resolutions[i];
                if (recommendedResolutionList.Count>0)
                    if (!recommendedResolutionList.Contains(new Vector2(item.width,item.height)))
                        continue;

                ResolutionList.Add(item);
            }
            
            ResolutionList =ResolutionList.OrderByDescending(x=>x.width).
                ThenByDescending(x=>x.height).ToList();
            for (int i = 0; i < ResolutionList.Count; i++)
            {
                var option = ResolutionList[i].width + " x " + ResolutionList[i].height;

                OptionTextList.Add(option);
                
                if (ResolutionList[i].width == Screen.currentResolution.width
                    && ResolutionList[i].height == Screen.currentResolution.height)
                    _currentResolutionIndex = i;
            }

           
            resolutionDropdown.AddOptions(OptionTextList);
            resolutionDropdown.RefreshShownValue();
            resolutionDropdown.onValueChanged.RemoveAllListeners();
            resolutionDropdown.onValueChanged.AddListener(SetResolution);
        }


        public void SetFullscreen(bool isFullscreen)
        {
            AudioStatic.PlayFx(DefaultAudioDataTypes.Click);
            Screen.fullScreen = isFullscreen;
        }
        public void SetResolution(int res)
        {
            if (ResolutionList.Count<=0)
            {
                return;
            }
            if (res>=ResolutionList.Count)
            {
                res = ResolutionList.Count - 1;
            }
            
            if (res<0)
            {
                res = 0;
            }

            _currentResolutionIndex = res;
            AudioStatic.PlayFx(DefaultAudioDataTypes.Click);

            Resolution resolution = ResolutionList[res];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            resolutionDropdown.RefreshShownValue();
        }
        public void SetTextureQuality(int textureIndex)
        {
            QualitySettings.globalTextureMipmapLimit = textureIndex;
            qualityDropdown.value = 6;
        }
        public void SetAntiAliasing(int aaIndex)
        {
            QualitySettings.antiAliasing = aaIndex;
            qualityDropdown.value = 6;
        }
        public void SetQuality(int qualityIndex)
        {
            AudioStatic.PlayFx(DefaultAudioDataTypes.Click);
            QualitySettings.SetQualityLevel(qualityIndex);
            qualityDropdown.value = qualityIndex;
        }
        #endregion

        #region Localization Settings

        [SerializeField] private NLocalSelector localSelector;
        [SerializeField] private Locale defaultLocale;
        [SerializeField] private List<Locale> activeLocaleList = new List<Locale>();

        private void InitLocal()
        {
            var save = NSaver.GetSaveData<SettingsSave>();
            if (StringHelper.IsNull(save.localeID))
            {
                save.localeID = defaultLocale.Identifier.Code;
                save.Save();
            }
            var localeID = save.localeID;
            if (!string.IsNullOrEmpty(localeID))
            {
                var locale = activeLocaleList.Find(x => x.Identifier.Code == localeID);
                if (locale != null)
                    LocalizationSettings.SelectedLocale = locale;
            }
            localSelector.Init(activeLocaleList);
        }

        #endregion

        #region Save
        public void LoadSettings()
        {
            InitVideoSettings();
            var saveData = NSaver.GetSaveData<SettingsSave>();
            qualityDropdown.value = saveData.qualityIndex >= 0
                ? saveData.qualityIndex
                : QualitySettings.GetQualityLevel();
            resolutionDropdown.value = saveData.resolutionIndex >= 0 && saveData.resolutionIndex < ResolutionList.Count
                ? saveData.resolutionIndex
                : _currentResolutionIndex;
            
            fullscreenToggle.isOn = saveData.isFullscreen;
            vsyncToggle.isOn = saveData.isVSyncOn;
            
            refreshRateDropdown.value =
                saveData.refreshRateIndex >= 0 && saveData.refreshRateIndex < RefreshRateList.Count
                    ? saveData.refreshRateIndex
                    : 0;

            SetFullscreen(saveData.isFullscreen);
            masterVolumeSlider.Build(saveData.masterVolume, SetMasterVolume);
            musicVolumeSlider.Build(saveData.musicVolume, SetMusicVolume);
            sfxVolumeSlider.Build(saveData.sfxVolume, SetSfxVolume);
            InitLocal();
        }

        public void StartSettings()
        {
            var saveData = NSaver.GetSaveData<SettingsSave>();
            SetMasterVolume(saveData.masterVolume);
            SetMusicVolume(saveData.musicVolume);
            SetSfxVolume(saveData.sfxVolume);
        }

        public void SaveSettings()
        {
            var saveData = NSaver.GetSaveData<SettingsSave>();
            saveData.qualityIndex = qualityDropdown.value;
            saveData.resolutionIndex = resolutionDropdown.value;
            saveData.isFullscreen = Screen.fullScreen;
            saveData.masterVolume = _currentMasterVolume;
            saveData.musicVolume = _currentMusicVolume;
            saveData.sfxVolume = _currentSfxVolume;
            saveData.Save();
        }
        #endregion
    }
}