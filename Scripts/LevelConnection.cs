using UnityEngine;

[CreateAssetMenu(menuName = "Game/Level Connection")]
public class LevelConnection : ScriptableObject
{
    public static LevelConnection ActiveConnection { get; set; }
}
