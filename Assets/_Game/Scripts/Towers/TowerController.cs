using UnityEngine;

public class TowerController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float _range = 3f;
    [SerializeField] private float _fireRate = 1f;
    public int Level { get; private set; } = 1;
    public int BaseCost { get; set; }
    public string TowerName { get; set; }

    [Header("Setup")]
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private LayerMask _enemyLayer; // We will select "Enemy" here

    private Transform _target;
    private float _fireCountdown = 0f;

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
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, _range, _enemyLayer);

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

        if (nearestEnemy != null && shortestDistance <= _range)
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

    private void Shoot()
    {
        GameObject bulletGO = Instantiate(_bulletPrefab, _firePoint.position, Quaternion.identity);

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
        Gizmos.DrawWireSphere(transform.position, _range);
    }
}