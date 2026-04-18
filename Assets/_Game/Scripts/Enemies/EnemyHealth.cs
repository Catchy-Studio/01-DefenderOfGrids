using __Project.Systems.NUpgradeSystem;
using _NueCore.NStatSystem;
using _NueExtras.StockSystem;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int _goldReward = 10;
    [SerializeField] private float _maxHealth = 10f;
    private float _currentHealth;

    // 1. Multiplier variable
    private float incomingDamageMultiplier = 1f;

    private void Start()
    {
        _currentHealth = _maxHealth;
    }

    // 2. Merged TakeDamage function using the multiplier
    public void TakeDamage(float amount)
    {
        float finalDamage = amount * incomingDamageMultiplier;
        _currentHealth -= finalDamage;

        Debug.Log($"{gameObject.name} took {finalDamage} damage! HP left: {_currentHealth}");

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Access the Singleton to give money
        if (CurrencySystem.Instance != null)
        {
            var totalGoldReward = _goldReward;
            var bonusGoldReward = UpgradeStatic.GetTotalStat(NStatEnum.InGameGoldPerEnemy);
            totalGoldReward += Mathf.RoundToInt(_goldReward * bonusGoldReward/100f);
            CurrencySystem.Instance.AddGold(totalGoldReward);
            
            if (UpgradeStatic.TryGetTotalRoundedStat(NStatEnum.CoinPerEnemy,out var coin))
            {
                StockStatic.IncreaseStock(StockTypes.Coin,coin);
            }
            GameManager.Instance.OnEnemyDestroyed();
        }
        
        Debug.Log("Enemy Died and gave gold!");
        Destroy(gameObject);
    }

    // 3. Public methods so the laser can talk to the enemy
    public void SetDamageMultiplier(float multiplier)
    {
        incomingDamageMultiplier = multiplier;
    }

    public void ResetDamageMultiplier()
    {
        incomingDamageMultiplier = 1f;
    }
}