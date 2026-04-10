using System.Collections;
using UnityEngine;

public class AoETowerAttack : MonoBehaviour, ITowerAttackBehaviour
{
    private TowerController _owner;
    private Collider2D[] _results = new Collider2D[50]; // Pre-allocated buffer to store hits
    private float _tickInterval = 0.25f; // Check for enemies 4 times per second

    public void Initialize(TowerController owner)
    {
        _owner = owner;
        StartCoroutine(AuraDamageRoutine());
    }

    public void Tick(float deltaTime)
    {
        // Damage is applied via Coroutine at a controlled tick rate, avoiding Update() overhead as requested.
    }

    private IEnumerator AuraDamageRoutine()
    {
        while (true)
        {
            if (_owner == null) yield break;

            float currentRadius = _owner.GetTotalRange();
            float baseDamage = _owner.Data.damagePerSecond;
            float damageBoostPercent = _owner.GetDamageBoost();
            
            float totalDPS = baseDamage + (baseDamage * damageBoostPercent / 100f);
            float damageToApply = totalDPS * _tickInterval;

            // Strict Performance: OverlapCircleNonAlloc with cached buffer
            int count = Physics2D.OverlapCircleNonAlloc(transform.position, currentRadius, _results, _owner.EnemyLayer);

            for (int i = 0; i < count; i++)
            {
                if (_results[i].TryGetComponent<EnemyHealth>(out var health))
                {
                    health.TakeDamage(damageToApply);
                }
            }

            yield return new WaitForSeconds(_tickInterval);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_owner == null && TryGetComponent<TowerController>(out var c)) _owner = c;
        if (_owner == null) return;

        Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
        Gizmos.DrawWireSphere(transform.position, _owner.GetTotalRange());
    }
}
