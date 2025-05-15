// Put this in its own file, e.g. InventoryEvents.cs
using System;
using UnityEngine;

public static class InventoryEvents
{
    /// <summary>Raised every time an Item is successfully added to the inventory.</summary>
    public static event Action<Item> OnItemAdded;

    /// <summary>Raised every time the player unlocks an elemental stone (1-4).</summary>
    public static event Action<int> OnPowerStoneUnlocked;

    // Helper methods so you don’t expose Invoke() publicly
    public static void RaiseItemAdded(Item item) => OnItemAdded?.Invoke(item);
    public static void RaisePowerStoneUnlocked(int stoneIndex) => OnPowerStoneUnlocked?.Invoke(stoneIndex);
}