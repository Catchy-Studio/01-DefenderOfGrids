using UnityEngine;

public class IceBullet : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _damage = 1f; // Low damage
    [SerializeField] private float _slowAmount = 0.5f; // 50% speed
    [SerializeField] private float _slowDuration = 2f; // Lasts 2 seconds

    private Transform _target;

    public void Seek(Transform target)
    {
        _target = target;
    }

    private void Update()
    {
        if (_target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = _target.position - transform.position;
        float distanceThisFrame = _speed * Time.deltaTime;

        if (dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }

    private void HitTarget()
    {
        // 1. Deal Damage
        EnemyHealth health = _target.GetComponent<EnemyHealth>();
        if (health != null)
        {
            health.TakeDamage(_damage);
        }

        // 2. Apply Slow
        EnemyMovement movement = _target.GetComponent<EnemyMovement>();
        if (movement != null)
        {
            movement.ApplySlow(_slowAmount, _slowDuration);
        }

        Destroy(gameObject);
    }
}