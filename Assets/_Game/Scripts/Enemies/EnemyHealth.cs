using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
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
        // TODO: We will add Coin gain logic here in Phase 3
        Debug.Log("Enemy Died!");
        Destroy(gameObject);
    }
}