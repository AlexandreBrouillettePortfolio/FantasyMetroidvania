using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsWindow;

    private bool isPaused = false;

    private int cursorPosition = 0;

    void Update()
    {
        if (Input.GetButtonDown(Buttons.Pause))
        {
            Debug.Log("PauseMenu");
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
        if (!isPaused) // Skip la 2e portion de la loop si le jeu n'est pas en pause
        {
            return;
        }

        if (Input.GetButtonDown(Buttons.Jump) && !Input.GetKeyDown(KeyCode.Space))
        {
            switch (cursorPosition)
            {
                case 0: ResumeGame(); break;   

                case 1: SettingsButton(); break;

                case 2: QuitToMainMenu(); break;
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        if (pausePanel != null)
            pausePanel.SetActive(true);

        Time.timeScale = 0f;
        Debug.Log("Pause");
    }

    public void ResumeGame()
    {
        isPaused = false;
        if (pausePanel != null)
            pausePanel.SetActive(false);

        Time.timeScale = 1f;
        Debug.Log("Resume");
    }


    public void SettingsButton()
    {
        settingsWindow.SetActive(true);
    }

    public void CloseSettingsWindow()
    {
        if (settingsWindow != null)
            settingsWindow.SetActive(false);
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void ChangeCursorPosition(int dir)
    {
        cursorPosition += dir;
        if (cursorPosition < 0) cursorPosition = 2;
        else if (cursorPosition > 2) cursorPosition = 0;
        switch (cursorPosition)
        {
            case 0: transform.Find("SelectionArrow").transform.position = new Vector2(-105.9f,-98.74f); break;

            case 1: transform.Find("SelectionArrow").transform.position = new Vector2(-105.9f, -149.6f); break;

            case 2: transform.Find("SelectionArrow").transform.position = new Vector2(-105.9f, -200.4f); break;
        }
    }
}


