using UnityEngine;
using UnityEngine.UI;      
using TMPro;             

public class SettingsWindow : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TMP_Dropdown commandsDropdown;

    private Vector2Int[] fixedResolutions = new Vector2Int[]
    {
        new Vector2Int(1920, 1080),
        new Vector2Int(1280, 720),
        new Vector2Int(1024, 768)
    };

    private void Start()
    {

        // Resolution
        InitializeResolutionDropdown();

        // Volume
        volumeSlider.value = AudioListener.volume;  
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);

        InitializeCommandsDropdown();
    }

    // ================== INITIALISATION RÉSOLUTION ================== //
    private void InitializeResolutionDropdown()
    {
   
        resolutionDropdown.ClearOptions();


        var options = new System.Collections.Generic.List<string>();
        foreach (var res in fixedResolutions)
        {
            options.Add($"{res.x} x {res.y}");
        }

   
        resolutionDropdown.AddOptions(options);

        // Valeur par defaut (1920x1080)
        resolutionDropdown.value = 0;
        resolutionDropdown.RefreshShownValue();

        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
    }

    // ====================== FULLSCREEN ====================== //

    public void SetFullScreen(bool isFullScreen)
    {
     
        int width = 1920;                    
        int height = 1080;                    

      
        FullScreenMode mode = isFullScreen
            ? FullScreenMode.ExclusiveFullScreen  
            : FullScreenMode.Windowed;

        Screen.SetResolution(width, height, mode);

    }



    private void OnResolutionChanged(int index)
    {
        // Resolution choisie
        Vector2Int chosenRes = fixedResolutions[index];
        // Application de la resolution
        Screen.SetResolution(chosenRes.x, chosenRes.y, Screen.fullScreen);

        Debug.Log($"New Resolution : {chosenRes.x} x {chosenRes.y}");
    }

    // ================== VOLUME ================== //

    private void OnVolumeChanged(float newVolume)
    {
        AudioListener.volume = newVolume;
        Debug.Log($" Volume : {newVolume}");
    }

    // ================== COMMANDS ================== //

    private void InitializeCommandsDropdown()
    {
        commandsDropdown.ClearOptions();

        var options = new System.Collections.Generic.List<string>()
    {
        "KEYBOARD",
        "GAMEPAD"
    };
        commandsDropdown.AddOptions(options);

        // Valeur par défaut (KEYBOARD)
        commandsDropdown.value = 0;
        commandsDropdown.RefreshShownValue();

        commandsDropdown.onValueChanged.AddListener(OnCommandsChanged);
    }

    private void OnCommandsChanged(int index)
    {
        switch (index)
        {
            case 0:
                Debug.Log("Choix commandes : KEYBOARD");
                // inputActions.bindingMask = InputBinding.MaskByGroup("AZERTY");
                break;
            case 1:
                Debug.Log("Choix commandes : GAMEPAD");
                // inputActions.bindingMask = InputBinding.MaskByGroup("GAMEPAD");
                break;
        }
    }

}


