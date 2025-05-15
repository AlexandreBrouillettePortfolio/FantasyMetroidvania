using UnityEngine;
using UnityEngine.UI;
using TMPro; // if you're using TextMeshPro

public class TooltipManager : MonoBehaviour
{
    // Singleton-style for easy access (like MouseManager)
    public static TooltipManager instance;

    [Header("Assign the Tooltip Panel (which contains text)")]
    public GameObject tooltipPanel;

    [Header("The text component for showing the description")]
    public TextMeshProUGUI descriptionTMP;
    // If using regular Text:
    // public Text descriptionText;

    private void Awake()
    {
        instance = this;
        HideTooltip();
    }

    /// <summary>
    /// Show the tooltip panel with the given description text.
    /// </summary>
    public void ShowTooltip(string description)
    {
        if (tooltipPanel == null) return;

        tooltipPanel.SetActive(true);

        if (descriptionTMP != null)
        {
            descriptionTMP.text = description;

        }
        // if using a standard Text UI: descriptionText.text = description;
    }

    /// <summary>
    /// Hide the tooltip panel.
    /// </summary>
    public void HideTooltip()
    {
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (tooltipPanel.activeSelf)
        {
            Vector2 mousePos = Input.mousePosition;
            // Optionally offset so it doesn't overlap the cursor
            mousePos.x += 50f;
            mousePos.y += 50f;

            // If your tooltip is on a separate Canvas, you may need to convert screen-to-UI space.
            // But if it's a Screen Space - Overlay canvas, you can set transform directly:

            tooltipPanel.transform.position = mousePos;
        }
    }
}