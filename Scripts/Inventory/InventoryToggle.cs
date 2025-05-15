using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryToggle : MonoBehaviour
{
    public GameObject inventoryUI;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        if (inventoryUI != null)
        {
            // If active, turn it off; if inactive, turn it on
            inventoryUI.SetActive(!inventoryUI.activeSelf);
            GetComponent<InventoryManager>().RefreshPowerStones();
        }
    }

    public void CloseInventory()
    {
        if (inventoryUI != null)
        {
            inventoryUI.SetActive(false);
            Time.timeScale = 1f;
        }
    }
}
