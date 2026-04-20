using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BowlingBall : MonoBehaviour
{
    private BowlingTowerData data;
    private Rigidbody2D rb;

    public void Initialize(BowlingTowerData towerData, Vector2 direction)
    {
        data = towerData;
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction * data.ballSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Check if we hit an enemy
        if (collision.CompareTag("Enemy"))
        {
            EnemyHealth enemy = collision.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(data.damage);
                
                // You will need to add this Stun method to your Enemy script!
                enemy.ApplyStun(data.stunDuration); 
            }
        }
        // 2. Check if we hit a wall/corner (Make sure your walls/off-path tiles have a specific tag or layer)
        else if (collision.CompareTag("Wall") || collision.CompareTag("Obstacle"))
        {
            // The ball hit a corner or the end of the path
            Destroy(gameObject);
        }
    }
}