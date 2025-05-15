using UnityEngine;

public class ennemiTerrestre : BaseEnemy
{
    [Header("Mouvement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Combat")]
    [SerializeField] private float frontAttackRange = 1.5f;
    [SerializeField] private float circularAttackRange = 2f;
    [SerializeField] private float chanceToAttack = 0.5f;

    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded;
 
    protected override void Start()
    {
        base.Start();
        
        // Ajouter le tag Enemy
        if (gameObject.tag != "Enemy")
        {
            gameObject.tag = "Enemy";
        }
        
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    protected override void Update()
    {
        base.Update();
        
        if (playerDetected)
        {
            if (isAttacking)
            {
                attackTimer += Time.deltaTime;
                if (attackTimer >= attackDuration)
                {
                    isAttacking = false;
                    if (animator != null)
                    {
                        animator.SetBool("IsAttacking", false);
                    }
                }
            }
            else
            {
                CheckGround();
                Movement();
                
                // Attaque aléatoire
                if (canAttack() && (Random.value < chanceToAttack))
                {
                    // 50% de chance pour chaque type d'attaque
                    if (Random.value > 0.5f)
                    {
                        Debug.Log("Attaque frontale aléatoire!");
                        FrontAttack();
                    }
                    else
                    {
                        Debug.Log("Attaque circulaire aléatoire!");
                        CircularAttack();
                    }
                }
            }
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            if (animator != null)
            {
                animator.SetBool("IsWalking", false);
            }
        }
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
    }

    private void Movement()
    {
        if (player == null) return;

        float direction = player.position.x > transform.position.x ? 1 : -1;
        Vector2 newPosition = rb.position + new Vector2(direction * moveSpeed * Time.fixedDeltaTime, 0);
        rb.MovePosition(newPosition);
        
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = direction < 0;
        }

        if (animator != null)
        {
            animator.SetBool("IsWalking", true);
        }
    }

    private void FrontAttack()
    {
        if (player == null) return;

        isAttacking = true;
        attackTimer = 0f;
        if (animator != null)
        {
            animator.SetBool("IsAttacking", true);
            animator.SetTrigger("Attack");
        }

        Debug.Log("Exécution de l'attaque frontale");
        lastAttackTime = Time.time;
        
        float direction = player.position.x > transform.position.x ? 1 : -1;
        Vector2 attackPosition = (Vector2)transform.position + Vector2.right * direction * frontAttackRange;
        
        GameObject attackVisual = new GameObject("FrontAttackVisual");
        attackVisual.transform.position = attackPosition;
        SpriteRenderer visualRenderer = attackVisual.AddComponent<SpriteRenderer>();
        
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, new Color(1, 0, 0, 0.5f));
        texture.Apply();
        visualRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1);
        attackVisual.transform.localScale = new Vector3(frontAttackRange, frontAttackRange, 1);
        
        Debug.DrawLine(transform.position, attackPosition, Color.red, 0.5f);
        
        Destroy(attackVisual, 0.2f);
        
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPosition, frontAttackRange);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Debug.Log("Front Attack hit player");
            }
        }
    }

    private void CircularAttack()
    {
        isAttacking = true;
        attackTimer = 0f;
        if (animator != null)
        {
            animator.SetBool("IsAttacking", true);
            animator.SetTrigger("Attack");
        }

        Debug.Log("Exécution de l'attaque circulaire");
        lastAttackTime = Time.time;
        
        GameObject attackVisual = new GameObject("CircularAttackVisual");
        attackVisual.transform.position = transform.position;
        SpriteRenderer visualRenderer = attackVisual.AddComponent<SpriteRenderer>();
        
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, new Color(1f, 0.5f, 0f, 0.7f));
        texture.Apply();
        visualRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1);
        attackVisual.transform.localScale = new Vector3(circularAttackRange * 2, circularAttackRange * 2, 1);
        
        visualRenderer.sortingOrder = 1;
        
        for (int i = 0; i < 36; i++)
        {
            float angle = i * 10 * Mathf.Deg2Rad;
            float nextAngle = (i + 1) * 10 * Mathf.Deg2Rad;
            Vector3 start = transform.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * circularAttackRange;
            Vector3 end = transform.position + new Vector3(Mathf.Cos(nextAngle), Mathf.Sin(nextAngle)) * circularAttackRange;
            Debug.DrawLine(start, end, Color.red, 0.5f);
        }
        
        Destroy(attackVisual, 0.3f);
        
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, circularAttackRange);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Debug.Log("Circular Attack hit player");
            }
        }
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }
    }

    protected override void Die()
    {
        if (animator != null)
        {
            animator.SetTrigger("Death");
        }
        base.Die();
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, Vector2.down * groundCheckDistance);
    }
}