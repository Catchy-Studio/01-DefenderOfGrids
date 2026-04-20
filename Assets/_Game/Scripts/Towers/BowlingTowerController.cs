using UnityEngine;

public class BowlingTowerController : MonoBehaviour
{
    [Header("References")]
    public BowlingTowerData towerData;
    public GameObject ballPrefab;
    public Transform firePoint;
    public LayerMask obstacleMask; // Set this to the layer of your walls/off-path areas

    private float fireTimer;
    private Vector2 bestDirection;

    private void Start()
    {
        // Find the longest path direction as soon as the tower is placed
        bestDirection = FindLongestPathDirection();
    }

    private void Update()
    {
        fireTimer -= Time.deltaTime;

        if (fireTimer <= 0f)
        {
            FireBall();
            fireTimer = towerData.fireCooldown;
        }
    }

    private void FireBall()
    {
        GameObject ball = Instantiate(ballPrefab, firePoint.position, Quaternion.identity);
        BowlingBall ballScript = ball.GetComponent<BowlingBall>();
        
        if (ballScript != null)
        {
            ballScript.Initialize(towerData, bestDirection);
        }
    }

    private Vector2 FindLongestPathDirection()
    {
        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        Vector2 bestDir = Vector2.up;
        float maxDistance = 0f;

        foreach (Vector2 dir in directions)
        {
            // Cast a ray in this direction to see how far it goes before hitting an obstacle
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 100f, obstacleMask);

            float distance = hit.collider != null ? hit.distance : 100f; // If it hits nothing, assume it's an open path

            if (distance > maxDistance)
            {
                maxDistance = distance;
                bestDir = dir;
            }
        }

        return bestDir;
    }
}