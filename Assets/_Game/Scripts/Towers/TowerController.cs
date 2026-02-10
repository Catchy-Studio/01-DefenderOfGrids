using UnityEngine;

public class TowerController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float _range = 3f;
    [SerializeField] private float _fireRate = 1f;

    [Header("Setup")]
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private LayerMask _enemyLayer; // We will select "Enemy" here

    private Transform _target;
    private float _fireCountdown = 0f;

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

    private void Shoot()
    {
        GameObject bulletGO = Instantiate(_bulletPrefab, transform.position, Quaternion.identity);
        Bullet bullet = bulletGO.GetComponent<Bullet>();

        if (bullet != null)
        {
            bullet.Seek(_target);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _range);
    }
}