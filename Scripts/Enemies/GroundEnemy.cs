using UnityEngine;

[RequireComponent(typeof(GroundMovement))]
[RequireComponent(typeof(MeleeAttack))]
public class GroundEnemy : BaseEnemy
{
    private GroundMovement movement;
    private MeleeAttack meleeAttack;

    protected void Awake()
    {
        movement = GetComponent<GroundMovement>();
        meleeAttack = GetComponent<MeleeAttack>();
    }

    protected override void Start()
    {
        base.Start();
        
        // Initialize components with player reference
        if (player != null)
        {
            movement.Initialize(player);
            meleeAttack.Initialize(player);
        }
    }

    protected override void HandleMovement()
    {
        movement.CheckGround();
        movement.MoveTowardsTarget();
    }

    protected override void HandleAttack()
    {
        if (!canAttack()) return;
        
        StartAttack();
        
        // 50% chance for each attack type
        if (Random.value > 0.5f)
        {
            meleeAttack.PerformFrontAttack();
        }
        else
        {
            meleeAttack.PerformCircularAttack();
        }
    }

    protected override void HandleIdleState()
    {
        movement.ReturnToStart();
        base.HandleIdleState();
    }
} 