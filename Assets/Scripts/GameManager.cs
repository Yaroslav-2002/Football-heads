using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    [Header("Player Spawning")]
    [SerializeField] private List<PlayerSpawnSettings> playerSpawnSettings = new();
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Transform ballSpawnPoint;

    [SerializeField] private BoxCollider2D gatesColliderLeft;
    [SerializeField] private BoxCollider2D gatesColliderRight;

    private readonly Dictionary<PlayerInput.ControlScheme, GameObject> _playerInstances = new();
    private IEntitySpawner _entitySpawner;
    private PlayerSpawner _playerSpawner;
    private GameObject _ballInstance;

    public static GameManager Instance => _instance;
    public IReadOnlyDictionary<PlayerInput.ControlScheme, GameObject> PlayerInstances => _playerInstances;
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

        _entitySpawner = new EntitySpawner();
        _playerSpawner = new PlayerSpawner(_entitySpawner, new PlayerConfigurator());

        EnsurePlayerSettings();
    }

    private void Start()
    {
        InitializeGame();
    }

    public void InitializeGame()
    {
        SpawnPlayers();
        SpawnBall();
    }

    public GameObject GetPlayer(PlayerInput.ControlScheme controlScheme)
    {
        return _playerInstances.TryGetValue(controlScheme, out GameObject instance) ? instance : null;
    }

    private void SpawnPlayers()
    {
        HashSet<PlayerInput.ControlScheme> processedSchemes = new HashSet<PlayerInput.ControlScheme>();

        foreach (PlayerSpawnSettings settings in playerSpawnSettings)
        {
            if (settings == null)
            {
                continue;
            }

            GameObject existingInstance = GetPlayer(settings.ControlScheme);
            GameObject playerInstance = _playerSpawner.SpawnPlayer(settings, existingInstance);

            if (playerInstance == null)
            {
                continue;
            }

            _playerInstances[settings.ControlScheme] = playerInstance;
            processedSchemes.Add(settings.ControlScheme);
        }

        if (processedSchemes.Count == 0)
        {
            return;
        }

        foreach (PlayerInput.ControlScheme scheme in new List<PlayerInput.ControlScheme>(_playerInstances.Keys))
        {
            if (processedSchemes.Contains(scheme))
            {
                continue;
            }

            GameObject orphanedInstance = _playerInstances[scheme];
            if (orphanedInstance != null)
            {
                Destroy(orphanedInstance);
            }

            _playerInstances.Remove(scheme);
        }
    }

    private void SpawnBall()
    {
        _ballInstance = _entitySpawner.Replace(ballPrefab, _ballInstance, ballSpawnPoint);
    }

    private void EnsurePlayerSettings()
    {
        if (playerSpawnSettings != null && playerSpawnSettings.Count > 0)
        {
            return;
        }
    }

    internal void GoalScored(int gateId)
    {
        throw new NotImplementedException();
    }
}
