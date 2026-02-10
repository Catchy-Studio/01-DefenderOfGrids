using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float _speed = 2f;

    private Transform[] _waypoints;
    private int _currentWaypointIndex = 0;

    // We initialize this when we spawn the enemy
    public void Initialize(Transform[] path)
    {
        _waypoints = path;
        _currentWaypointIndex = 0;

        // Teleport to start immediately
        if (_waypoints.Length > 0)
        {
            transform.position = _waypoints[0].position;
        }
    }

    private void Update()
    {
        if (_waypoints == null || _waypoints.Length == 0) return;

        Move();
    }

    private void Move()
    {
        // 1. Look at the target
        Transform targetWaypoint = _waypoints[_currentWaypointIndex];

        // 2. Move towards it
        // (Using Vector3.MoveTowards is cleaner than manual math)
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, _speed * Time.deltaTime);

        // 3. Check if we reached it
        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            // Go to next waypoint
            _currentWaypointIndex++;

            // If we reached the end...
            if (_currentWaypointIndex >= _waypoints.Length)
            {
                ReachEndOfPath();
            }
        }
    }

    private void ReachEndOfPath()
    {
        // TODO: Deal damage to player here later
        Debug.Log("Enemy Reached the End!");
        Destroy(gameObject);
    }
}