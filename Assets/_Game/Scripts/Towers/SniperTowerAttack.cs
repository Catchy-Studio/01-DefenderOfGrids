using __Project.Systems.NUpgradeSystem;
using _NueCore.NStatSystem;
using UnityEngine;

public class SniperTowerAttack : MonoBehaviour, ITowerAttackBehaviour
{
    [Header("Metrics")]
    [SerializeField] private SniperTowerMetrics _metrics;

    [Header("Setup")]
    [SerializeField] private Transform _firePoint;
    [SerializeField] private Transform _weaponPart;
    [SerializeField] private LayerMask _enemyLayer;

    private TowerController _owner;
    private float _fireCooldown;
    private float _retargetCooldown;
    private Transform _target;

    public void Initialize(TowerController owner)
    {
        _owner = owner;
    }

    public void Tick(float deltaTime)
    {
        if (_metrics == null || _firePoint == null || _weaponPart == null) return;

        _retargetCooldown -= deltaTime;
        if (_retargetCooldown <= 0f)
        {
            _retargetCooldown = _metrics.retargetInterval;
            _target = AcquireTarget();
        }

        if (_target != null)
        {
            AimAt(_target.position);
        }

        _fireCooldown -= deltaTime;
        if (_fireCooldown <= 0f)
        {
            if (_target != null)
            {
                FireLine();
                _fireCooldown = 1f / Mathf.Max(0.01f, _metrics.fireRate);
            }
        }
    }

    private Transform AcquireTarget()
    {
        var range = GetTotalRange();
        var hits = Physics2D.OverlapCircleAll(transform.position, range, _enemyLayer);
        if (hits == null || hits.Length == 0) return null;

        var bestDist = float.PositiveInfinity;
        Transform best = null;

        for (var i = 0; i < hits.Length; i++)
        {
            var t = hits[i].transform;
            var d = Vector2.Distance(transform.position, t.position);
            if (d < bestDist)
            {
                bestDist = d;
                best = t;
            }
        }

        return best;
    }

    private void AimAt(Vector3 worldPosition)
    {
        var direction = worldPosition - _weaponPart.position;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _weaponPart.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private float GetTotalRange()
    {
        var r = _metrics.range;
        r += r * (UpgradeStatic.GetTotalStat(NStatEnum.SniperTower_Range) / 100f);
        return r;
    }

    private float GetTotalDamage()
    {
        var d = _metrics.damage;
        d += d * (UpgradeStatic.GetTotalStat(NStatEnum.SniperTower_Damage) / 100f);
        return d;
    }

    private void FireLine()
    {
        var origin = _firePoint.position;
        var dir = _weaponPart.right.normalized;
        var range = GetTotalRange();

        RaycastHit2D[] hits;
        if (_metrics.lineThickness > 0f)
        {
            hits = Physics2D.CircleCastAll(origin, _metrics.lineThickness, dir, range, _enemyLayer);
        }
        else
        {
            hits = Physics2D.RaycastAll(origin, dir, range, _enemyLayer);
        }

        if (hits == null || hits.Length == 0) return;

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        var remaining = Mathf.Max(1, _metrics.pierceCount);
        var damage = GetTotalDamage();

        for (var i = 0; i < hits.Length && remaining > 0; i++)
        {
            var health = hits[i].transform.GetComponent<EnemyHealth>();
            if (health == null) continue;

            health.TakeDamage(damage);
            remaining--;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_metrics == null || _firePoint == null || _weaponPart == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(_firePoint.position, _firePoint.position + _weaponPart.right.normalized * _metrics.range);
    }
}

