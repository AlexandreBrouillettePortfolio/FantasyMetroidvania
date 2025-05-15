using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISlotHandler : MonoBehaviour , IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Item item;
    public Image icon;
    public TextMeshProUGUI itemCountText;
    public InventoryManager inventoryManager;

    void Awake()
    {
        if (item != null)
        {
            icon.sprite = item.itemIcon;
            if(item.itemCount == 1) itemCountText.text = string.Empty;
            else itemCountText.text = item.itemCount.ToString();
        }
        else
        {
            itemCountText.text = string.Empty;
            icon.gameObject.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (item == null) { return; }

            MouseManager.instance.PickupFromStack(this);
            return;
        }

        MouseManager.instance.UpdateHeldItem(this);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
        {
            // If your item has a 'description' field:
            // TooltipManager.instance.ShowTooltip(item.description);

            // Or if you have a different property:
            TooltipManager.instance.ShowTooltip(item.itemID);
        }
    }

    // New: hide tooltip when mouse leaves
    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.instance.HideTooltip();
    }
} 
