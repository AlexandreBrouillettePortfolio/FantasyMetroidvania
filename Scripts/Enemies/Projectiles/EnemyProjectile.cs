using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] protected float speed = 8f;
    [SerializeField] protected int damage = 10;

    protected Rigidbody2D rb;
    protected SpriteRenderer spriteRenderer;

    protected virtual void Awake()
    {
        // Ajouter et configurer le Rigidbody2D
        rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        // Ajouter le SpriteRenderer pour la forme colorée
        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();

        // Ajouter le collider circulaire
        CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
    }

    protected virtual void Start()
    {
        Destroy(gameObject, 5f);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        // Détruire au contact du joueur
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Projectile a touché le joueur! Dégâts: {damage}");
            Destroy(gameObject);
        }
        // Détruire au contact du sol
        else if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Debug.Log("Projectile a touché le sol");
            Destroy(gameObject);
        }
    }
}