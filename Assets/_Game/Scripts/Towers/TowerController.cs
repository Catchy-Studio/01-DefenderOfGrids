using __Project.Systems.NUpgradeSystem;
using _Game.Scripts.Data;
using _NueCore.NStatSystem;
using UnityEngine;

public class TowerController : MonoBehaviour
{
    [Header("Stats")] 
    [SerializeField] private TowerData data;
    [SerializeField] private float _range = 3f;
    [SerializeField] private float _fireRate = 1f;
    public int Level { get; private set; } = 1;
    public int BaseCost { get; set; }
    public string TowerName { get; set; }

    [Header("Setup")]
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private LayerMask _enemyLayer; // We will select "Enemy" here
    [SerializeField] private Transform _weaponPart;

    private Transform _target;
    private float _fireCountdown = 0f;
    public float GetTotalRange()
    {
        var t = _range;
        var towerType = data.TowerType;
        if (towerType is TowerTypes.Arrow)
        {
            t += (t*UpgradeStatic.GetTotalStat(NStatEnum.ArrowTower_Range)/100f);
        }
        else if (towerType is TowerTypes.Cannon)
        {
            t += (t*UpgradeStatic.GetTotalStat(NStatEnum.CannonTower_Range)/100f);
        }
        else if (towerType is TowerTypes.Ice)
        {
            t += (t*UpgradeStatic.GetTotalStat(NStatEnum.IceTower_Range)/100f);
        }
        return t;
    }
    
    
    
    public int GetUpgradeCost()
    {
        // Simple Math: Upgrade costs 100% of base cost * Level
        return BaseCost * Level;
    }

    public int GetSellValue()
    {
        // Refund 50% of what you spent
        return (int)(BaseCost * 0.5f);
    }

    private void Start()
    {
        InvokeRepeating(nameof(UpdateTarget), 0f, 0.5f);
    }

    private void UpdateTarget()
    {
        // Physics2D.OverlapCircle checks our specific LayerMask
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, GetTotalRange(), _enemyLayer);

        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (var enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestEnemy = enemy.gameObject;
            }
        }

        if (nearestEnemy != null && shortestDistance <= GetTotalRange())
        {
            _target = nearestEnemy.transform;
        }
        else
        {
            _target = null;
        }
    }

    private void Update()
    {
        if (_target == null) return;

        Vector3 direction = _target.position - _weaponPart.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        // Rotate ONLY the weapon part. If your sprite was drawn facing RIGHT, this works perfectly.
        // If it was drawn facing UP, you might need: angle - 90f
        _weaponPart.rotation = Quaternion.Euler(0f, 0f, angle);
        
        if (_fireCountdown <= 0f)
        {
            Shoot();
            _fireCountdown = 1f / _fireRate;
        }

        _fireCountdown -= Time.deltaTime;
        
        
    }

    /* OLD SHOOT
    private void Shoot()
    {
        GameObject bulletGO = Instantiate(_bulletPrefab, transform.position, Quaternion.identity);

        // Try to find ANY script that has a "Seek" method
        // Ideally we would use an Interface "IProjectile", but let's be fast.

        Bullet normalBullet = bulletGO.GetComponent<Bullet>();
        if (normalBullet != null)
        {
            normalBullet.Seek(_target);
            return;
        }

        ExplosiveBullet explosive = bulletGO.GetComponent<ExplosiveBullet>();
        if (explosive != null)
        {
            explosive.Seek(_target);
            return;
        }
    }*/

    private float GetDamageBoost()
    {
        var d = 0f;
        var towerType = data.TowerType;
        if (towerType is TowerTypes.Arrow)
        {
            d =UpgradeStatic.GetTotalStat(NStatEnum.ArrowTower_Damage);
        }
        else if (towerType is TowerTypes.Cannon)
        {
            d =UpgradeStatic.GetTotalStat(NStatEnum.CannonTower_Damage);
        }
        else if (towerType is TowerTypes.Ice)
        {
            d = UpgradeStatic.GetTotalStat(NStatEnum.IceTower_Damage);
        }

        return d;
    }
    private void Shoot()
    {
        GameObject bulletGO = Instantiate(_bulletPrefab, _firePoint.position, Quaternion.identity);
        if (bulletGO.TryGetComponent<Bullet>(out var bullet))
        {
            bullet.SetDamageBoost(GetDamageBoost());
        }
        // This line searches for ANY method named "Seek" on the bullet and calls it.
        // It works for Bullet, ExplosiveBullet, AND IceBullet automatically!
        bulletGO.SendMessage("Seek", _target, SendMessageOptions.DontRequireReceiver);
    }

    public void Upgrade()
    {
        Level++;

        // Boost Stats!
        _range += 0.5f;        // +0.5 Range
        _fireRate *= 1.2f;     // +20% Speed

        // Optional: Make it bigger to show it's stronger
        transform.localScale *= 1.1f;

        Debug.Log($"Tower Upgraded to Level {Level}!");
    }

    public void Sell()
    {
        Debug.Log("Tower Sold!");
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, GetTotalRange());
    }
}