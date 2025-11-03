using System;
using Unity.Netcode;
using UnityEngine;

public class NetworkEntitySpawner : EntitySpawnerBase
{
    private NetworkManager _networkManager;

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

    public override void Respawn()
    {
        if (!IsServer())
            return;

        base.Respawn();
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
        if (IsServer())
        {
            SpawnBall();
        }
    }
}
