using System.Collections.Generic;
using UnityEngine;

public abstract class EntitySpawnerBase : MonoBehaviour
{
    [SerializeField] protected GameobjectsRuntimeSet gameobjects;

    [Header("Player")] [SerializeField] protected List<PlayerSpawnSettings> playerSpawnSettings = new();
    [SerializeField] protected GameObject playerPrefab;

    [Header("Ball")] [SerializeField] protected GameObject ballPrefab;
    [SerializeField] protected Transform ballSpawnPoint;
    private GameObject _ballInstance;

    private readonly Dictionary<string, GameObject> _playerInstances = new();

    public IReadOnlyDictionary<string, GameObject> PlayerInstances => _playerInstances;
    public GameObject BallInstance => _ballInstance;

    protected virtual void Awake()
    {
        RemoveNullSettings();
    }

    public abstract void Init();

    protected IEnumerable<PlayerSpawnSettings> PlayerSettings => playerSpawnSettings;

    public virtual void SpawnEntities()
    {
        ClearCachedInstances();
        SpawnPlayers();
        SpawnBall();
    }

    public virtual void SpawnPlayers()
    {
        foreach (PlayerSpawnSettings settings in playerSpawnSettings)
        {
            if (settings == null)
                continue;

            GameObject playerInstance = Spawn(playerPrefab, settings.SpawnPoint);
            if (playerInstance == null)
                continue;

            InitializePlayer(playerInstance, settings);
            CachePlayer(settings.Identifier, playerInstance);
            OnPlayerSpawned(playerInstance, settings);
        }
    }

    public virtual void SpawnBall()
    {
        _ballInstance = Spawn(ballPrefab, ballSpawnPoint);
        OnBallSpawned(_ballInstance);
    }

    public GameObject GetPlayer(string id)
    {
        return _playerInstances.TryGetValue(id, out GameObject instance) ? instance : null;
    }

    public virtual void Respawn(TeamSide scoringTeam = TeamSide.None)
    {
        foreach (PlayerSpawnSettings settings in playerSpawnSettings)
        {
            if (settings == null)
                continue;

            if (!_playerInstances.TryGetValue(settings.Identifier, out GameObject playerInstance))
                continue;

            ApplyTransform(playerInstance.transform, settings.SpawnPoint);
            OnPlayerRespawned(playerInstance, settings);
        }

        if (_ballInstance != null)
        {
            ApplyTransform(_ballInstance.transform, ballSpawnPoint);
            OnBallRespawned(_ballInstance, scoringTeam);
        }
    }

    protected virtual void RemoveNullSettings()
    {
        playerSpawnSettings.RemoveAll(settings => settings == null);
    }

    protected virtual void CachePlayer(string id, GameObject instance)
    {
        if (string.IsNullOrWhiteSpace(id) || instance == null)
            return;

        _playerInstances[id] = instance;
    }

    protected virtual void ClearCachedInstances()
    {
        foreach (GameObject instance in _playerInstances.Values)
        {
            DestroyInstance(instance);
        }

        _playerInstances.Clear();

        DestroyInstance(_ballInstance);
        _ballInstance = null;
    }

    protected virtual void DestroyInstance(GameObject instance)
    {
        if (instance != null)
        {
            gameobjects.RemoveItem(instance);
            Destroy(instance);
        }
    }

    protected virtual GameObject Spawn(GameObject prefab, Transform spawnPoint)
    {
        if (prefab == null)
            return null;

        Vector3 position = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        Quaternion rotation = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;

        GameObject gameobject = Instantiate(prefab, position, rotation);
        gameobjects.AddItem(gameobject);

        return gameobject;
    }

    protected static void ApplyTransform(Transform target, Transform source)
    {
        if (target == null)
            return;

        if (source != null)
        {
            target.SetPositionAndRotation(source.position, source.rotation);
        }
        else
        {
            target.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
    }

    protected virtual void OnPlayerSpawned(GameObject playerInstance, PlayerSpawnSettings settings) { }

    protected virtual void OnBallSpawned(GameObject ballInstance) { }

    protected virtual void OnPlayerRespawned(GameObject playerInstance, PlayerSpawnSettings settings) { }

    protected virtual void OnBallRespawned(GameObject ballInstance, TeamSide teamSide = TeamSide.None) 
    {
        _ballInstance.GetComponent<Ball>().Respawn(teamSide);
    }

    protected abstract void InitializePlayer(GameObject playerInstance, PlayerSpawnSettings settings);

    private void OnDisable()
    {
        gameobjects.Clear();
    }
}
