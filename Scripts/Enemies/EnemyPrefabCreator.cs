using UnityEngine;

public class EnemyPrefabCreator : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject groundEnemyPrefab;
    [SerializeField] private GameObject flyingEnemyPrefab;
    [SerializeField] private GameObject bossEnemyPrefab;

    [Header("Projectile Prefabs")]
    [SerializeField] private GameObject bigProjectilePrefab;
    [SerializeField] private GameObject homingProjectilePrefab;

    private void Awake()
    {
        CreateGroundEnemyPrefab();
        CreateFlyingEnemyPrefab();
        CreateBossEnemyPrefab();
    }

    private void CreateGroundEnemyPrefab()
    {
        GameObject enemy = new GameObject("GroundEnemy");
        
        // Add required components
        enemy.AddComponent<SpriteRenderer>();
        Rigidbody2D rb = enemy.AddComponent<Rigidbody2D>();
        BoxCollider2D collider = enemy.AddComponent<BoxCollider2D>();
        GroundMovement movement = enemy.AddComponent<GroundMovement>();
        MeleeAttack attack = enemy.AddComponent<MeleeAttack>();
        GroundEnemy enemyScript = enemy.AddComponent<GroundEnemy>();

        // Configure components
        rb.gravityScale = 1f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        collider.size = new Vector2(1f, 1f);
        
        // Set tag and layer
        enemy.tag = "Enemy";
        enemy.layer = LayerMask.NameToLayer("Enemy");

        // Save as prefab
        #if UNITY_EDITOR
        UnityEditor.PrefabUtility.SaveAsPrefabAsset(enemy, "Assets/Prefab/Enemies/GroundEnemy.prefab");
        DestroyImmediate(enemy);
        #endif
    }

    private void CreateFlyingEnemyPrefab()
    {
        GameObject enemy = new GameObject("FlyingEnemy");
        
        // Add required components
        SpriteRenderer renderer = enemy.AddComponent<SpriteRenderer>();
        Rigidbody2D rb = enemy.AddComponent<Rigidbody2D>();
        CircleCollider2D collider = enemy.AddComponent<CircleCollider2D>();
        FlyingMovement movement = enemy.AddComponent<FlyingMovement>();
        ProjectileAttack attack = enemy.AddComponent<ProjectileAttack>();
        FlyingEnemy enemyScript = enemy.AddComponent<FlyingEnemy>();

        // Configure components
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        collider.radius = 0.5f;
        renderer.color = new Color(0.8f, 0.2f, 0.8f);
        
        // Set tag and layer
        enemy.tag = "Enemy";
        enemy.layer = LayerMask.NameToLayer("Enemy");

        // Save as prefab
        #if UNITY_EDITOR
        UnityEditor.PrefabUtility.SaveAsPrefabAsset(enemy, "Assets/Prefab/Enemies/FlyingEnemy.prefab");
        DestroyImmediate(enemy);
        #endif
    }

    private void CreateBossEnemyPrefab()
    {
        GameObject enemy = new GameObject("BossEnemy");
        
        // Add required components
        SpriteRenderer renderer = enemy.AddComponent<SpriteRenderer>();
        Rigidbody2D rb = enemy.AddComponent<Rigidbody2D>();
        CircleCollider2D collider = enemy.AddComponent<CircleCollider2D>();
        FlyingMovement movement = enemy.AddComponent<FlyingMovement>();
        ProjectileAttack attack = enemy.AddComponent<ProjectileAttack>();
        BossEnemy enemyScript = enemy.AddComponent<BossEnemy>();

        // Configure components
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        collider.radius = 1.5f;
        renderer.color = new Color(0.5f, 0f, 0.5f);
        enemy.transform.localScale = new Vector3(3f, 3f, 1f);
        
        // Set tag and layer
        enemy.tag = "Enemy";
        enemy.layer = LayerMask.NameToLayer("Enemy");

        // Save as prefab
        #if UNITY_EDITOR
        UnityEditor.PrefabUtility.SaveAsPrefabAsset(enemy, "Assets/Prefab/Enemies/BossEnemy.prefab");
        DestroyImmediate(enemy);
        #endif
    }
} 