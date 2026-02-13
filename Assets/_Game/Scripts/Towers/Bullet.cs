using __Project.Systems.NUpgradeSystem;
using _Game.Scripts.Data;
using _NueCore.NStatSystem;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _damage = 2f;

    private Transform _target;

    private float _damageBoostRate;
    
    public float GetTotalDamage(TowerTypes towerType, float baseDamage)
    {
        var d = baseDamage;
        d += (d * _damageBoostRate / 100f);

        return d;
    }


    public void SetDamageBoost(float bonusPercent)
    {
        _damageBoostRate = bonusPercent;
    }

    public void Seek(Transform target)
    {
        _target = target;
    }

    private void Update()
    {
        if (_target == null)
        {
            Destroy(gameObject); // Target disappeared, destroy bullet
            return;
        }

        // Move towards target
        Vector3 direction = _target.position - transform.position;
        float distanceThisFrame = _speed * Time.deltaTime;

        if (direction.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(direction.normalized * distanceThisFrame, Space.World);
    }

    private void HitTarget()
    {
        EnemyHealth health = _target.GetComponent<EnemyHealth>();
        if (health != null)
        {
            health.TakeDamage(_damage);
        }

        Destroy(gameObject); // Destroy bullet on impact
    }
}