using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class Player : Character
{
    [Header("Important Stuff")]
    private bool _isDead = false;
    public GameObject characterUI;
    public TurnManager turnManager;
    private ExperienceManager experienceManager;
    [SerializeField] private EquipmentManager equipmentManager;
    [SerializeField] private PlayerMovement _playerMovementScript;

    [Header("Stat Bars")]
    [SerializeField] private Image manaBar;
    [SerializeField] private Image enduranceBar;

    [Header("Stat Texts")]
    [SerializeField] private TextMeshProUGUI manaText;
    [SerializeField] private TextMeshProUGUI enduranceText;
    [SerializeField] private GameObject _endScreen;

    [Header("Animation Settings")]
    [SerializeField] private float barAnimationSpeed = 5f;

    private float targetHealthFill;
    private float targetManaFill;
    private float targetEnduranceFill;

    [Header("Character Stats")]
    [SerializeField] private int baseHealth = 100;
    [SerializeField] private int baseMana = 50;
    [SerializeField] private int baseEndurance = 75;
    [SerializeField] private int pointsPerLevel = 5;
    [SerializeField] private int _healingCost = 10;
    [SerializeField] private int _stunCost = 15;

    [Header("Stat Scaling")]
    [SerializeField] private float healthPerPoint = 10f;
    [SerializeField] private float manaPerPoint = 5f;
    [SerializeField] private float endurancePerPoint = 7f;
    [HideInInspector] public int currentMana;
    [HideInInspector] public int maxMana;
    [HideInInspector] public int currentEndurance;
    [HideInInspector] public int maxEndurance;
    [HideInInspector] public int healthPoints = 0;
    [HideInInspector] public int manaPoints = 0;
    [HideInInspector] public int endurancePoints = 0;
    [HideInInspector] public int availablePoints = 0;


    [Header("Level Up UI")]
    [SerializeField] private GameObject levelUpPanel;
    [SerializeField] private TextMeshProUGUI availablePointsText;
    [SerializeField] private TextMeshProUGUI healthValueText;
    [SerializeField] private TextMeshProUGUI manaValueText;
    [SerializeField] private TextMeshProUGUI enduranceValueText;
    [SerializeField] private Button confirmButton;
    public event Action OnStatsUpdated;

    [Header("Endurance Regeneration")]
    [SerializeField] private float enduranceRegenAmount = 5f;
    private float enduranceRegenAccumulator = 0f;

    private int previousLevel = 0;

    private CombatTrigger combatTrigger;

    [HideInInspector] public bool isInCombat;
    [Header("Clips SFX")]
    private AudioSource _audioSource;
    public AudioClip attackSFX;
    public AudioClip healingSFX;
    public AudioClip stunSFX;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        experienceManager = GetComponent<ExperienceManager>();
        _audioSource = GetComponent<AudioSource>();

        if (experienceManager == null)
        {
            experienceManager = FindObjectOfType<ExperienceManager>();
            if (experienceManager == null)
            {
                Debug.LogError("ExperienceManager not found in the scene! Stats system will not function properly.");
                return;
            }
        }

        InitializeStats();
        UpdateStatTexts();

        if (levelUpPanel != null)
        {
            levelUpPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("Level Up Panel not assigned in the inspector!");
        }
    }

    private void Update()
    {
        if (isInCombat)
        {
            if (Input.GetButtonDown(Buttons.Fire1) && !Input.GetKeyDown(KeyCode.Mouse0)) //Controller Press Left Button
            {
                AttackTarget();
            }

            if (Input.GetButtonDown(Buttons.Fire2) && !Input.GetKeyDown(KeyCode.Mouse1)) //Controller Press Down Button
            {
               StunTarget();
            }

            if (Input.GetButtonDown(Buttons.Roll) && !Input.GetKeyDown(KeyCode.LeftShift)) //Controller Press Right Button
            {
                Heal();
            }

            if (Input.GetButtonDown(Buttons.SelectNextElem) && !Input.GetKeyDown(KeyCode.E)) //Controller Press Left Bumper
            {
                turnManager.SelectNextEnemy();
            }

            if (Input.GetButtonDown(Buttons.SelectPrevElem) && !Input.GetKeyDown(KeyCode.Q)) //Controller Press Right Bumper
            {
                turnManager.SelectPreviousEnemy();
            }

            if (Input.GetButtonDown(Buttons.Interact) && !Input.GetKeyDown(KeyCode.F)) //Controller Press Right Bumper
            {
                turnManager.ChangeTurn();
            }
        }

        if (levelUpPanel.activeSelf)
        {
            if (Input.GetButtonDown(Buttons.Fire1) && !Input.GetKeyDown(KeyCode.Mouse0)) //Controller Press Left Button
            {
                AddHealthPoint();
            }

            if (Input.GetButtonDown(Buttons.Fire2) && !Input.GetKeyDown(KeyCode.Mouse1)) //Controller Press Down Button
            {
                AddManaPoint();
            }

            if (Input.GetButtonDown(Buttons.Roll) && !Input.GetKeyDown(KeyCode.LeftShift)) //Controller Press Right Button
            {
                AddEndurancePoint();
            }

            if (Input.GetButtonDown(Buttons.Interact) && !Input.GetKeyDown(KeyCode.F)) //Controller Press Right Bumper
            {
                CloseLevelUpPanel();
            }
        }

        if (experienceManager != null && experienceManager.CurrentLevel > previousLevel)
        {
            equipmentManager.UnequipAllForLevelUp();
            int levelsGained = experienceManager.CurrentLevel - previousLevel;
            availablePoints += levelsGained * pointsPerLevel;
            previousLevel = experienceManager.CurrentLevel;
            StartCoroutine(DelayPanel(1.5f));
        }

        UpdateHealthUI();
        UpdateManaUI();
        UpdateEnduranceUI();
        RegenerateEndurance();
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        if (animator != null)
        {
            animator.SetTrigger("Player_Hit");
        }
    }

    public override void GetStunned()
    {
        base.GetStunned();
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

    public override void Death()
    {
        base.Death();

        if (animator != null)
            animator.SetTrigger("IsDead");

        if (characterUI != null)
            characterUI.SetActive(false);

        _isDead = true;
        _playerMovementScript.Death();
        _endScreen.SetActive(true);
        Invoke("ReturnToMenu", 3f);
    }

    public void Heal()
    {
        if (currentHealth == maxHealth) return;

        if (currentMana - _healingCost >= 0 && UseAction(1))
        {
            if (animator != null)
            {
                animator.SetTrigger("IsHealing");
            }

            _audioSource.PlayOneShot(healingSFX);
            ModifyHealth(_healingAmount);
            ModifyMana(-_healingCost);
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

            _audioSource.PlayOneShot(attackSFX);
            target.TakeDamage(_attackDamage);
        }
    }

    public void StunTarget()
    {
        if (target != null && currentMana - _stunCost >= 0 && UseAction(1))
        {
            if (animator != null)
            {
                animator.SetInteger("CurrElem", 3);
                animator.SetTrigger("IsElemAttacking");
            }
            ModifyMana(-_stunCost);
            _audioSource.PlayOneShot(stunSFX);
            target.GetStunned();
        }
    }

    protected override void UpdateActionUI()
    {
        base.UpdateActionUI();
    }

    private void InitializeStats()
    {
        maxHealth = baseHealth;
        currentHealth = maxHealth;

        maxMana = baseMana;
        currentMana = maxMana;

        maxEndurance = baseEndurance;
        currentEndurance = maxEndurance;

        if (experienceManager != null)
        {
            previousLevel = experienceManager.CurrentLevel;
        }
        else
        {
            previousLevel = 0;
        }
    }

    public void ShowLevelUpPanel()
    {
        Time.timeScale = 0f;
        levelUpPanel.SetActive(true);
        UpdateStatTexts();
    }

    IEnumerator DelayPanel(float duration)
    {
        Debug.Log($"Started at {Time.time}, waiting for {duration} seconds");
        yield return new WaitForSeconds(duration);
        Debug.Log($"Ended at {Time.time}");
        ShowLevelUpPanel();
    }


    public void CloseLevelUpPanel()
    {
        Time.timeScale = 1f;
        levelUpPanel.SetActive(false);
        equipmentManager.ReequipAllAfterLevelUp();
    }

    protected void NotifyStatsUpdated()
    {
        OnStatsUpdated?.Invoke();
    }
    public void AddHealthPoint()
    {
        if (availablePoints > 0)
        {
            healthPoints++;
            availablePoints--;
            UpdateStats();
            UpdateStatTexts();
            NotifyStatsUpdated();
        }
    }

    public void AddManaPoint()
    {
        if (availablePoints > 0)
        {
            manaPoints++;
            availablePoints--;
            UpdateStats();
            UpdateStatTexts();
            NotifyStatsUpdated();
        }
    }

    public void AddEndurancePoint()
    {
        if (availablePoints > 0)
        {
            endurancePoints++;
            availablePoints--;
            UpdateStats();
            UpdateStatTexts();
            NotifyStatsUpdated();
        }
    }

    private void UpdateStats()
    {
        int oldMaxHealth = maxHealth;
        int oldMaxMana = maxMana;
        int oldMaxEndurance = maxEndurance;
        float healthPercent = (float)currentHealth / (maxHealth > 0 ? maxHealth : 1);
        float manaPercent = (float)currentMana / (maxMana > 0 ? maxMana : 1);
        float endurancePercent = (float)currentEndurance / (maxEndurance > 0 ? maxEndurance : 1);

        maxHealth = baseHealth + Mathf.RoundToInt(healthPoints * healthPerPoint);
        maxMana = baseMana + Mathf.RoundToInt(manaPoints * manaPerPoint);
        maxEndurance = baseEndurance + Mathf.RoundToInt(endurancePoints * endurancePerPoint);

        int healthDiff = maxHealth - oldMaxHealth;
        currentHealth += healthDiff;
        int manaDiff = maxMana - oldMaxMana;
        currentMana += manaDiff;
        int enduranceDiff = maxEndurance - oldMaxEndurance;
        currentEndurance += enduranceDiff;
    }

    public void ModifyHealth(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
    }

    public void ModifyMana(int amount)
    {
        currentMana = Mathf.Clamp(currentMana + amount, 0, maxMana);
    }

    public void ModifyEndurance(int amount)
    {
        currentEndurance = Mathf.Clamp(currentEndurance + amount, 0, maxEndurance);
    }

    public void RestoreAllStats()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        currentEndurance = maxEndurance;
    }

    private void UpdateStatTexts()
    {
        availablePointsText.text = "Available Points: " + availablePoints;
        healthValueText.text = maxHealth.ToString();
        manaValueText.text = maxMana.ToString();
        enduranceValueText.text = maxEndurance.ToString();
        confirmButton.interactable = (availablePoints == 0);

    }

    public void RecalculateStatsEquipement()
    {
        // 1) Start from base + level points
        int newMaxHealth = baseHealth + Mathf.RoundToInt(healthPoints * healthPerPoint);
        int newMaxMana = baseMana + Mathf.RoundToInt(manaPoints * manaPerPoint);
        int newMaxEndurance = baseEndurance + Mathf.RoundToInt(endurancePoints * endurancePerPoint);

        // 2) Check each equipped item, add its bonuses
        if (equipmentManager.necklaceEquipped != null)
        {
            newMaxHealth += equipmentManager.necklaceEquipped.lifeBonus;
            newMaxMana += equipmentManager.necklaceEquipped.manaBonus;
            newMaxEndurance += equipmentManager.necklaceEquipped.staminaBonus;
        }
        if (equipmentManager.ring1Equipped != null)
        {
            newMaxHealth += equipmentManager.ring1Equipped.lifeBonus;
            newMaxMana += equipmentManager.ring1Equipped.manaBonus;
            newMaxEndurance += equipmentManager.ring1Equipped.staminaBonus;
        }
        if (equipmentManager.ring2Equipped != null)
        {
            newMaxHealth += equipmentManager.ring2Equipped.lifeBonus;
            newMaxMana += equipmentManager.ring2Equipped.manaBonus;
            newMaxEndurance += equipmentManager.ring2Equipped.staminaBonus;
        }
        if (equipmentManager.bootsEquipped != null)
        {
            newMaxHealth += equipmentManager.bootsEquipped.lifeBonus;
            newMaxMana += equipmentManager.bootsEquipped.manaBonus;
            newMaxEndurance += equipmentManager.bootsEquipped.staminaBonus;
            // If you want to do something with bootsEquipped.doubleJump, handle it here or elsewhere
        }

        // 3) Apply to max stats
        // Keep track of old ratio, so current health doesn't go out of whack
        float healthRatio = (float)currentHealth / (maxHealth > 0 ? maxHealth : 1);
        float manaRatio = (float)currentMana / (maxMana > 0 ? maxMana : 1);
        float enduranceRatio = (float)currentEndurance / (maxEndurance > 0 ? maxEndurance : 1);

        maxHealth = newMaxHealth;
        maxMana = newMaxMana;
        maxEndurance = newMaxEndurance;

        // 4) Adjust current stats based on the new max
        currentHealth = maxHealth;
        currentMana = maxMana;
        currentEndurance = maxEndurance;

        // 5) Clamp so we never go above new max
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        currentMana = Mathf.Min(currentMana, maxMana);
        currentEndurance = Mathf.Min(currentEndurance, maxEndurance);

        // 6) Optionally, update any UI, or call OnStatsUpdated
        UpdateStatTexts();  // if you have a method that refreshes the UI
        NotifyStatsUpdated();
        Debug.Log($"RecalculateStats: maxHealth={maxHealth}, maxMana={maxMana}, maxEndurance={maxEndurance}.");
    }

    public void UpdateHealthUI()
    {
        targetHealthFill = (float)currentHealth / maxHealth;

        if (_healthBar != null)
        {
            _healthBar.fillAmount = Mathf.Lerp(_healthBar.fillAmount, targetHealthFill, Time.deltaTime * barAnimationSpeed);

        }

        if (_healthText != null)
        {
            _healthText.text = $"{currentHealth} / {maxHealth}";
        }
    }

    void UpdateManaUI()
    {
        targetManaFill = (float)currentMana / maxMana;

        if (manaBar != null)
        {
            manaBar.fillAmount = Mathf.Lerp(manaBar.fillAmount, targetManaFill, Time.deltaTime * barAnimationSpeed);
        }

        if (manaText != null)
        {
            manaText.text = $"{currentMana} / {maxMana}";
        }
    }

    void UpdateEnduranceUI()
    {
        targetEnduranceFill = (float)currentEndurance / maxEndurance;

        if (enduranceBar != null)
        {
            enduranceBar.fillAmount = Mathf.Lerp(enduranceBar.fillAmount, targetEnduranceFill, Time.deltaTime * barAnimationSpeed);
        }

        if (enduranceText != null)
        {
            enduranceText.text = $"{currentEndurance} / {maxEndurance}";
        }
    }

    public void DecreaseMana(int amount)
    {
        ModifyMana(-amount);
    }

    public void DecreaseEndurance(int amount)
    {
        ModifyEndurance(-amount);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }

    private void RegenerateEndurance()
    {
        if (currentEndurance < maxEndurance)
        {
            enduranceRegenAccumulator += enduranceRegenAmount * Time.deltaTime;

            if (enduranceRegenAccumulator >= 1.0f)
            {
                int pointsToAdd = Mathf.FloorToInt(enduranceRegenAccumulator);

                enduranceRegenAccumulator -= pointsToAdd;
                ModifyEndurance(pointsToAdd);

                //Debug.Log("Regenerated " + pointsToAdd + " endurance points");
            }
        }
    }

    // ---------------------------------------------------- //
    //                  Combat with enemies                 //
    // ---------------------------------------------------- //

    public List<GameObject> GetEnemiesWithUUID(string uuidToSearch)
    {
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        List<GameObject> filteredEnemies = new List<GameObject>();

        foreach (GameObject enemy in allEnemies)
        {
            Enemy enemyScript = enemy.GetComponent<Enemy>();

            if (enemyScript != null && enemyScript.GetUuidSpawn() == uuidToSearch)
            {
                filteredEnemies.Add(enemy);
            }
        }

        return filteredEnemies;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            // Start the combat
            combatTrigger = GetComponent<CombatTrigger>();
            if (combatTrigger != null && enemy != null)
            {
                combatTrigger.StartCombatTrigger(enemy);
            }
        }
    }
    // ---------------------------------------------------- //
}