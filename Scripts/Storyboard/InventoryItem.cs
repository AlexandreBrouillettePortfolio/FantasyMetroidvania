using UnityEngine;

/// <summary>
/// Represents an item in the player's inventory.
/// </summary>
[CreateAssetMenu(menuName = "Game/Inventory Item")]
public class InventoryItem : ScriptableObject
{
    public string Description;

    public Sprite Sprite;

    public bool IsKeyItem;
}
