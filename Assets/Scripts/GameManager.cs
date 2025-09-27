using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    [Header("Player Spawning")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Transform ballSpawnPoint;
    [SerializeField] private Transform playerRightSpawnPoint;
    [SerializeField] private Transform playerLeftSpawnPoint;

    private GameObject _playerInstance;
    private GameObject _secondPlayerInstance;
    private GameObject _ballInstance;

    public static GameManager Instance => _instance;
    public GameObject PlayerInstance => _playerInstance;
    public GameObject BallInstance => _ballInstance;

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
        _playerInstance = InitializeEntity(playerPrefab, _playerInstance, playerRightSpawnPoint, "Player");
        _secondPlayerInstance = InitializeEntity(playerPrefab, _secondPlayerInstance, playerLeftSpawnPoint, "Player");
        _ballInstance = InitializeEntity(ballPrefab, _ballInstance, ballSpawnPoint, "Ball");
    }

    private GameObject InitializeEntity(GameObject prefab, GameObject currentInstance, Transform spawnPoint, string name)
    {
        if (prefab == null)
        {
            Debug.LogError($"GameManager: {name} prefab is not assigned.");
            return currentInstance;
        }

        if (currentInstance != null)
        {
            Destroy(currentInstance);
        }

        return SpawnEntity(prefab, spawnPoint);
    }

    private GameObject SpawnEntity(GameObject prefab, Transform spawnPoint)
    {
        Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        Quaternion spawnRotation = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;

        return Instantiate(prefab, spawnPosition, spawnRotation);
    }
}
