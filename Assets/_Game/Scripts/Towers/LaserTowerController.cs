using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserTowerController : MonoBehaviour
{
    [Header("References")]
    public LaserTowerData towerData;
    public Transform firePoint;     // The tip of the tower
    public LayerMask enemyMask;     // Set this to your Enemy layer
    
    private LineRenderer lineRenderer;
    private Transform currentTarget;
    private EnemyHealth targetHealthComponent; // Replace 'EnemyHealth' with your actual script name

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
    }

    private void Update()
    {
        if (currentTarget == null)
        {
            FindTarget();
        }
        else
        {
            float distanceToTarget = Vector2.Distance(transform.position, currentTarget.position);
            
            // Check if target moved out of range or was destroyed/deactivated
            if (distanceToTarget > towerData.range || !currentTarget.gameObject.activeInHierarchy)
            {
                ClearTarget();
            }
            else
            {
                UpdateLaserVisuals();
            }
        }
    }

    private void FindTarget()
    {
        // 2D Overlap check for enemies in range
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, towerData.range, enemyMask);

        if (hitColliders.Length > 0)
        {
            // Lock onto the first enemy found
            currentTarget = hitColliders[0].transform;
            targetHealthComponent = currentTarget.GetComponent<EnemyHealth>();

            if (targetHealthComponent != null)
            {
                // Apply the damage amplification
                targetHealthComponent.SetDamageMultiplier(towerData.damageMultiplier);
                lineRenderer.enabled = true;
            }
        }
    }

    private void UpdateLaserVisuals()
    {
        // Draw the line from the fire point to the enemy's center
        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, currentTarget.position);
    }

    private void ClearTarget()
    {
        if (targetHealthComponent != null)
        {
            // Remove the damage amplification
            targetHealthComponent.ResetDamageMultiplier();
        }
        
        currentTarget = null;
        targetHealthComponent = null;
        lineRenderer.enabled = false;
    }

    // Optional: Draw a circle in the editor so you can see the range
    private void OnDrawGizmosSelected()
    {
        if (towerData != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, towerData.range);
        }
    }
}