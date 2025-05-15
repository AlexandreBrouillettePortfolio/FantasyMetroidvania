using UnityEngine;

public class BigProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 20;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float knockbackForce = 5f;

    private Vector2 direction;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifetime);
    }

    public void Initialize(Vector2 moveDirection)
    {
        direction = moveDirection.normalized;
        rb.velocity = direction * speed;

        // Rotate projectile to face movement direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage);

                // Apply knockback
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();
                if (playerMovement != null)
                {
                    playerMovement.OnPlayerHit(knockbackDirection * knockbackForce);
                }
            }
        }

        if (!collision.CompareTag("Enemy") && !collision.CompareTag("Projectile"))
        {
            Destroy(gameObject);
        }
    }
}