using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float _baseSpeed = 2f;

    private float _currentSpeed;
    private float _slowDuration = 0f;
    
    // Stun variables
    private float _stunDuration = 0f;
    private bool _isStunned = false;

    private Color _originalColor;
    private SpriteRenderer _spriteRenderer;

    private Transform[] _waypoints;
    private int _currentWaypointIndex = 0;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalColor = _spriteRenderer.color;
    }

    public void Initialize(Transform[] path)
    {
        _waypoints = path;
        _currentWaypointIndex = 0;

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

        // Only move if not stunned
        if (!_isStunned)
        {
            Move();
        }
    }

    private void HandleDebuffs()
    {
        // Handle Stun first
        if (_isStunned)
        {
            _stunDuration -= Time.deltaTime;
            
            if (_stunDuration <= 0f)
            {
                _isStunned = false;
                _spriteRenderer.color = _originalColor;
            }
        }
        // Handle Slow
        else if (_slowDuration > 0)
        {
            _slowDuration -= Time.deltaTime;

            if (_slowDuration <= 0)
            {
                _currentSpeed = _baseSpeed;
                _spriteRenderer.color = _originalColor;
            }
        }
    }

    public void ApplySlow(float slowFactor, float duration)
    {
        _currentSpeed = _baseSpeed * slowFactor;
        _slowDuration = duration;
        GetComponent<SpriteRenderer>().color = Color.cyan;
    }

    public void ApplyStun(float duration)
    {
        if (!_isStunned)
        {
            _stunDuration = duration;
            _isStunned = true;
            
            // Visual Feedback (Turn Gray)
            _spriteRenderer.color = Color.gray; 
        }
    }

    private void Move()
    {
        if (_waypoints == null || _waypoints.Length == 0) return;

        Transform targetWaypoint = _waypoints[_currentWaypointIndex];

        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, _currentSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            _currentWaypointIndex++;

            if (_currentWaypointIndex >= _waypoints.Length)
            {
                ReachEndOfPath();
            }
        }
    }

    private void ReachEndOfPath()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TakeDamage(1);
            GameManager.Instance.OnEnemyDestroyed();
        }

        Destroy(gameObject);
    }
}