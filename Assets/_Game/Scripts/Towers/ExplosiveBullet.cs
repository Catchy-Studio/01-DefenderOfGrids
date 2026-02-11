using UnityEngine;

public class ExplosiveBullet : MonoBehaviour
{
    [SerializeField] private float _speed = 8f;
    [SerializeField] private GameObject _explosionPrefab; // <--- The explosion visual

    private Transform _target;
    private Vector3 _lastKnownPosition; // In case target dies while bullet is flying

    public void Seek(Transform target)
    {
        _target = target;
        _lastKnownPosition = target.position;
    }

    private void Update()
    {
        // If target exists, update destination. If not, fly to last known spot.
        Vector3 destination = _target != null ? _target.position : _lastKnownPosition;
        if (_target != null) _lastKnownPosition = _target.position;

        Vector3 dir = destination - transform.position;
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
        // Spawn the Explosion
        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);

        // Destroy the bullet
        Destroy(gameObject);
    }
}