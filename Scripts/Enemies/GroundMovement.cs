using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GroundMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Detection Settings")]
    [SerializeField] private float detectionRadius = 7f;

    private Rigidbody2D rb;
    private Transform target;
    private bool isGrounded;
    private bool playerDetected = false;
    private Vector3 startPosition;
    public bool hasSetStartPosition = false;

    [Header("Movement Animation")]
    public Animator animator;

    [SerializeField] private Transform _spriteHolder;

    public void SetMoveSpeed(float newMoveSpeed)
    {
        if (newMoveSpeed == 0)
        {
            animator.SetBool("IsWalking", false);
        }

        moveSpeed = newMoveSpeed;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // RB
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        // Enregistrer la position de départ
        startPosition = transform.position;
        hasSetStartPosition = true;

        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            _spriteHolder = spriteRenderer.transform;
        }
    }

    public void Initialize(Transform targetTransform)
    {
        target = targetTransform;

        // Si la position de départ n'a pas été enregistrée, le faire maintenant
        if (!hasSetStartPosition)
        {
            startPosition = transform.position;
            hasSetStartPosition = true;
        }
    }

    public void SetStartPosition(Vector3 position)
    {
        startPosition = position;
        transform.position = position;
        hasSetStartPosition = true;
        Debug.Log($"[GroundMovement] Start position set to: {position}");
    }

    public void CheckGround()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
    }

    public void MoveTowardsTarget()
    {
        if (!playerDetected || target == null) return;

        if (moveSpeed > 0)
            animator.SetBool("IsWalking", true);
        else
            animator.SetBool("IsWalking", false);

        // Orienter le sprite vers le joueur
        float direction = target.position.x > transform.position.x ? 1f : -1f;

        Vector3 scale = _spriteHolder.localScale;
        scale.x = Mathf.Abs(scale.x) * (direction * -1);
        _spriteHolder.localScale = scale;

        Vector2 newPosition = rb.position + new Vector2(direction * moveSpeed * Time.fixedDeltaTime, 0);

        rb.MovePosition(newPosition);
    }

    public void ReturnToStart()
    {
        if (!hasSetStartPosition) return;

        // Calculer la direction vers la position de départ
        float direction = startPosition.x > transform.position.x ? 1f : -1f;
        Vector3 scale = _spriteHolder.localScale;
        scale.x = Mathf.Abs(scale.x) * (direction * -1);
        _spriteHolder.localScale = scale;

        // S'arrêter si près de la position de départ
        if (Mathf.Abs(transform.position.x - startPosition.x) < 0.1f)
        {
            animator.SetBool("IsWalking", false);
            rb.velocity = Vector2.zero;
            return;
        }

        Vector2 newPosition = rb.position + new Vector2(direction * moveSpeed * Time.fixedDeltaTime, 0);
        rb.MovePosition(newPosition);
    }

    public void FlipEnemyInTargetDirection(Transform target)
    {
        if (target == null)
            return;
        float direction = target.position.x > transform.position.x ? 1f : -1f;
        Vector3 scale = _spriteHolder.localScale;
        scale.x = Mathf.Abs(scale.x) * (direction * -1);
        _spriteHolder.localScale = scale;
    }

    public void StopMoving()
    {
        animator.SetBool("IsWalking", false);
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    private void DetectPlayer()
    {
        // All collider in a cicrle
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        playerDetected = false;

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                playerDetected = true;
                target = hit.transform;
                break;
            }
        }

        if (playerDetected)
        {
            animator.SetBool("IsWalking", true);
        } else {
            animator.SetBool("IsWalking", false);
        }
    }

    private void Update()
    {
        DetectPlayer();

        if (moveSpeed > 0)
            animator.SetBool("IsWalking", true);
        else
            animator.SetBool("IsWalking", false);
        if (playerDetected)
        {
            MoveTowardsTarget();
        }
        else if (hasSetStartPosition && Vector2.Distance(transform.position, startPosition) > 0.1f)
        {
            ReturnToStart();
        }
        else
        {
            StopMoving();
        }
    }

    // Protection contre le repositionnement à (0,0)
    private void LateUpdate()
    {
        // Si on détecte une position à (0,0) non autorisée
        if (transform.position.x == 0 && transform.position.y == 0 && hasSetStartPosition)
        {
            Debug.LogWarning($"[GroundMovement] Position reset to (0,0) detected! Restoring to: {startPosition}");
            transform.position = startPosition;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        if (hasSetStartPosition)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(startPosition, 0.5f);
        }
    }
}