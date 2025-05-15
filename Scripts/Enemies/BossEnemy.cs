using UnityEngine;
using System.Collections;

public class BossEnemy : FlyingEnemy
{
    [Header("Boss Settings")]
    [SerializeField] private Vector3 bossScale = new Vector3(3f, 3f, 1f);
    [SerializeField] private float timeBetweenShots = 0.5f;
    [SerializeField] private int projectileCount = 3;
    private bool isShooting = false;

    protected new void Awake()
    {
        base.Awake();
        projectileAttack = GetComponent<ProjectileAttack>();
        
        // Override attack duration for boss
        attackDuration = 0.8f;
        
        // Set boss-specific color
        enemyColor = new Color(0.5f, 0f, 0.5f);
    }

    protected override void Start()
    {
        base.Start();
        transform.localScale = bossScale;
    }

    protected override void HandleAttack()
    {
        if (!canAttack() || isShooting) return;
 
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer < projectileAttack.GetAttackRange())
        {
            StartAttack();

            // 50% chance for each special attack type
            if (Random.value > 0.5f)
            {
                StartCoroutine(TripleBigProjectileRoutine());
            }
            else
            {
                projectileAttack.ShootMultipleHomingProjectiles(3);
            }
        }
    }

    private IEnumerator TripleBigProjectileRoutine()
    {
        isShooting = true;
        
        for (int i = 0; i < projectileCount; i++)
        {
            projectileAttack.ShootBigProjectile();
            yield return new WaitForSeconds(timeBetweenShots);
        }

        lastAttackTime = Time.time;
        isShooting = false;
    }
}