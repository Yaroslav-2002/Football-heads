using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class NetworkEntitySpawner : EntitySpawnerBase
{
    private NetworkManager _networkManager;
    [SerializeField] private int requiredPlayersToStartMatch = 2;
    [SerializeField] private float ballSpawnDelaySeconds = 2f;
    private Coroutine _ballSpawnCoroutine;

    public void SetNetworkManager(NetworkManager networkManager)
    {
        if (_networkManager != null)
            return;

        _networkManager = networkManager;
        _networkManager.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnDisable()
    {
        if (_networkManager != null)
        {
            _networkManager.OnClientConnectedCallback -= OnClientConnected;
        }

        if (_ballSpawnCoroutine != null)
        {
            StopCoroutine(_ballSpawnCoroutine);
            _ballSpawnCoroutine = null;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        // Only server/host can spawn networked objects
        if (!IsServer())
            return;

        Debug.Log($"Client connected: {clientId}");

        int index = PlayerInstances.Count;
        if (index < 0 || index >= playerSpawnSettings.Count)
        {
            Debug.LogWarning($"No spawn settings configured for client {clientId}.", this);
            return;
        }

        var settings = playerSpawnSettings[index];

        GameObject playerInstance = SpawnPlayerForClient(playerPrefab, settings.SpawnPoint, clientId);
        if (playerInstance == null)
        {
            Debug.LogWarning($"Failed to spawn player for client {clientId}.", this);
            return;
        }

        InitializePlayer(playerInstance, settings);
        CachePlayer(settings.Identifier, playerInstance);
        OnPlayerSpawned(playerInstance, settings);

        TryScheduleBallSpawn();
    }

    protected override void InitializePlayer(GameObject playerInstance, PlayerSpawnSettings settings)
    {
        if (playerInstance.TryGetComponent(out NetworkPlayer player))
        {
            player.Init(settings);
        }
        else
        {
            Debug.LogWarning($"Player prefab is missing {nameof(NetworkPlayer)} component.", playerInstance);
        }
    }

    public override void Respawn(TeamSide teamSide = TeamSide.None)
    {
        if (!IsServer())
            return;

        base.Respawn(teamSide);
    }

    protected override GameObject Spawn(GameObject prefab, Transform spawnPoint)
    {
        GameObject instance = base.Spawn(prefab, spawnPoint);
        if (instance == null)
            return null;

        TrySpawnNetworkObject(instance);
        return instance;
    }

    protected override void OnBallSpawned(GameObject ballInstance)
    {
        TrySpawnNetworkObject(ballInstance);
    }

    protected override void DestroyInstance(GameObject instance)
    {
        if (instance == null)
            return;

        if (instance.TryGetComponent(out NetworkObject networkObject))
        {
            if (IsServer() && networkObject.IsSpawned)
            {
                networkObject.Despawn(true);
            }
        }
        else
        {
            base.DestroyInstance(instance);
        }
    }

    private GameObject SpawnPlayerForClient(GameObject prefab, Transform spawnPoint, ulong clientId)
    {
        GameObject instance = base.Spawn(prefab, spawnPoint);
        if (instance == null)
        {
            return null;
        }

        TrySpawnPlayerObject(instance, clientId);
        return instance;
    }

    private static void TrySpawnNetworkObject(GameObject instance)
    {
        if (instance == null || !IsServer())
            return;

        if (!instance.TryGetComponent(out NetworkObject networkObject))
            return;

        if (!networkObject.IsSpawned)
        {
            networkObject.Spawn();
        }
    }

    private static void TrySpawnPlayerObject(GameObject instance, ulong clientId)
    {
        if (instance == null || !IsServer())
            return;

        if (!instance.TryGetComponent(out NetworkObject networkObject))
            return;

        if (!networkObject.IsSpawned)
        {
            networkObject.SpawnAsPlayerObject(clientId);
        }
        else
        {
            networkObject.ChangeOwnership(clientId);
        }
    }

    private static bool IsServer()
    {
        return NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer;
    }

    public override void Init()
    {
        if (!IsServer())
            return;

        TryScheduleBallSpawn();
    }

    private void TryScheduleBallSpawn()
    {
        if (!IsServer())
            return;

        if (_ballSpawnCoroutine != null || BallInstance != null)
            return;

        int playersNeeded = GetPlayersNeededToStartMatch();
        if (playersNeeded <= 0)
            return;

        if (PlayerInstances.Count < playersNeeded)
            return;

        _ballSpawnCoroutine = StartCoroutine(SpawnBallAfterDelay());
    }

    private IEnumerator SpawnBallAfterDelay()
    {
        float delay = Mathf.Max(0f, ballSpawnDelaySeconds);
        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }

        _ballSpawnCoroutine = null;

        if (!IsServer())
            yield break;

        int playersNeeded = GetPlayersNeededToStartMatch();
        if (playersNeeded > 0 && PlayerInstances.Count < playersNeeded)
            yield break;

        SpawnBall();
    }

    private int GetPlayersNeededToStartMatch()
    {
        int maxPlayers = playerSpawnSettings.Count;
        if (maxPlayers <= 0)
            return 0;

        if (requiredPlayersToStartMatch <= 0)
            return maxPlayers;

        return Mathf.Min(requiredPlayersToStartMatch, maxPlayers);
    }
}
