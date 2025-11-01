using System;
using Unity.Netcode;
using UnityEngine;

public class NetworkEntitySpawner : EntitySpawnerBase
{
    [SerializeField] private NetworkManager networkManager;

    private void OnEnable()
    {
        networkManager.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        // Only server/host can spawn networked objects
        if (!IsServer())
            return;

        Debug.Log($"Client connected: {clientId}");

        int index = networkManager.ConnectedClientsIds.Count - 1;

        var settings = playerSpawnSettings[index];

        GameObject playerInstance = Spawn(playerPrefab, settings.SpawnPoint);

        InitializePlayer(playerInstance, settings);
        //playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
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
