using UnityEngine;

[CreateAssetMenu(fileName = "New Boots", menuName = "ScriptableObject/Item/Boots")]
public class BootsItem : Item
{
    public int lifeBonus;
    public int manaBonus;
    public int staminaBonus;

    public bool doubleJump;
}