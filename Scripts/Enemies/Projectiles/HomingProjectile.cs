using UnityEngine;

public class HomingProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float rotationSpeed = 200f;
    [SerializeField] private int damage = 15;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float knockbackForce = 3f;
    [SerializeField] private float homingDelay = 0.5f;

    private Transform target;
    private Rigidbody2D rb;
    private float currentDelay;
    private bool isHoming = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifetime);
    }

    public void Initialize(Transform targetTransform)
    {
        target = targetTransform;
        currentDelay = homingDelay;
    }

    private void FixedUpdate()
    {
        if (target == null) return;

        currentDelay -= Time.fixedDeltaTime;
        if (currentDelay <= 0)
        {
            isHoming = true;
        }

        if (isHoming)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            float rotateAmount = Vector3.Cross(direction, transform.right).z;
            rb.angularVelocity = -rotateAmount * rotationSpeed;
            rb.velocity = transform.right * speed;
        }
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