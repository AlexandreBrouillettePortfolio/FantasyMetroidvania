using UnityEngine;

public class FlyingMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float hoverAmplitude = 0.5f;
    [SerializeField] private float hoverFrequency = 2f;

    [Header("Detection Settings")]
    [SerializeField] private float detectionRadius = 10f;

    private Vector3 startPosition;
    private Transform target;
    public bool playerDetected = false;

    [Header("Movement Animation")]
    public Animator animator;

    private bool isInitialized = false;

    [SerializeField] private Transform _spriteHolder;

    private float lastXPosition = 0.0f;

    public void FlipEnemyInTargetDirection(Transform target)
    {
        if (target == null)
            return;
        float direction = target.position.x > transform.position.x ? 1f : -1f;
        Vector3 scale = _spriteHolder.localScale;
        scale.x = Mathf.Abs(scale.x) * (direction * -1);
        _spriteHolder.localScale = scale;
    }

    public void Awake()
    {
        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            _spriteHolder = spriteRenderer.transform;
        }
    }

    public void SetMoveSpeed(float newMoveSpeed)
    {
        moveSpeed = newMoveSpeed;
    }

    public void Initialize(Transform targetTransform)
    {
        target = targetTransform;

        // Ne pas réinitialiser la position de départ si déjà définie
        if (startPosition == Vector3.zero)
        {
            startPosition = transform.position;
            lastXPosition = startPosition.x;
        }

        isInitialized = true;
        Debug.Log($"[FlyingMovement] Initialized with target. Start position: {startPosition}");
    }

    public void SetStartPosition(Vector3 position)
    {
        startPosition = position;
        transform.position = position;
        Debug.Log($"[FlyingMovement] Start position set to: {position}");
    }

    private void Update()
    {
        if (!isInitialized) return;

        DetectPlayer();

        if (playerDetected && target != null)
        {
            animator.SetBool("IsWalking", true);
            MoveTowardsTarget();
        }
        else
        {
            ReturnToStart();
        }
    }

    private void DetectPlayer()
    {
        if (target == null) return;

        // Vérifier si le joueur est à portée
        float distanceToPlayer = Vector2.Distance(transform.position, target.position);
        playerDetected = distanceToPlayer <= detectionRadius;
    }

    public void MoveTowardsTarget()
    {
        if (target == null) return;

        // Hover + go to the player
        Vector3 baseY = new Vector3(0, Mathf.Sin(Time.time * hoverFrequency) * hoverAmplitude, 0);
        Vector3 targetPosition = new Vector3(target.position.x, transform.position.y, 0) + baseY;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Orienter le sprite vers le joueur
        FlipEnemyInTargetDirection(target);
    }

    public void ReturnToStart()
    {
        lastXPosition = transform.position.x;
        if (lastXPosition == transform.position.x)
        {
            animator.SetBool("IsWalking", false);
        } else {
            animator.SetBool("IsWalking", true);
        }
        // Get back to origin
        Vector3 hoverPosition = startPosition + Vector3.up * Mathf.Sin(Time.time * hoverFrequency) * hoverAmplitude;
        transform.position = Vector3.MoveTowards(transform.position, hoverPosition, moveSpeed * Time.deltaTime);

        if (moveSpeed > 0.0f)
        {
            float direction = startPosition.x > transform.position.x ? 1f : -1f;
            Vector3 scale = _spriteHolder.localScale;
            scale.x = Mathf.Abs(scale.x) * (direction * -1);
            _spriteHolder.localScale = scale;
        }

        // Update last X position
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
