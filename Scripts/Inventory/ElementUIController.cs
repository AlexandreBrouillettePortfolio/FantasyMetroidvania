using UnityEngine;
using UnityEngine.UI;

public class ElementUIController : MonoBehaviour
{
    public PlayerMovement player;
    public Image[] elementIcons;

    private int lastElement;
    void Update()
    {
        if (player == null) return;

        int currentElement = player.selectedElem - 1;

        if (currentElement != -2 && currentElement != lastElement)
        {
            UpdateUI(currentElement);
            lastElement = currentElement;
            Debug.Log("Selected Element: " + currentElement);
        }
        else if (currentElement == -1) 
        {
            HideUI();
        }
    }

    void UpdateUI(int currentElement)
    {
        for (int i = 0; i < elementIcons.Length; i++)
        {
            Color targetColor = elementIcons[i].color;
            targetColor.a = (i == currentElement) ? 1f : 0f; //can see selected stone
            elementIcons[i].color = targetColor;
        }
    }

    void HideUI()
    {
        for (int i = 0; i < elementIcons.Length; i++)
        {
            Color targetColor = elementIcons[i].color;
            targetColor.a = 0f; 
            elementIcons[i].color = targetColor;
        }
    }
}
