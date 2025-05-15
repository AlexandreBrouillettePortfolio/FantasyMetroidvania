using UnityEngine;

[CreateAssetMenu(fileName = "New Necklace", menuName = "ScriptableObject/Item/Necklace")]
public class NecklaceItem : Item
{
    public int lifeBonus;
    public int manaBonus;
    public int staminaBonus;
}