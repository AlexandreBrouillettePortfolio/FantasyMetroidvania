using UnityEngine;

public class ProjectileAttack : MonoBehaviour
{
    [Header("Attack Parameters")]
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private float projectileSpawnOffset = 1f;
    [SerializeField] private GameObject bigProjectilePrefab;
    [SerializeField] private GameObject homingProjectilePrefab;

    private Transform target;

    public void Initialize(Transform targetTransform)
    {
        target = targetTransform;
    }

    public void ShootBigProjectile()
    {
        if (target == null || bigProjectilePrefab == null) return;

        // Calculate direction to target
        Vector2 direction = (target.position - transform.position).normalized;
        Vector3 spawnPosition = transform.position + (Vector3)direction * projectileSpawnOffset;
        
        // Instantiate projectile
        GameObject projectileObj = Instantiate(bigProjectilePrefab, spawnPosition, Quaternion.identity);
        
        // Initialize projectile if it has the expected component
        BigProjectile projectile = projectileObj.GetComponent<BigProjectile>();
        if (projectile != null)
        {
            projectile.Initialize(direction);
            Debug.Log("Big projectile fired at player");
        }
    }

    public void ShootHomingProjectile()
    {
        if (target == null || homingProjectilePrefab == null) return;

        // Spawn projectile below enemy
        Vector3 spawnPosition = transform.position + Vector3.down * projectileSpawnOffset;
        GameObject projectile = Instantiate(homingProjectilePrefab, spawnPosition, Quaternion.identity);
        
        Debug.Log("Homing projectile fired");
    }

    public void ShootMultipleHomingProjectiles(int count, bool circular = false)
    {
        if (target == null || homingProjectilePrefab == null) return;

        if (circular)
        {
            // Shoot in a circular pattern
            for (int i = 0; i < count; i++)
            {
                float angle = (360f / count) * i * Mathf.Deg2Rad;
                Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
                Vector3 spawnPosition = transform.position + direction * projectileSpawnOffset;
                GameObject projectile = Instantiate(homingProjectilePrefab, spawnPosition, Quaternion.identity);
                Debug.Log($"Circular homing projectile {i+1}/{count} fired");
            }
        }
        else
        {
            // Shoot in predefined directions (up, down, towards player)
            Vector3 upPosition = transform.position + Vector3.up * projectileSpawnOffset;
            Instantiate(homingProjectilePrefab, upPosition, Quaternion.identity);
            Debug.Log("Homing projectile up fired");

            Vector3 downPosition = transform.position + Vector3.down * projectileSpawnOffset;
            Instantiate(homingProjectilePrefab, downPosition, Quaternion.identity);
            Debug.Log("Homing projectile down fired");

            if (count > 2)
            {
                Vector2 playerDirection = (target.position - transform.position).normalized;
                Vector3 sidePosition = transform.position + (Vector3)playerDirection * projectileSpawnOffset;
                Instantiate(homingProjectilePrefab, sidePosition, Quaternion.identity);
                Debug.Log("Homing projectile towards player fired");
            }
        }
    }

    public float GetAttackRange()
    {
        return attackRange;
    }
} 