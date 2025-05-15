using UnityEngine;

/// <summary>
/// Represents an item in the player's inventory.
/// </summary>
[CreateAssetMenu(menuName = "Game/Weapon")]
public class Weapon : ScriptableObject
{
    public string Name;
    public string Description;
    public Sprite Sprite;
}