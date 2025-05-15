using UnityEngine;

public class ProjectilePrefabCreator : MonoBehaviour
{
    private void Awake()
    {
        CreateBigProjectilePrefab();
        CreateHomingProjectilePrefab();
    }

    private void CreateBigProjectilePrefab()
    {
        GameObject projectile = new GameObject("BigProjectile");
        
        // Add required components
        SpriteRenderer renderer = projectile.AddComponent<SpriteRenderer>();
        Rigidbody2D rb = projectile.AddComponent<Rigidbody2D>();
        CircleCollider2D collider = projectile.AddComponent<CircleCollider2D>();
        BigProjectile projectileScript = projectile.AddComponent<BigProjectile>();

        // Configure components
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        collider.radius = 0.25f;
        collider.isTrigger = true;
        renderer.color = Color.red;
        
        // Set tag and layer
        projectile.tag = "Projectile";
        projectile.layer = LayerMask.NameToLayer("Projectile");

        // Save as prefab
        #if UNITY_EDITOR
        UnityEditor.PrefabUtility.SaveAsPrefabAsset(projectile, "Assets/Prefab/Enemies/Projectiles/BigProjectile.prefab");
        DestroyImmediate(projectile);
        #endif
    }

    private void CreateHomingProjectilePrefab()
    {
        GameObject projectile = new GameObject("HomingProjectile");
        
        // Add required components
        SpriteRenderer renderer = projectile.AddComponent<SpriteRenderer>();
        Rigidbody2D rb = projectile.AddComponent<Rigidbody2D>();
        CircleCollider2D collider = projectile.AddComponent<CircleCollider2D>();
        HomingProjectile projectileScript = projectile.AddComponent<HomingProjectile>();

        // Configure components
        rb.gravityScale = 0f;
        collider.radius = 0.25f;
        collider.isTrigger = true;
        renderer.color = Color.magenta;
        
        // Set tag and layer
        projectile.tag = "Projectile";
        projectile.layer = LayerMask.NameToLayer("Projectile");

        // Save as prefab
        #if UNITY_EDITOR
        UnityEditor.PrefabUtility.SaveAsPrefabAsset(projectile, "Assets/Prefab/Enemies/Projectiles/HomingProjectile.prefab");
        DestroyImmediate(projectile);
        #endif
    }
} 