
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("MainMenu")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsWindow;
    [SerializeField] private GameObject creditsPanel;

    [Header("Audio")]
    [SerializeField] private AudioSource clickAudioSource;
    [SerializeField] private AudioClip buttonClickClip;

    private void Start()
    {

    }

    public void StartGame()
    {

        StartCoroutine(LoadSceneAfterDelay(2f, "TrapLevel"));
    }

    private System.Collections.IEnumerator LoadSceneAfterDelay(float delay, string sceneName)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }

    public void LoadGame()
    {

        Debug.Log("Chargement de la sauvegarde...");

    }

    public void SettingsButton()
    {
        settingsWindow.SetActive(true);
    }

    public void CloseSettingsWindow()
    {
        settingsWindow.SetActive(false);
    }

    public void ShowCredits()
    {
        creditsPanel.SetActive(true);
    }

    public void HideCredits()
    {
        creditsPanel.SetActive(false);
    }


    public void QuitGame()
    {
        Application.Quit(); 
    }
}




