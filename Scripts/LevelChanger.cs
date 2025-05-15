using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
    [SerializeField]
    private CharacterController2d _player;

    [SerializeField]
    private LevelConnection _connection;

    [SerializeField]
    private Transform _spawnPoint;

    [SerializeField]
    public string _targetSceneName;

    private void Start()
    {
        if (_connection == LevelConnection.ActiveConnection)
        {
            _player.transform.position = _spawnPoint.position;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent<CharacterController2d>(out var player))
        {
            LevelConnection.ActiveConnection = _connection;
            SceneManager.LoadScene(_targetSceneName);
        }
    }
}
