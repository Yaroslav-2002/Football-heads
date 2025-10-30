using Unity.Netcode;
using UnityEngine;

public class NetworkEntitySpawner : EntitySpawnerBase
{
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

    public override void SpawnEntities()
    {
        if (!IsServer())
        {
            Debug.LogWarning("NetworkEntitySpawner.SpawnEntities can only be invoked on the server instance.", this);
            return;
        }

        base.SpawnEntities();
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
}
