using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{

    public GameObject inventoryGrid;
    public bool messyInventory;
    public EquipmentManager equipmentManager;

    [Header("Player unlocked")]
    public bool earthPower;
    public bool firePower;
    public bool icePower;
    public bool windPower;
    public bool doubleJump;

    public Image powerStoneEarth;
    public Image powerStoneFire;
    public Image powerStoneIce;
    public Image powerStoneWind;


    private void Awake()
    {
        ConfigureInventory();
        RefreshPowerStones();
    }

    //******************** Power Stone ************************//
    public void RefreshPowerStones()
    {
        UpdateStoneVisibility(powerStoneEarth, earthPower);
        UpdateStoneVisibility(powerStoneIce, icePower);
        UpdateStoneVisibility(powerStoneFire, firePower);
        UpdateStoneVisibility(powerStoneWind, windPower);
    }

    private void UpdateStoneVisibility(Image stoneImage, bool unlocked)
    {
        if (stoneImage == null) return;

        // If the stone is unlocked...
        if (unlocked)
        {
            stoneImage.color = new Color(1f, 1f, 1f, 1f);
        }
        else
        {
            
            stoneImage.color = new Color(0.11f, 0.11f, 0.11f, 1f);
        }
    }

    //******************** Equipement ************************//

    public void EquipItem(Item item)
    {
        if (item is BootsItem bootsItem)
        {
            doubleJump = bootsItem.doubleJump;
        }
        equipmentManager.EquipItem(item);
    }

    public void UnequipItem(Item item)
    {

        equipmentManager.UnequipItem(item);
        doubleJump = false;

    }
    //******************** Inventory ************************//
    public void PlaceInInventory(UISlotHandler activeSlot, Item item)
    {
        if (activeSlot == null || item == null) return;

        // Always clone the incoming item to avoid shared references
        activeSlot.item = item.Clone();

        // Now set up visuals
        activeSlot.icon.sprite = activeSlot.item.itemIcon;
        if (item.itemCount == 1) activeSlot.itemCountText.text = string.Empty;
        else activeSlot.itemCountText.text = activeSlot.item.itemCount.ToString();
        activeSlot.icon.gameObject.SetActive(true);

        ConfigureInventory();
    }

    public void StackInInventory(UISlotHandler activeSlot, Item item)
    {
        if (activeSlot.item.itemID != item.itemID) { return; }

        activeSlot.item.itemCount += item.itemCount;
        activeSlot.itemCountText.text = activeSlot.item.itemCount.ToString();
        ConfigureInventory();
    }

    public void ClearItemSlot(UISlotHandler activeSlot)
    {
        activeSlot.icon.sprite = null;
        activeSlot.icon.gameObject.SetActive(false);
        activeSlot.itemCountText.text = string.Empty;
        activeSlot.item = null;
    }


    public void ConfigureInventory()
    {
        if (messyInventory) { return; }
        //Rearrange by populated items

        List<Transform> uiSlots = ListInventorySlots();

        uiSlots.Sort((a, b) =>
        {
            UISlotHandler itemA = a.GetComponent<UISlotHandler>();
            UISlotHandler itemB = b.GetComponent<UISlotHandler>();

            bool hasItemA = itemA.item != null;
            bool hasItemB = itemB.item != null;

            return hasItemB.CompareTo(hasItemA);
        });

        for (int i = 0; i < uiSlots.Count; i++)
        {
            uiSlots[i].SetSiblingIndex(i);
        }
    }

    public void AddItemInventory(Item item)
    {
        if (item == null) return;

        // Fetch all the slot transforms
        List<Transform> uiSlots = ListInventorySlots();

        // Iterate over each slot
        foreach (Transform slotTransform in uiSlots)
        {
            UISlotHandler slotHandler = slotTransform.GetComponent<UISlotHandler>();

            // If this slot is empty, place the new item here
            if (slotHandler != null && slotHandler.item == null)
            {
                PlaceInInventory(slotHandler, item);
                return; // Stop after placing the item
            }
        }

        // If we reach here, all slots are filled
        Debug.LogWarning($"No empty slot available for item: {item.itemID}");

    }

    private List<Transform> ListInventorySlots()
    {
        //Loop through each child of inventory grid
        List<Transform> uiSlots = new List<Transform>();
        for (int i = 0; i < inventoryGrid.transform.childCount; i++)
        {
            uiSlots.Add(inventoryGrid.transform.GetChild(i));
        }

        return uiSlots;
    }

    public void switchMessyInventory()
    {
        messyInventory = (messyInventory == true) ? true : false;
    }

}
