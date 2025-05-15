using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "ScriptableObject/Item/Consumable")]
public class ConsumableItem : Item
{
    public int restoredHealth;
    public int restoredMana;
    public int restoredStamina;
}