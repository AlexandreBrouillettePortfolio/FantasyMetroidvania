using UnityEngine;

public class EnemyGlobalState : MonoBehaviour
{
    public static EnemyGlobalState Instance { get; private set; }
    
    public bool IsPlayerHit { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetPlayerHit(bool hit)
    {
        IsPlayerHit = hit;
    }
} 