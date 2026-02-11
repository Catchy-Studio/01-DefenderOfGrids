using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float _baseSpeed = 2f;

    private float _currentSpeed;
    private float _slowDuration = 0f;
    private Color _originalColor;
    private SpriteRenderer _spriteRenderer;

    private Transform[] _waypoints;
    private int _currentWaypointIndex = 0;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalColor = _spriteRenderer.color; // <--- REMEMBER: "I am Red"
    }

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

        _currentSpeed = _baseSpeed;

    }

    private void Update()
    {
        if (_waypoints == null || _waypoints.Length == 0) return;
        HandleDebuffs();
        Move();
    }

    private void HandleDebuffs()
    {
        if (_slowDuration > 0)
        {
            _slowDuration -= Time.deltaTime;

            // If the slow just expired, reset speed
            if (_slowDuration <= 0)
            {
                _currentSpeed = _baseSpeed;
                // Optional: Reset color here
                GetComponent<SpriteRenderer>().color = Color.white;

                _spriteRenderer.color = _originalColor;
            }
        }
    }

    public void ApplySlow(float slowFactor, float duration)
    {
        // slowFactor 0.5f means 50% speed
        _currentSpeed = _baseSpeed * slowFactor;
        _slowDuration = duration;

        // Visual Feedback (Turn Blue)
        GetComponent<SpriteRenderer>().color = Color.cyan;
    }

    private void Move()
    {
        if (_waypoints == null || _waypoints.Length == 0) return;
        // 1. Look at the target
        Transform targetWaypoint = _waypoints[_currentWaypointIndex];

        // 2. Move towards it
        // (Using Vector3.MoveTowards is cleaner than manual math)
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, _currentSpeed * Time.deltaTime);
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
        // Deal damage to the player
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TakeDamage(1); // 1 damage per enemy
        }

        Destroy(gameObject); // Enemy vanishes into the base
    }
}