using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatTrigger : MonoBehaviour
{
    [SerializeField] private GameObject _combatUI;
    [SerializeField] private List<GameObject> _opponentManagers;
    private bool _combatStarted = false;

    public bool GetCombatStarted()
    {
        return _combatStarted;
    }

    public void StartCombatTrigger(Enemy enemy)
    {
        _opponentManagers.Clear();
        Player player = GetComponent<Player>();

        if (_combatStarted)
        {
            // The old fight have set the variables -> Clear it for the new one
            GetComponent<PlayerMovement>().CombatTrigger(false);
            _combatStarted = false;
        }

        string uuidToAttack = enemy.GetUuidSpawn();
        List<GameObject> enemiesToFight = GetComponent<Player>().GetEnemiesWithUUID(uuidToAttack);

        if (enemiesToFight.Count > 0)
        {
            AddOpponents(enemiesToFight);
        }

        PlayerMovement playerMovementComponent = GetComponent<PlayerMovement>();
        if (playerMovementComponent != null)
        {
            playerMovementComponent.CombatTrigger(true);
        }

        foreach (GameObject tmpEnemy in _opponentManagers)
        {
            GroundMovement enemyGround = tmpEnemy.GetComponent<GroundMovement>();
            if (enemyGround != null)
            {
                enemyGround.hasSetStartPosition = false;
                enemyGround.FlipEnemyInTargetDirection(player.transform);
                enemyGround.SetMoveSpeed(0.0f);
            }

            FlyingMovement enemyFlying = tmpEnemy.GetComponent<FlyingMovement>();
            if (enemyFlying != null)
            {
                enemyFlying.SetMoveSpeed(0.0f);
                enemyFlying.playerDetected = false;
                enemyFlying.FlipEnemyInTargetDirection(player.transform);
            }
        }

        _combatStarted = true;
        _combatUI.SetActive(true);
        GameObject.Find("TurnManager").GetComponent<TurnManager>().AddEnemies(_opponentManagers);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!_combatStarted)
        {
            if (other.transform.tag == "Enemy")
            {
                Enemy enemy = other.GetComponent<Enemy>();
                if (enemy != null)
                {
                    StartCombatTrigger(enemy);
                }
            }
        } else {
            bool isAllEnemiesNull = _opponentManagers.All(item => item != null);

            if (!isAllEnemiesNull)
            {
                _combatStarted = false;
                _opponentManagers.Clear();
            }
        }
    }

    public void AddOpponents(List<GameObject> opponents)
    {
        foreach (GameObject opponent in opponents)
        {
            if (!_opponentManagers.Contains(opponent))
            {
                _opponentManagers.Add(opponent);
            }
        }
    }
}
