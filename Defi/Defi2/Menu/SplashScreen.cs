using UnityEngine;
using UnityEngine.SceneManagement;


public class SplashScreen : MonoBehaviour
{
    private bool hasTriggered = false;

    void Update()
    {
        if (Input.anyKeyDown && !hasTriggered)
        {
            hasTriggered = true;
            StartCoroutine(LoadSceneAfterDelay(0.5f));
        }
    }

    private System.Collections.IEnumerator LoadSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("MainMenu");
    }
}
