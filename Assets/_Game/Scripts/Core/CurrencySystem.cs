using UnityEngine;
using TMPro; // Standard Unity UI text

public class CurrencySystem : MonoBehaviour
{
    public static CurrencySystem Instance { get; private set; }

    [SerializeField] private int _startingGold = 100;
    [SerializeField] private TextMeshProUGUI _goldText; // UI Reference

    public int CurrentGold { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;

        CurrentGold = _startingGold;
        UpdateUI();
    }

    public void AddGold(int amount)
    {
        CurrentGold += amount;
        UpdateUI();
    }

    public bool TrySpendGold(int amount)
    {
        if (CurrentGold >= amount)
        {
            CurrentGold -= amount;
            UpdateUI();
            return true;
        }

        Debug.Log("Not enough gold!");
        return false;
    }

    private void UpdateUI()
    {
        if (_goldText != null)
        {
            _goldText.text = $"Gold: {CurrentGold}";
        }
    }
}