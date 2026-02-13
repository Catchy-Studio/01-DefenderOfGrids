using __Project.Systems.NUpgradeSystem;
using _NueCore.NStatSystem;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float _radius = 2f;
    [SerializeField] private float _damage = 5f;
    [SerializeField] private LayerMask _enemyLayer;

    
    
    private float _damageBoostRate;
    
    public float GetTotalDamage(float baseDamage)
    {
        var d = baseDamage;
        d += (d * _damageBoostRate / 100f);

        return d;
    }


    public void SetDamageBoost(float bonusPercent)
    {
        _damageBoostRate = bonusPercent;
    }

    
    private void Start()
    {
        SetDamageBoost(UpgradeStatic.GetTotalStat(NStatEnum.CannonTower_Damage));
        // 1. Find everyone in range
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _radius, _enemyLayer);

        // 2. Damage them
        foreach (var hit in hits)
        {
            EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(GetTotalDamage(_damage));
            }
        }

        // 3. Visuals (Optional: Screen shake, particles)
        // ...

        // 4. Clean up immediately (it's an instant explosion)
        Destroy(gameObject, 0.5f); // Delay slightly if you have a particle system attached
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}