using UnityEngine;

public class EntitySpawner : EntitySpawnerBase
{
    public override void Init()
    {
        SpawnEntities();
    }

    protected override void InitializePlayer(GameObject playerInstance, PlayerSpawnSettings settings)
    {
        if (playerInstance.TryGetComponent(out Player player))
        {
            player.Init(settings);
        }
        else
        {
            Debug.LogWarning($"Player prefab is missing {nameof(Player)} component.", playerInstance);
        }
    }
}
