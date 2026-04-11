using __Project.Systems.NUpgradeSystem;
using _Game.Scripts.Data;
using _NueCore.NStatSystem;
using System;
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
    public GameObject BulletPrefab => _bulletPrefab;
    [SerializeField] private Transform _firePoint;
    public Transform FirePoint => _firePoint;
    [SerializeField] private LayerMask _enemyLayer; // We will select "Enemy" here
    public LayerMask EnemyLayer => _enemyLayer;
    public TowerData Data => data;
    [SerializeField] private Transform _weaponPart;
    public Transform WeaponPart => _weaponPart;
    public float FireRate => _fireRate;

    private ITowerAttackBehaviour _attackBehaviour;
    private Transform _target;
    public Transform Target => _target;
    private float _fireCountdown = 0f;
    public float GetTotalRange()
    {
        if (data == null) return _range;

        var t = _range;
        if (!Application.isPlaying) 
        {
            if (data.TowerType == TowerTypes.Aura) return data.aoeRadius;
            return t;
        }

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
        else if (towerType is TowerTypes.Sniper)
        {
            t += (t*UpgradeStatic.GetTotalStat(NStatEnum.SniperTower_Range)/100f);
        }
        else if (towerType is TowerTypes.Aura)
        {
            t = data.aoeRadius + (data.aoeRadius * UpgradeStatic.GetTotalStat(NStatEnum.AuraTower_Range) / 100f);
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
        // Sniper towers must never use the base controller targeting/rotation.
        // Prefer the concrete behaviour if present, then fall back to interface scan.
        if (data != null && data.TowerType == TowerTypes.Sniper && TryGetComponent<SniperTowerAttack>(out var sniper))
        {
            _attackBehaviour = sniper;
        }
        else
        {
            // Unity's GetComponent<T>() is not always reliable for interface types across versions.
            // Use MonoBehaviour scan so custom attack behaviours always take over.
            _attackBehaviour = FindAttackBehaviour();
            
            // If no attack behaviour is found, and this is NOT an Aura tower, attach the default BasicTowerAttack
            if (_attackBehaviour == null && (data == null || data.TowerType != TowerTypes.Aura))
            {
                _attackBehaviour = gameObject.AddComponent<BasicTowerAttack>();
            }
        }
        if (_attackBehaviour != null)
        {
            _attackBehaviour.Initialize(this);
        }
        InvokeRepeating(nameof(UpdateTarget), 0f, 0.5f);
    }

    private ITowerAttackBehaviour FindAttackBehaviour()
    {
        var behaviours = GetComponentsInChildren<MonoBehaviour>(); // Search children too
        for (var i = 0; i < behaviours.Length; i++)
        {
            if (behaviours[i] is ITowerAttackBehaviour attack)
            {
                return attack;
            }
        }
        return null;
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
        if (_attackBehaviour != null)
        {
            _attackBehaviour.Tick(Time.deltaTime);
        }
    }

    public float GetDamageBoost()
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
        else if (towerType is TowerTypes.Sniper)
        {
            d = UpgradeStatic.GetTotalStat(NStatEnum.SniperTower_Damage);
        }
        else if (towerType is TowerTypes.Aura)
        {
            d = UpgradeStatic.GetTotalStat(NStatEnum.AuraTower_Damage);
        }

        return d;
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