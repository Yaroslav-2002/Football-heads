using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    [Header("Player Spawning")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform playerSpawnPoint;

    private GameObject _playerInstance;

    public static GameManager Instance => _instance;
    public GameObject PlayerInstance => _playerInstance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitializeGame();
    }

    public void InitializeGame()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("GameManager: Player prefab is not assigned.");
            return;
        }

        if (_playerInstance != null)
        {
            Destroy(_playerInstance);
        }

        Vector3 spawnPosition = playerSpawnPoint != null ? playerSpawnPoint.position : Vector3.zero;
        Quaternion spawnRotation = playerSpawnPoint != null ? playerSpawnPoint.rotation : Quaternion.identity;

        _playerInstance = Instantiate(playerPrefab, spawnPosition, spawnRotation);
    }
}
