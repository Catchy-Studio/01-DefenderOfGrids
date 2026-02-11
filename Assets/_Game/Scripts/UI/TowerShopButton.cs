using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerShopButton : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private TowerData _towerData; // Drag Tower_Archer here

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI _costText;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private Image _iconImage; // Optional: If you have icons

    private void Start()
    {
        // Auto-fill the text from the Data (So you don't have to type "50 Gold" manually)
        if (_towerData != null)
        {
            if (_costText != null) _costText.text = _towerData.cost.ToString();
            if (_nameText != null) _nameText.text = _towerData.towerName;
            // if (_iconImage != null) _iconImage.sprite = _towerData.icon; 
        }

        // Add the click listener automatically
        GetComponent<Button>().onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        // Tell the Manager to switch towers
        LevelGridManager.Instance.SelectTower(_towerData);
    }
}