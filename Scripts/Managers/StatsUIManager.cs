using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StatsUIManager : MonoBehaviour
{
    [Header("Stat Bars")]
    [SerializeField] private Image healthBar;
    [SerializeField] private Image manaBar;
    [SerializeField] private Image enduranceBar;
    
    [Header("Stat Texts")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI manaText;
    [SerializeField] private TextMeshProUGUI enduranceText;
    [SerializeField] private GameObject _endScreen;
    
    
    private StatsManager statsManager;
    private ExperienceManager experienceManager;
    [SerializeField] private PlayerMovement _playerMovementScript;
    private bool _isDead = false;

    
    void Start()
    {
        statsManager = FindObjectOfType<StatsManager>();
        experienceManager = FindObjectOfType<ExperienceManager>();
    }
    
    void Update()
    {
        if(_isDead) return;
        
        if (statsManager != null)
        {
            UpdateHealthUI();
            UpdateManaUI();
            UpdateEnduranceUI();
        }
    }
    
    [Header("Animation Settings")]
    [SerializeField] private float barAnimationSpeed = 5f;
    
    private float targetHealthFill;
    private float targetManaFill;
    private float targetEnduranceFill;

    void UpdateHealthUI()
    {
        targetHealthFill = (float)statsManager.currentHealth / statsManager.maxHealth;

        if (healthBar != null)
        {
            healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, targetHealthFill, Time.deltaTime * barAnimationSpeed);
            
        }

        if (healthText != null)
        {
            healthText.text = $"{statsManager.currentHealth} / {statsManager.maxHealth}";
        }

        if(statsManager.currentHealth <= 0)
        {
            PlayerIsDead();
        }
    }
    
    void UpdateManaUI()
    {
        targetManaFill = (float)statsManager.currentMana / statsManager.maxMana;

        if (manaBar != null)
        {
            manaBar.fillAmount = Mathf.Lerp(manaBar.fillAmount, targetManaFill, Time.deltaTime * barAnimationSpeed);
        }

        if (manaText != null)
        {
            manaText.text = $"{statsManager.currentMana} / {statsManager.maxMana}";
        }
    }
    
    void UpdateEnduranceUI()
    {
        targetEnduranceFill = (float)statsManager.currentEndurance / statsManager.maxEndurance;
        
        if (enduranceBar != null)
        {
            enduranceBar.fillAmount = Mathf.Lerp(enduranceBar.fillAmount, targetEnduranceFill, Time.deltaTime * barAnimationSpeed);
        }
        
        if (enduranceText != null)
        {
            enduranceText.text = $"{statsManager.currentEndurance} / {statsManager.maxEndurance}";
        }
    }
    
    public void DecreaseHealth(int amount)
    {
        if (statsManager != null)
        {
            statsManager.ModifyHealth(-amount);
        }
    }
    
    public void DecreaseMana(int amount)
    {
        if (statsManager != null)
        {
            statsManager.ModifyMana(-amount);
        }
    }
    
    public void DecreaseEndurance(int amount)
    {
        if (statsManager != null)
        {
            statsManager.ModifyEndurance(-amount);
        }
    }

    public void PlayerIsDead()
    {   
        _isDead = true;
        _playerMovementScript.Death();
        _endScreen.SetActive(true);
        Invoke("ReturnToMenu", 3f);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }
    
}