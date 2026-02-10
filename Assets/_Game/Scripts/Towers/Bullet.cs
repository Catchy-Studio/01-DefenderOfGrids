using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _damage = 2f;

    private Transform _target;

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