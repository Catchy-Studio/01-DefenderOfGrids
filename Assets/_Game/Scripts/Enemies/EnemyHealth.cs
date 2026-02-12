using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int _goldReward = 10;
    [SerializeField] private float _maxHealth = 10f;
    private float _currentHealth;

    private void Start()
    {
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(float amount)
    {
        _currentHealth -= amount;

        Debug.Log($"{gameObject.name} took {amount} damage! HP left: {_currentHealth}");

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
            CurrencySystem.Instance.AddGold(_goldReward);
            GameManager.Instance.OnEnemyDestroyed();
        }
        Debug.Log("Enemy Died and gave gold!");
        Destroy(gameObject);
    }
}