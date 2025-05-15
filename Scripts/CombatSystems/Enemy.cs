using System.Collections;
using UnityEngine;

public class Enemy : Character
{
    [Header("UI")]
    public GameObject combatUI;
    public GameObject focusUI;
    [SerializeField] private float barAnimationSpeed = 5f;

    [Header("Animations")]
    [SerializeField] private float DelayWhenAttacking = 0.5f;

    [Header("When Killed")]
    public int experienceGain;

    private string uuidSpawn;

    protected override void Awake()
    {
        base.Awake();
    }

    void Update()
    {
        UpdateHealthUI();
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        if (animator != null)
        {
            animator.SetTrigger("Player_Hit");
        }
    }

    private IEnumerator HealRoutine()
    {
        Heal();
        yield return new WaitForSeconds(0.5f);
    }

    public override void GetStunned()
    {
        base.GetStunned();

        if (animator != null)
        {
            animator.SetTrigger("Player_Hit");
        }
    }

    public override void RemoveStun()
    {
        base.RemoveStun();
    }

    public override void GainAction(int actionAmount)
    {
        base.GainAction(actionAmount);
    }

    public override void NewTurn()
    {
        base.NewTurn();
    }

    public IEnumerator EnemyTurnRoutine()
    {
        yield return new WaitForSeconds(1f);

        if (isStun)
        {
            RemoveStun();
            yield break;
        }

        while (_actionAmount > 0)
        {
            if (target.currentHealth < target.maxHealth * 0.3f)
            {
                yield return StartCoroutine(AttackRoutine());
            }
            else if (currentHealth < maxHealth * 0.75f)
            {
                yield return StartCoroutine(HealRoutine());
            }
            else
            {
                yield return StartCoroutine(AttackRoutine());
            }

            yield return new WaitForSeconds(1f);
        }
    }

    public override void Death()
    {
        base.Death();
        combatUI.SetActive(false);
        animator.SetBool("DeadBool", true);
        animator.SetTrigger("IsDead");
        Destroy(gameObject, 1.5f);
    }

    public void UpdateHealthUI()
    {
        var targetHealthFill = (float)currentHealth / maxHealth;

        if (_healthBar != null)
        {
            _healthBar.fillAmount = Mathf.Lerp(_healthBar.fillAmount, targetHealthFill, Time.deltaTime * barAnimationSpeed);

        }

        if (_healthText != null)
        {
            _healthText.text = $"{currentHealth} / {maxHealth}";
        }
    }

    protected override void UpdateActionUI()
    {
        base.UpdateActionUI();
    }

    public void Heal()
    {
        if (currentHealth == maxHealth) return;

        if (UseAction(1))
        {
            animator.SetTrigger("IsHealing");
            currentHealth += _healingAmount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            UpdateHealthUI();
        }
    }

    public void AttackTarget()
    {
        if (target != null && UseAction(1))
        {
            if (animator != null)
            {
                animator.SetTrigger("IsAttacking");
            }

            Invoke("DamageEnemy", DelayWhenAttacking);
        }
    }

    private void DamageEnemy()
    {
        target.TakeDamage(_attackDamage);
    }

    private IEnumerator AttackRoutine()
    {
        AttackTarget();
        yield return new WaitForSeconds(0.5f);
    }

    public void StunTarget()
    {
        if (target != null && UseAction(1))
        {
            if (animator != null)
            {
                animator.SetTrigger("Player_Hit");
            }
            target.GetStunned();
        }
    }

    public void AddUuidSpawn(string uuid)
    {
        uuidSpawn = uuid;
    }

    public string GetUuidSpawn()
    {
        return uuidSpawn;
    }
}