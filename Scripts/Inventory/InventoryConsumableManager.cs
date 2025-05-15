using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryConsumableManager : MonoBehaviour, IPointerClickHandler
{
    public Player statsManager; //Player scirpt is only used as a stats manager in this instance

    void Start()
    {
        statsManager = FindAnyObjectByType<Player>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 1) If we have an item in the mouse (heldItem), try to equip it
        if (MouseManager.instance.heldItem != null)
        {
            Item itemToEquip = MouseManager.instance.heldItem;

            if (itemToEquip is ConsumableItem itemToConsume)
            {
                ConsumeItem(itemToConsume);
            }
            else 
            {
                Debug.LogWarning("Item class doesn't match this slot's type!");
                return;
            }
        }
    }

    private void ConsumeItem(ConsumableItem item)
    {
        statsManager.ModifyHealth(item.restoredHealth);
        statsManager.ModifyEndurance(item.restoredStamina);
        statsManager.ModifyMana(item.restoredMana);

        MouseManager.instance.heldItem.itemCount--;
        if(MouseManager.instance.heldItem.itemCount==0)
        {
            MouseManager.instance.heldItem = null;
        }
    }
}

