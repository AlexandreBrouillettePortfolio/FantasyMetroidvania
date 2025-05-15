using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//using static UnityEditor.Progress;


public class UIEquipmentSlotHandler : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    public InventoryManager inventoryManager; // drag your manager here
    public Image placeholderImage;            // child image for empty slot
    public Image equippedImage;               // child image for equipped icon

    public ItemType equipementSlotType;

    // The item currently equipped in this slot
    public Item equippedItem;


    void Awake()
    {
        RefreshUI();
    }

    // Called when the user clicks on this slot
    public void OnPointerClick(PointerEventData eventData)
    {
        // 1) If we have an item in the mouse (heldItem), try to equip it
        if (MouseManager.instance.heldItem != null)
        {
            Item itemToEquip = MouseManager.instance.heldItem;
            bool validItem = false;

            if ((equipementSlotType == ItemType.Necklace && itemToEquip is NecklaceItem) ||
                (equipementSlotType == ItemType.Ring && itemToEquip is RingItem) ||
                (equipementSlotType == ItemType.Boot && itemToEquip is BootsItem))
            {
                validItem = true;
            }

            if (!validItem)
            {
                Debug.LogWarning("Item class doesn't match this slot's type!");
                return;
            }


            // Remove it from the mouse
            MouseManager.instance.heldItem = null;

            if (equippedItem != null)
            {
                // Put that item in the mouse
                MouseManager.instance.heldItem = equippedItem;
                inventoryManager.UnequipItem(equippedItem);
            }

            // Equip it via InventoryManager (this handles ring vs necklace vs boots, etc.)
            inventoryManager.EquipItem(itemToEquip);

            // Store reference to show in UI slot
            equippedItem = itemToEquip;
            RefreshUI();
        }
        // 2) Else if there's no item in the mouse, but we have an item in the slot,
        //    pick it up (unequip).
        else if (equippedItem != null)
        { 

                // Put that item in the mouse
                MouseManager.instance.heldItem = equippedItem;

            // Clear this slot
            equippedItem = null;
            RefreshUI();

            // Also tell InventoryManager we "unequipped" it, if needed
            // (You might set ring1Equiped = null, ring2Equiped = null, etc.)
            // For example:
            inventoryManager.UnequipItem(MouseManager.instance.heldItem);      
        }
    }

    /// <summary>
    /// Update placeholder/equipped icon visibility
    /// </summary>
    public void RefreshUI()
    {
        if (equippedItem == null)
        {
            // Show placeholder, hide equipped
            if (placeholderImage != null) placeholderImage.gameObject.SetActive(true);
            if (equippedImage != null)
            {
                equippedImage.gameObject.SetActive(false);
                equippedImage.sprite = null;
            }
        }
        else
        {
            // Hide placeholder, show equipped with item icon
            if (placeholderImage != null) placeholderImage.gameObject.SetActive(false);
            if (equippedImage != null)
            {
                equippedImage.gameObject.SetActive(true);
                equippedImage.sprite = equippedItem.itemIcon;
                equippedImage.color = Color.white; // ensure visible
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (equippedItem != null)
        {
            // If your item has a 'description' field:
            // TooltipManager.instance.ShowTooltip(item.description);

            // Or if you have a different property:
            TooltipManager.instance.ShowTooltip(equippedItem.itemID);
        }
    }

    // New: hide tooltip when mouse leaves
    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.instance.HideTooltip();
    }
}
