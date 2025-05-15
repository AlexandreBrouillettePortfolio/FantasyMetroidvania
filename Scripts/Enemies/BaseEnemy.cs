using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] protected float detectionRange = 8f;
    [SerializeField] protected bool showDetectionZone = true;

    [Header("Combat")]
    [SerializeField] protected float attackCooldown = 1f;
    [SerializeField] protected float attackDuration = 0.5f;
    [SerializeField] protected int maxHealth = 100;
    protected int currentHealth;
    protected float lastAttackTime;
    protected bool isAttacking = false;
    protected float attackTimer = 0f;
    protected bool inCombat = false;

    [Header("Appearance")]
    [SerializeField] protected Color enemyColor = Color.red;
    protected SpriteRenderer spriteRenderer;

    protected Transform player;
    protected bool playerDetected;

    protected virtual void Start()
    {
        // Set tag for all enemies
        if (gameObject.tag != "Enemy")
        {
            gameObject.tag = "Enemy";
        }
        
        // Find player reference
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    protected virtual void Update()
    {
        // Si on est en combat, on arrête tout
        if (inCombat)
        {
            HandleCombatState();
            return;
        }

        // Si le joueur a été touché, on arrête tout
        if (EnemyGlobalState.Instance != null && EnemyGlobalState.Instance.IsPlayerHit)
        {
            HandlePlayerHit();
            return;
        }

        CheckPlayerDetection();
        
        if (playerDetected)
        {
            if (isAttacking)
            {
                UpdateAttackState();
            }
            else
            {
                HandleMovement();
                HandleAttack();
            }
        }
        else
        {
            HandleIdleState();
        }
    }

    protected virtual void HandleCombatState()
    {
        // Arrêter tout mouvement et attaque
        isAttacking = false;
        HandleIdleState();
    }

    public void SetInCombat(bool state)
    {
        inCombat = state;
        if (inCombat)
        {
            // Initialiser les points de vie uniquement quand on entre en combat
            currentHealth = maxHealth;
        }
    }

    public bool IsInCombat()
    {
        return inCombat;
    }

    protected virtual void HandlePlayerHit()
    {
        // Arrêter tout mouvement et attaque
        isAttacking = false;
        HandleIdleState();
    }

    protected virtual void CheckPlayerDetection()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        playerDetected = distanceToPlayer <= detectionRange;
    }

    protected virtual void UpdateAttackState()
    {
        attackTimer += Time.deltaTime;
        if (attackTimer >= attackDuration)
        {
            isAttacking = false;
        }
    }

    protected virtual void HandleMovement()
    {
        // Override in subclasses
    }

    protected virtual void HandleAttack()
    {
        // Override in subclasses
    }

    protected virtual void HandleIdleState()
    {
        // Override in subclasses
    }

    protected bool canAttack()
    {
        return Time.time >= lastAttackTime + attackCooldown;
    }

    protected virtual void StartAttack()
    {
        isAttacking = true;
        attackTimer = 0f;
        lastAttackTime = Time.time;
    }

    protected virtual void OnDrawGizmos()
    {
        if (!showDetectionZone) return;

        Gizmos.color = playerDetected ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    public virtual void TakeDamage(int damage)
    {
        // Ne prendre des dégâts que si on est en combat
        if (!inCombat) return;

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        // Base implementation - can be overridden by child classes
        Destroy(gameObject);
    }
} 