using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    [Header("Musiques")]
    public AudioClip splashMenuClip;     // Musique pour SplashScreen/MainMenu
    public AudioClip trapLevelClip1;     // 1re musique pour TrapLevel
    public AudioClip trapLevelClip2;     // 2e musique pour TrapLevel
    public AudioClip trapLevelClip3;     // 3e musique pour TrapLevel

    [Header("Fade")]
    public float fadeDuration = 1.5f;    // Durée du fondu
    [Range(0f, 1f)]
    public float maxVolume = 1f;         // Volume max

    private AudioSource audioSource;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        // Empêche la duplication de MusicManager (singleton)
        if (FindObjectsOfType<MusicManager>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        // Récupérer l'AudioSource
        audioSource = GetComponent<AudioSource>();

        // Si aucun clip n'est assigné, on met la musique du splash/menu
        if (audioSource.clip == null && splashMenuClip != null)
        {
            audioSource.clip = splashMenuClip;
            audioSource.volume = maxVolume;
            audioSource.Play();
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // SplashScreen ou MainMenu => Musique splashMenuClip
        if (scene.name == "SplashScreen" || scene.name == "MainMenu")
        {
            StartMusic(splashMenuClip);
        }
        // TrapLevel => on enchaîne 3 pistes
        else if (scene.name == "TrapLevel")
        {
            StartTrapLevelSequence();
        }
    }

    /// <summary>
    /// Lance la séquence des 3 musiques en TrapLevel.
    /// </summary>
    private void StartTrapLevelSequence()
    {
        // Si un fade est en cours, on l'arrête pour éviter les conflits
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(PlayTrapLevelTracks());
    }

    /// <summary>
    /// Joue successivement trapLevelClip1, trapLevelClip2 et trapLevelClip3,
    /// avec un fade-out et un fade-in entre chaque piste.
    /// </summary>
    private IEnumerator PlayTrapLevelTracks()
    {
        // (1) FADE OUT de la musique actuelle (splashMenuClip, par ex.)
        yield return StartCoroutine(FadeOut());

        // (2) Première piste TrapLevel
        audioSource.clip = trapLevelClip1;
        audioSource.Play();
        yield return StartCoroutine(FadeIn());

        // ? Ici, on attend que isPlaying soit false
        yield return new WaitWhile(() => audioSource.isPlaying);

        // On attend la fin de la piste
        yield return new WaitWhile(() => audioSource.isPlaying);

        // (3) On enchaîne avec la 2e piste
        yield return StartCoroutine(FadeOut());
        audioSource.clip = trapLevelClip2;
        audioSource.Play();
        yield return StartCoroutine(FadeIn());

        yield return new WaitWhile(() => audioSource.isPlaying);

        // (4) Enfin, la 3e piste
        yield return StartCoroutine(FadeOut());
        audioSource.clip = trapLevelClip3;
        audioSource.Play();
        yield return StartCoroutine(FadeIn());

        yield return new WaitWhile(() => audioSource.isPlaying);

        // Séquence terminée, on remet la variable à null
        fadeCoroutine = null;
    }

    /// <summary>
    /// Lance un fade-out + changement de clip + fade-in pour la musique
    /// (utilisé pour changer de musique d'une scène à l'autre).
    /// </summary>
    private void StartMusic(AudioClip newClip)
    {
        if (audioSource.clip == newClip)
            return;

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeMusic(newClip));
    }

    /// <summary>
    /// Joue un fade-out de la musique actuelle, change de clip, puis fade-in.
    /// </summary>
    private IEnumerator FadeMusic(AudioClip newClip)
    {
        // FADE OUT
        float startVolume = audioSource.volume;
        float timeElapsed = 0f;

        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            float t = timeElapsed / fadeDuration;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, t);
            yield return null;
        }

        audioSource.volume = 0f;

        // Nouveau clip
        audioSource.Stop();
        audioSource.clip = newClip;
        audioSource.Play();

        // FADE IN
        timeElapsed = 0f;
        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            float t = timeElapsed / fadeDuration;
            audioSource.volume = Mathf.Lerp(0f, maxVolume, t);
            yield return null;
        }

        audioSource.volume = maxVolume;
        fadeCoroutine = null;
    }

    /// <summary>
    /// FADE OUT simple (volume vers 0).
    /// </summary>
    private IEnumerator FadeOut()
    {
        float startVolume = audioSource.volume;
        float timeElapsed = 0f;

        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            float t = timeElapsed / fadeDuration;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, t);
            yield return null;
        }

        audioSource.volume = 0f;
    }

    /// <summary>
    /// FADE IN simple (volume 0 vers maxVolume).
    /// </summary>
    private IEnumerator FadeIn()
    {
        float timeElapsed = 0f;

        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            float t = timeElapsed / fadeDuration;
            audioSource.volume = Mathf.Lerp(0f, maxVolume, t);
            yield return null;
        }

        audioSource.volume = maxVolume;
    }
}




