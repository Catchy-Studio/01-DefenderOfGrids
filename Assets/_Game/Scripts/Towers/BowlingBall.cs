using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class BowlingBall : MonoBehaviour
{
    private BowlingTowerData data;
    private Rigidbody2D rb;
    private float maxDistance;
    private Vector2 startPosition;

    public void Initialize(BowlingTowerData towerData, Vector2 direction, float distance)
    {
        data = towerData;
        maxDistance = distance;
        startPosition = transform.position;
        
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction * data.ballSpeed;
    }

    private void Update()
    {
        // Check how far we've traveled from the start point
        float distanceTraveled = Vector2.Distance(startPosition, transform.position);
        
        if (distanceTraveled >= maxDistance)
        {
            Destroy(gameObject); // It reached the end of the road tile sequence
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyHealth enemy = collision.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(data.damage);
                enemy.ApplyStun(data.stunDuration); 
            }
        }
    }
}