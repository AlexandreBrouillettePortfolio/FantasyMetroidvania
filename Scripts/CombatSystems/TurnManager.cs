using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TurnManager : MonoBehaviour
{
    private bool _combatFinished = false;
    private bool isCombatActive = false;

    [Header("Player")]
    [SerializeField] private GameObject _player;
    [SerializeField] private PlayerMovement _playerMovementScript;
    [SerializeField] private GameObject _playerUI;
    [SerializeField] private StatsManager _statsManager;
    private Player _playerManager;

    [Header("Enemies UI")]
    [SerializeField] private List<Enemy> _opponentManagers;
    [SerializeField] private int _targetEnemyCount;
    [SerializeField] private Enemy _currentEnemy;

    [Header("Combat UI")]
    [SerializeField] private GameObject _winUI;
    [SerializeField] private GameObject _loseUI;
    [SerializeField] private GameObject _combatUI;
    [SerializeField] private ExperienceManager _experienceManager;
    [SerializeField] private int _experienceGain;

    [Header("Clips SFX")]
    private AudioSource _audioSource;
    public AudioClip selectSFX;
    public AudioClip endTurnSFX;

    public enum TurnState { Player, Enemy }
    private TurnState currentTurn;

    void OnEnable()
    {
        Character.OnCharacterDeath += HandleCharacterDeath;
        _playerManager = _player.GetComponent<Player>();
        _audioSource = _player.GetComponent<AudioSource>();
        _playerManager.myTurn = true;
        _targetEnemyCount = 0;

        ChangeTurn();
    }

    void OnDisable()
    {
        Character.OnCharacterDeath -= HandleCharacterDeath;
    }

    public void ChangeTurn()
    {
        StartCoroutine(Turn());
        _audioSource.PlayOneShot(endTurnSFX);
    }

    private void ClearDeadEnemy()
    {
        Debug.Log("CHECK DEAD ENEMY");
        for (int i = 0; i < _opponentManagers.Count; i++)
        {
            if (_opponentManagers[i] == null)
            {
                Debug.Log("Delete enemy : " + _opponentManagers[i].name);
                HandleCharacterDeath(_opponentManagers[i]);
            }
        }
    }

    public IEnumerator Turn()
    {
        ClearDeadEnemy();
        if (!_combatFinished)
        {
            if (_playerManager.myTurn)
            {
                _playerManager.NewTurn();
                _playerManager.myTurn = false;

                if (!_playerManager.isStun)
                {
                    _playerManager.characterUI.SetActive(true);
                    _targetEnemyCount = 0;
                    HighlightEnemy();
                }
                else
                {
                    _playerManager.RemoveStun();
                    ChangeTurn();
                }
            }
            else
            {
                _playerManager.characterUI.SetActive(false);
                _playerManager.myTurn = true;
                RemoveHighlight();
                _targetEnemyCount = 0;

                foreach (Enemy manager in _opponentManagers)
                {
                    HighlightEnemy();
                    manager.NewTurn();
                    yield return StartCoroutine(manager.EnemyTurnRoutine());
                    RemoveHighlight();
                    _targetEnemyCount++;
                }

                ChangeTurn();
            }
        }
    }

    private void HandleCharacterDeath(Character character)
    {
        if (character is Player)
        {
            StopAllCoroutines();
            _combatFinished = true;
            _loseUI.SetActive(true);
            Invoke("ExitCombat", 3f);
        }
        else if (character is Enemy enemy)
        {
            Debug.Log("CHECK DEAD ENEMY NAME : " + enemy.name);
            _opponentManagers.Remove(enemy);
            _currentEnemy = null;
            _playerManager.target = null;
            _experienceGain += enemy.experienceGain;

            if (_opponentManagers.Count != 0)
            {
                if (_targetEnemyCount > _opponentManagers.Count - 1) _targetEnemyCount--;

                HighlightEnemy();
            }

            if (_opponentManagers.Count == 0)
            {
                StopAllCoroutines();
                _combatFinished = true;
                _winUI.SetActive(true);
                Invoke("ExitCombat", 1.5f);
            }
        }
    }

    public void AddEnemies(List<GameObject> enemies)
    {
        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
            {
                Enemy managerEnemy = enemy.GetComponent<Enemy>();
                if (!_opponentManagers.Contains(managerEnemy))
                {
                    _opponentManagers.Add(managerEnemy);
                    managerEnemy.target = _playerManager;
                    managerEnemy.combatUI.SetActive(true);
                }
            }
        }

        HighlightEnemy();
    }

    public void SelectNextEnemy()
    {
        if (_opponentManagers.Count == 0) return;

        RemoveHighlight();

        _targetEnemyCount++;
        if (_targetEnemyCount >= _opponentManagers.Count)
            _targetEnemyCount = 0;

        _audioSource.PlayOneShot(selectSFX);
        HighlightEnemy();
    }

    public void SelectPreviousEnemy()
    {
        if (_opponentManagers.Count == 0) return;

        RemoveHighlight();

        _targetEnemyCount--;
        if (_targetEnemyCount < 0)
            _targetEnemyCount = _opponentManagers.Count - 1;

        _audioSource.PlayOneShot(selectSFX);
        HighlightEnemy();
    }

    private void HighlightEnemy()
    {
        if (_opponentManagers.Count == 0) return;

        _currentEnemy = _opponentManagers[_targetEnemyCount];

        if (_currentEnemy.focusUI != null)
        {
            _currentEnemy.focusUI.SetActive(true);
        }

        _playerManager.target = _currentEnemy;
    }

    private void RemoveHighlight()
    {
        if (_currentEnemy == null) return;

        _currentEnemy.focusUI.SetActive(false);
    }

    public void ExitCombat()
    {
        if (_playerManager.currentHealth == 0)
        {
            SceneManager.LoadScene(0);
            return;
        }

        if (_experienceManager != null)
        {
            _experienceManager.AddExperience(_experienceGain);
        }

        _playerMovementScript.CombatTrigger(false);
        _combatUI.SetActive(false);
        _winUI.SetActive(false);
        _loseUI.SetActive(false);

        _playerManager.ResetActionPoints();
        _combatFinished = false;
        _opponentManagers.Clear();
    }

    public void StartCombat(BaseEnemy enemy, Player player)
    {
        enemy.SetInCombat(true);

        player.GetComponent<PlayerMovement>().CombatTrigger(true);

        currentTurn = TurnState.Player;
        isCombatActive = true;

        player.GetComponent<PlayerMovement>().StopMovement();

        if (_combatUI != null)
        {
            _combatUI.SetActive(true);
        }

        ChangeTurn();
    }
}
