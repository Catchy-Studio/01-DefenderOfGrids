using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class BowlingTowerController : MonoBehaviour, IBuffable
{
    [Header("References")]
    public BowlingTowerData towerData;
    public GameObject ballPrefab;
    public Transform firePoint;

    [Header("Tilemap Setup")]
    public Tilemap levelTilemap;
    public TileBase[] roadTiles; // Assign your grey road tiles here in the Inspector!

    private float fireTimer;
    private Vector2 bestDirection;
    private float maxRollDistance;

    private float baseSpawnCooldown;
    private float currentSpawnCooldown;

    private void Start()
    {
        // If the tilemap wasn't assigned, find it automatically in the scene!
        if (levelTilemap == null)
        {
            levelTilemap = FindObjectOfType<Tilemap>();
        }

        FindLongestPath();

        baseSpawnCooldown = towerData.fireCooldown;
        currentSpawnCooldown = baseSpawnCooldown;

        // Notify listeners (e.g. SpeedBufferTower) that a new tower is ready
        TowerEvents.NotifyTowerBuilt(gameObject);
    }

    private void Update()
    {
        if (maxRollDistance <= 0) return; // Don't fire if no valid path was found

        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0f)
        {
            FireBall();
            fireTimer = currentSpawnCooldown;
        }
    }

    public void ApplySpeedBuff(float buffPercentage)
    {
        // Reduce the cooldown by the percentage (e.g., 3.0 * (1 - 0.15) = 2.55 seconds)
        currentSpawnCooldown = baseSpawnCooldown * (1f - buffPercentage);
    }

    public void RemoveSpeedBuff(float buffPercentage)
    {
        // Reset back to standard
        currentSpawnCooldown = baseSpawnCooldown;
    }

    private void FindLongestPath()
    {
        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        int maxTiles = 0;
        
        // Convert the tower's position to a Tilemap Grid coordinate
        Vector3Int startCell = levelTilemap.WorldToCell(firePoint.position);

        foreach (Vector2 dir in directions)
        {
            int distanceInTiles = 0;
            Vector3Int currentCell = startCell;
            
            // Define the grid step direction
            Vector3Int step = new Vector3Int(Mathf.RoundToInt(dir.x), Mathf.RoundToInt(dir.y), 0);

            while (true)
            {
                currentCell += step;
                TileBase tileAtCell = levelTilemap.GetTile(currentCell);

                // If a tile exists here AND it matches one of your assigned grey road tiles
                if (tileAtCell != null && roadTiles.Contains(tileAtCell))
                {
                    distanceInTiles++;
                }
                else
                {
                    break; // We hit a green grid tile or empty space
                }
            }

            if (distanceInTiles > maxTiles)
            {
                maxTiles = distanceInTiles;
                bestDirection = dir;
            }
        }

        // Convert the number of tiles back into actual Unity distance
        // (Assuming your tiles are 1x1 unit. If they are different, use levelTilemap.cellSize.x)
        maxRollDistance = maxTiles * levelTilemap.cellSize.x; 
    }

    private void FireBall()
    {
        GameObject ball = Instantiate(ballPrefab, firePoint.position, Quaternion.identity);
        BowlingBall ballScript = ball.GetComponent<BowlingBall>();
        
        if (ballScript != null)
        {
            // We pass the maximum distance to the ball
            ballScript.Initialize(towerData, bestDirection, maxRollDistance);
        }
    }
}