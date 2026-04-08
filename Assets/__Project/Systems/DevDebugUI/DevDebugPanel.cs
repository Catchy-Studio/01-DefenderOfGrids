using __Project.Systems.RunSystem;
using _NueCore.SceneSystem;
using _NueExtras.StockSystem;
using UnityEngine;
using UnityEngine.UI;

namespace __Project.Systems.DevDebugUI
{
    /// <summary>
    /// Developer-only debug UI. Automatically no-ops in non-dev builds.
    /// </summary>
    public sealed class DevDebugPanel : MonoBehaviour
    {
        [Header("Enable")]
        [SerializeField] private bool startVisible = true;
        [SerializeField] private KeyCode toggleKey = KeyCode.BackQuote;

        [Header("Shortcuts")]
        [SerializeField] private KeyCode addCurrencyKey = KeyCode.C;
        [SerializeField] private KeyCode failKey = KeyCode.F;

        [Header("Upgrade Scene - Currencies")]
        [SerializeField] private StockTypes currencyA = StockTypes.Coin;
        [SerializeField] private int currencyAAmount = 100;
        [SerializeField] private StockTypes currencyB = StockTypes.Gem;
        [SerializeField] private int currencyBAmount = 10;

        [Header("Game Scene - Fail")]
        [SerializeField] private SceneEnums failTransitionTarget = SceneEnums.UpgradeScene;

        [Header("UI (optional overrides)")]
        [SerializeField] private Font uiFont;

        private GameObject _root;
        private Text _currencyLabel;
        private float _nextRefreshTime;

        private bool IsDev =>
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            true;
#else
            false;
#endif

        private void Awake()
        {
            if (!IsDev)
            {
                enabled = false;
                return;
            }

            BuildUi();
            SetVisible(startVisible);
        }

        private void Update()
        {
            if (!IsDev || _root == null) return;

            if (Input.GetKeyDown(toggleKey))
            {
                SetVisible(!_root.activeSelf);
            }

            if (!_root.activeSelf) return;

            if (Input.GetKeyDown(addCurrencyKey) && SceneStatic.CheckActiveScene(SceneStatic.UpgradeScene))
            {
                AddCurrency(currencyA, currencyAAmount);
                AddCurrency(currencyB, currencyBAmount);
            }

            if (Input.GetKeyDown(failKey) && SceneStatic.CheckActiveScene(SceneStatic.GameScene))
            {
                TriggerFailAndTransition();
            }

            if (Time.unscaledTime >= _nextRefreshTime)
            {
                _nextRefreshTime = Time.unscaledTime + 0.2f;
                RefreshCurrencyLabel();
            }
        }

        private void AddCurrency(StockTypes type, int amount)
        {
            if (amount == 0) return;
            StockStatic.IncreaseStock(type, amount);
        }

        private void TriggerFailAndTransition()
        {
            // Prefer the existing run-state transition flow when available.
            if (RunStatic.Temp != null)
            {
                RunStatic.Temp.SetState(RunState.Transition);
                return;
            }

            // Fallback: direct scene change.
            SceneStatic.ChangeScene(failTransitionTarget);
        }

        private void SetVisible(bool visible)
        {
            if (_root != null) _root.SetActive(visible);
        }

        private void RefreshCurrencyLabel()
        {
            if (_currencyLabel == null) return;

            var coin = StockStatic.GetStockRounded(StockTypes.Coin);
            var gem = StockStatic.GetStockRounded(StockTypes.Gem);
            var emerald = StockStatic.GetStockRounded(StockTypes.Emerald);

            _currencyLabel.text = $"Coin: {coin}   Gem: {gem}   Emerald: {emerald}";
        }

        private void BuildUi()
        {
            _root = new GameObject("DevDebugCanvas");
            _root.transform.SetParent(transform, false);

            var canvas = _root.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = short.MaxValue;

            _root.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            _root.AddComponent<GraphicRaycaster>();

            var panel = CreatePanel(_root.transform);

            _currencyLabel = CreateLabel(panel.transform, "CurrenciesLabel", "");
            RefreshCurrencyLabel();

            if (SceneStatic.CheckActiveScene(SceneStatic.UpgradeScene))
            {
                CreateButton(panel.transform, $"{currencyA}+{currencyAAmount}", () => AddCurrency(currencyA, currencyAAmount));
                CreateButton(panel.transform, $"{currencyB}+{currencyBAmount}", () => AddCurrency(currencyB, currencyBAmount));
            }
            else if (SceneStatic.CheckActiveScene(SceneStatic.GameScene))
            {
                CreateButton(panel.transform, "FAIL -> Upgrade Scene", TriggerFailAndTransition);
            }
            else
            {
                CreateLabel(panel.transform, "Hint", "Open Upgrade/Game Scene for actions.");
            }

            CreateLabel(panel.transform, "Shortcuts", $"Shortcuts: [{toggleKey}] toggle, [{addCurrencyKey}] add currency (Upgrade), [{failKey}] fail (Game)");
        }

        private GameObject CreatePanel(Transform parent)
        {
            var go = new GameObject("Panel");
            go.transform.SetParent(parent, false);

            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(12f, -12f);
            rect.sizeDelta = new Vector2(380f, 180f);

            var image = go.AddComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0.6f);

            var layout = go.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(10, 10, 10, 10);
            layout.spacing = 8f;
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlHeight = true;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;

            go.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            return go;
        }

        private Text CreateLabel(Transform parent, string name, string text)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);

            var uiText = go.AddComponent<Text>();
            uiText.font = uiFont != null ? uiFont : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            uiText.fontSize = 14;
            uiText.color = Color.white;
            uiText.text = text;

            var rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0f, 22f);
            return uiText;
        }

        private void CreateButton(Transform parent, string label, UnityEngine.Events.UnityAction onClick)
        {
            var buttonGo = new GameObject($"Btn_{label}");
            buttonGo.transform.SetParent(parent, false);

            var img = buttonGo.AddComponent<Image>();
            img.color = new Color(1f, 1f, 1f, 0.15f);

            var btn = buttonGo.AddComponent<Button>();
            btn.onClick.AddListener(onClick);

            var rect = buttonGo.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0f, 28f);

            var text = CreateLabel(buttonGo.transform, "Text", label);
            text.alignment = TextAnchor.MiddleCenter;

            var textRect = text.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
        }
    }
}

