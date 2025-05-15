using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    public static event Action<Character> OnCharacterDeath;

    [Header("Character Properties")]
    [SerializeField] public int maxHealth = 100;
    [SerializeField] public int currentHealth;
    [SerializeField] protected int _maxActionPoint = 3;
    [SerializeField] protected int _actionAmount;
    [SerializeField] protected int _attackDamage = 10;
    [SerializeField] protected int _healingAmount = 20;
    public Character target;
    public bool isStun = false;
    public bool myTurn = false;

    [Header("Character UI")]
    [SerializeField] protected Image _healthBar;
    [SerializeField] protected TextMeshProUGUI _healthText;
    [SerializeField] protected TextMeshProUGUI _actionPointUI;
    [SerializeField] protected GameObject _stunUI;

    [Header("Animations")]
    public Animator animator;


    protected virtual void Awake()
    {
        currentHealth = maxHealth;
        _actionAmount = _maxActionPoint;

        UpdateActionUI();
    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth == 0)
        {
            Death();
        }
    }

    public virtual void GetStunned()
    {
        isStun = true;
        _stunUI.SetActive(true);
    }

    public virtual void RemoveStun()
    {
        isStun = false;
        _stunUI.SetActive(false);
    }

    public virtual bool UseAction(int actionAmount)
    {
        if (_actionAmount - actionAmount >= 0)
        {
            _actionAmount -= actionAmount;
            UpdateActionUI();
            return true;
        }
        return false;
    }

    public virtual void GainAction(int actionAmount)
    {
        _actionAmount += actionAmount;
        _actionAmount = Mathf.Clamp(_actionAmount, 0, _maxActionPoint);

        UpdateActionUI();
    }

    public virtual void ResetActionPoints()
    {
        _actionAmount = _maxActionPoint;
        _actionAmount = Mathf.Clamp(_actionAmount, 0, _maxActionPoint);

        UpdateActionUI();
    }

    public virtual void NewTurn()
    {
        GainAction(_maxActionPoint);
    }

    public virtual void Death()
    {
        OnCharacterDeath?.Invoke(this);
    }

    protected virtual void UpdateActionUI()
    {
        if (_actionPointUI != null)
            _actionPointUI.text = _actionAmount.ToString();
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }
}