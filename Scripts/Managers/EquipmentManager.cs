using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Minimal Equipment Manager:
/// - No stat bonus logic
/// - Just tracks which items are equipped in each slot
/// - Has stubs for "UnequipAllForLevelUp" and "ReequipAllAfterLevelUp"
/// </summary>
public class EquipmentManager : MonoBehaviour
{
    public Player player;

    [Header("Currently Equipped Items")]
    public NecklaceItem necklaceEquipped;
    public RingItem ring1Equipped;
    public RingItem ring2Equipped;
    public BootsItem bootsEquipped;

    // Temporary storage for items when leveling up
    private NecklaceItem tempNecklace;
    private RingItem tempRing1;
    private RingItem tempRing2;
    private BootsItem tempBoots;

    /// <summary>
    /// Equip an item in the correct slot if possible (no stat changes).
    /// </summary>
    public void EquipItem(Item item)
    {
        if (item == null) return;

        if (item is NecklaceItem necklace)
        {
            // If you only allow 1 necklace, just overwrite
            necklaceEquipped = necklace;
            Debug.Log($"Equipped necklace: {necklace.name}");
        }
        else if (item is RingItem ring)
        {
            // If ring1 is empty, put it there; else ring2
            if (ring1Equipped == null)
            {
                ring1Equipped = ring;
                Debug.Log($"Equipped ring: {ring.name} in Ring Slot 1");
            }
            else if (ring2Equipped == null)
            {
                ring2Equipped = ring;
                Debug.Log($"Equipped ring: {ring.name} in Ring Slot 2");
            }
            else
            {
                Debug.LogWarning("Both ring slots are full! Cannot equip another ring.");
            }
        }
        else if (item is BootsItem boots)
        {
            bootsEquipped = boots;
            Debug.Log($"Equipped boots: {boots.name}");
        }
        else
        {
            Debug.LogWarning($"{item.name} is not recognized as equippable in this system.");
        }
        player.RecalculateStatsEquipement();
    }

    /// <summary>
    /// Unequip a currently equipped item, if it's in any slot (no stat changes).
    /// </summary>
    public void UnequipItem(Item item)
    {
        if (item == null) return;

        if (item is NecklaceItem && necklaceEquipped == item)
        {
            necklaceEquipped = null;
            Debug.Log($"Unequipped necklace: {item.name}");
        }
        else if (item is RingItem)
        {
            if (ring1Equipped == item)
            {
                ring1Equipped = null;
                Debug.Log($"Unequipped ring: {item.name} from Ring Slot 1");
            }
            else if (ring2Equipped == item)
            {
                ring2Equipped = null;
                Debug.Log($"Unequipped ring: {item.name} from Ring Slot 2");
            }
        }
        else if (item is BootsItem && bootsEquipped == item)
        {
            bootsEquipped = null;
            Debug.Log($"Unequipped boots: {item.name}");
        }
        else
        {
            Debug.LogWarning($"Cannot unequip {item.name}: it isn't equipped.");
        }
        player.RecalculateStatsEquipement();
    }

    /// <summary>
    /// Unequip everything and store them in temp fields 
    /// </summary>
    public void UnequipAllForLevelUp()
    {
        // Store references in temp fields
        tempNecklace = necklaceEquipped;
        tempRing1 = ring1Equipped;
        tempRing2 = ring2Equipped;
        tempBoots = bootsEquipped;

        // Clear the actual equipped slots
        necklaceEquipped = null;
        ring1Equipped = null;
        ring2Equipped = null;
        bootsEquipped = null;

        Debug.Log("UnequipAllForLevelUp: All items unequipped (temp stored).");
    }

    /// <summary>
    /// Re-equip whatever was previously equipped 
    /// before leveling up.
    /// </summary>
    public void ReequipAllAfterLevelUp()
    {
        // Restore from temp fields
        necklaceEquipped = tempNecklace;
        ring1Equipped = tempRing1;
        ring2Equipped = tempRing2;
        bootsEquipped = tempBoots;

        // Clear temp fields
        tempNecklace = null;
        tempRing1 = null;
        tempRing2 = null;
        tempBoots = null;

        player.RecalculateStatsEquipement();

        Debug.Log("ReequipAllAfterLevelUp: All items re-equipped from temp.");
    }
}