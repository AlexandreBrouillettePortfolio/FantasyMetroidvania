using UnityEngine;

[CreateAssetMenu(fileName = "New Ring", menuName = "ScriptableObject/Item/Ring")]
public class RingItem : Item
{
    public int lifeBonus;
    public int manaBonus;
    public int staminaBonus;
}