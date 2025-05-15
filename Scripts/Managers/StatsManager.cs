using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class StatsManager : MonoBehaviour
{
    [SerializeField] private ExperienceManager experienceManager;
    [SerializeField] private EquipmentManager equipmentManager;
    
    [Header("Character Stats")]
    [SerializeField] private int baseHealth = 100;
    [SerializeField] private int baseMana = 50;
    [SerializeField] private int baseEndurance = 75;
    [SerializeField] private int pointsPerLevel = 5;

    [Header("Stat Scaling")]
    [SerializeField] private float healthPerPoint = 10f;
    [SerializeField] private float manaPerPoint = 5f;
    [SerializeField] private float endurancePerPoint = 7f;

    [HideInInspector] public int currentHealth;
    [HideInInspector] public int maxHealth;
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
    
    private int previousLevel = 0;

    private void Start()
    {
        experienceManager = GetComponent<ExperienceManager>();
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
        if (experienceManager != null && experienceManager.CurrentLevel > previousLevel)
        {
            equipmentManager.UnequipAllForLevelUp();
            int levelsGained = experienceManager.CurrentLevel - previousLevel;
            availablePoints += levelsGained * pointsPerLevel;
            previousLevel = experienceManager.CurrentLevel;
            ShowLevelUpPanel();
        }
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
        currentHealth = Mathf.RoundToInt(maxHealth * healthRatio);
        currentMana = Mathf.RoundToInt(maxMana * manaRatio);
        currentEndurance = Mathf.RoundToInt(maxEndurance * enduranceRatio);

        // 5) Clamp so we never go above new max
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        currentMana = Mathf.Min(currentMana, maxMana);
        currentEndurance = Mathf.Min(currentEndurance, maxEndurance);

        // 6) Optionally, update any UI, or call OnStatsUpdated
        UpdateStatTexts();  // if you have a method that refreshes the UI
        NotifyStatsUpdated();
        Debug.Log($"RecalculateStats: maxHealth={maxHealth}, maxMana={maxMana}, maxEndurance={maxEndurance}.");
    }
}