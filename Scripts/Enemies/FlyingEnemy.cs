using UnityEngine;

[RequireComponent(typeof(FlyingMovement))]
[RequireComponent(typeof(ProjectileAttack))]
public class FlyingEnemy : BaseEnemy
{
    public FlyingMovement movement;
    protected ProjectileAttack projectileAttack;
    private bool positionLocked = false;
    private Vector3 lockedPosition;
    private Vector3 originalPosition;
    private bool hasInitialPosition = false;
    private bool initialized = false;

    protected void Awake()
    {
        Debug.Log($"[FlyingEnemy] Awake - Position: {transform.position}");
        movement = GetComponent<FlyingMovement>();
        projectileAttack = GetComponent<ProjectileAttack>();

        // Set a default purple color for flying enemies
        enemyColor = new Color(0.8f, 0.2f, 0.8f);

        // Capture original position
        originalPosition = transform.position;
        hasInitialPosition = true;

        // Disable movement until properly initialized
        if (movement != null)
        {
            movement.enabled = false;
        }
    }

    /// <summary>
    /// Locks the position of the flying enemy at a specific point
    /// </summary>
    public void LockPosition(Vector3 position)
    {
        Debug.Log($"[FlyingEnemy] LockPosition called with: {position}");

        // Always update position
        transform.position = position;
        lockedPosition = position;

        if (movement != null)
        {
            movement.SetStartPosition(position);
            movement.enabled = true; // Réactiver le mouvement
        }

        positionLocked = true;
        Debug.Log($"[FlyingEnemy] Position locked at: {position}");
    }

    protected override void Start()
    {
        base.Start();

        Debug.Log($"[FlyingEnemy] Start - Position: {transform.position}");

        // Initialize components with player reference
        if (player != null)
        {
            movement.Initialize(player);
            projectileAttack.Initialize(player);
        }

        // If position wasn't locked in generation, lock it at the original position
        if (!positionLocked && hasInitialPosition)
        {
            Debug.Log($"[FlyingEnemy] Locking position in Start to original: {originalPosition}");
            LockPosition(originalPosition);
        }

        // Marquer comme initialisé
        initialized = true;
    }

    // Force position to stay at locked position only during initialization
    void LateUpdate()
    {
        // Seulement avant l'initialisation complète
        if (!initialized)
        {
            // Si on détecte une position à (0,0) non autorisée
            if (transform.position.x == 0 && transform.position.y == 0 && positionLocked)
            {
                Debug.LogWarning($"[FlyingEnemy] Position reset to (0,0) detected! Restoring to: {lockedPosition}");
                transform.position = lockedPosition;
            }
        }
    }

    protected override void HandleMovement()
    {
        // Ne rien faire si pas initialisé
        if (!initialized) return;

        // Après initialisation, utiliser le système de mouvement normal
        movement.MoveTowardsTarget();
    }

    protected override void HandleAttack()
    {
        if (!canAttack() || !initialized) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer < projectileAttack.GetAttackRange())
        {
            StartAttack();

            // 50% chance for each projectile type
            if (Random.value > 0.5f)
            {
                projectileAttack.ShootBigProjectile();
            }
            else
            {
                projectileAttack.ShootHomingProjectile();
            }
        }
    }

    protected override void HandleIdleState()
    {
        if (!initialized) return;

        movement.ReturnToStart();
        base.HandleIdleState();
    }
}