using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TowerOptionsUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _panel;
    [SerializeField] private TextMeshProUGUI _infoText;
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private TextMeshProUGUI _upgradeText; // The text INSIDE the button
    [SerializeField] private Button _sellButton;
    [SerializeField] private TextMeshProUGUI _sellText;    // The text INSIDE the button
    [SerializeField] private Button _closeButton;

    private TowerController _selectedTower;

    private void Start()
    {
        // Hide on start
        _panel.SetActive(false);

        // Setup Buttons
        _upgradeButton.onClick.AddListener(OnUpgradeClicked);
        _sellButton.onClick.AddListener(OnSellClicked);
        _closeButton.onClick.AddListener(Hide);
    }

    public void Show(TowerController tower)
    {
        _selectedTower = tower;
        _panel.SetActive(true);
        UpdateUI();
    }

    public void Hide()
    {
        _selectedTower = null;
        _panel.SetActive(false);
    }

    private void UpdateUI()
    {
        if (_selectedTower == null) return;

        // Update Text
        _infoText.text = $"Lvl {_selectedTower.Level} {_selectedTower.TowerName}";

        // Calculate Costs
        int upgradeCost = _selectedTower.GetUpgradeCost();
        int sellValue = _selectedTower.GetSellValue();

        _upgradeText.text = $"Upgrade ({upgradeCost}g)";
        _sellText.text = $"Sell ({sellValue}g)";

        // Disable Upgrade button if we are broke
        _upgradeButton.interactable = CurrencySystem.Instance.CurrentGold >= upgradeCost;
    }

    private void OnUpgradeClicked()
    {
        if (_selectedTower == null) return;

        int cost = _selectedTower.GetUpgradeCost();

        if (CurrencySystem.Instance.TrySpendGold(cost))
        {
            _selectedTower.Upgrade();
            UpdateUI(); // Refresh the prices (next level is more expensive!)
        }
    }

    private void OnSellClicked()
    {
        if (_selectedTower == null) return;

        // Use the Manager to handle the logic cleanly
        LevelGridManager.Instance.SellTower(_selectedTower);

        // Close the menu
        Hide();
    }
}