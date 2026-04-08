using __Project.Systems.DamageNumberSystem;
using __Project.Systems.NUpgradeSystem;
using _Game.Scripts.Data;
using _NueCore.NStatSystem;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerShopButton : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private TowerData _towerData;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI _costText;
    [SerializeField] private TextMeshProUGUI _nameText;

    // Add this to change the button color
    private Image _buttonImage;
    private Color _defaultColor = Color.white;
    private Color _selectedColor = Color.green; // Or any color you like

    private void Start()
    {
        _buttonImage = GetComponent<Image>();
        _defaultColor = _buttonImage.color;

        if (_towerData != null)
        {
            if (_costText != null) _costText.text = _towerData.cost.ToString();
            if (_nameText != null) _nameText.text = _towerData.towerName;
        }

        GetComponent<Button>().onClick.AddListener(OnButtonClicked);

        // SUBSCRIBE to the event
        if (LevelGridManager.Instance != null)
        {
            LevelGridManager.Instance.OnTowerSelected += UpdateSelectionVisual;
        }
    }

    private void OnDestroy()
    {
        // ALWAYS Unsubscribe when object is destroyed to prevent errors
        if (LevelGridManager.Instance != null)
        {
            LevelGridManager.Instance.OnTowerSelected -= UpdateSelectionVisual;
        }
    }

    private void OnButtonClicked()
    {
        if (!CanSelectTower(out var reason))
        {
            var number =NumberStatic.GetDamageNumber(NumberTypes.Warning);
            number.enableLeftText = true;
            number.leftText = reason;
            number.transform.position = transform.position;
            return;
        }
        LevelGridManager.Instance.SelectTower(_towerData);
    }

    public bool CanSelectTower(out string reason)
    {
        reason = "";

        var towerType = _towerData.TowerType;
        if (towerType is TowerTypes.Arrow && !UpgradeStatic.HasStat(NStatEnum.Unlock_ArrowTower))
        {
            reason = "Unlock Arrow Tower to play!";
            return false;
        }
        if (towerType is TowerTypes.Cannon && !UpgradeStatic.HasStat(NStatEnum.Unlock_CannonTower))
        {
            reason = "Unlock Cannon Tower to play!";
            return false;
        }
        if (towerType is TowerTypes.Ice && !UpgradeStatic.HasStat(NStatEnum.Unlock_IceTower))
        {
            reason = "Unlock Ice Tower to play!";
            return false;
        }
        if (towerType is TowerTypes.Sniper && !UpgradeStatic.HasStat(NStatEnum.Unlock_SniperTower))
        {
            reason = "Unlock Sniper Tower to play!";
            return false;
        }
        
        return true;
    }

    private void UpdateSelectionVisual(TowerData selectedData)
    {
        // If the selected tower is ME, turn Green. Otherwise, turn White.
        if (selectedData == _towerData)
        {
            _buttonImage.color = _selectedColor;
        }
        else
        {
            _buttonImage.color = _defaultColor;
        }
    }
}