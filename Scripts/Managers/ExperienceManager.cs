using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.PlayerLoop;


public class ExperienceManager : MonoBehaviour
{
    [Header("Experience")]
    [SerializeField] AnimationCurve experienceCurve;
    [SerializeField] private int currentLevel;
    [SerializeField] private int totalExperience;
    int previousLevelExperience;
    int nextLevelExperience;

    private int maxExperience = 500000;
    private int maxLevel = 50;
    
    [Header("Interface")]
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI experienceText;
    [SerializeField] Image experienceFiller;
    
    [Header("Level Up Event")]
    [SerializeField] private AudioClip levelUpSound;
    private AudioSource audioSource;
    private float targetFill;
    private bool lvlMaxReached = false;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && levelUpSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        UpdateLevel();
    }

    public void QuitGame()
        {
            Application.Quit();
        }
    
    void Update() 
    {
        UpdateInterface();
        
    }

    public void AddExperience(int amount)
    {
        if (totalExperience + amount < maxExperience)
        {
            totalExperience += amount;
            CheckForLevelUp();
            UpdateInterface();
        }
        else if (totalExperience + amount >= maxExperience && totalExperience < maxExperience)
        {
            totalExperience = maxExperience;
            UpdateInterfaceMaxLevel();
            lvlMaxReached = true;
        }
    }

    void CheckForLevelUp()
    {
        if (totalExperience >= nextLevelExperience)
        {
            PlayLevelUpEffects();
        }
        while (totalExperience >= nextLevelExperience)
        {
            currentLevel++;
            UpdateLevel();
        }
    }

    void UpdateLevel()
    {
        previousLevelExperience = (int)experienceCurve.Evaluate(currentLevel);
        nextLevelExperience = (int)experienceCurve.Evaluate(currentLevel + 1);
        UpdateInterface();
    }

    void UpdateInterface()
    {
        if (!lvlMaxReached){
        int start = totalExperience - previousLevelExperience;
        int end = nextLevelExperience - previousLevelExperience; 

        levelText.text = currentLevel.ToString();
        experienceText.text = start + " / " + end + " exp";
        targetFill = (float)start / end;
        experienceFiller.fillAmount = Mathf.Lerp(experienceFiller.fillAmount, targetFill, Time.deltaTime * 5f);
        }
    }

    void UpdateInterfaceMaxLevel()
    {
        levelText.text = maxLevel.ToString();
        experienceText.text = "Max Level";
        experienceFiller.fillAmount = 1;
    }
    
    public int CurrentLevel { get { return currentLevel; } }
    public int TotalExperience { get { return totalExperience; } }
    
    void PlayLevelUpEffects()
    {
        if (audioSource != null && levelUpSound != null)
        {
            audioSource.PlayOneShot(levelUpSound);
        }
    }
}
