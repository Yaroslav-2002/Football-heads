using UnityEngine;

public sealed class PlayerSpawner
{
    private readonly IEntitySpawner _entitySpawner;
    private readonly PlayerConfigurator _playerConfigurator;

    public PlayerSpawner(IEntitySpawner entitySpawner, PlayerConfigurator playerConfigurator)
    {
        _entitySpawner = entitySpawner;
        _playerConfigurator = playerConfigurator;
    }

    public GameObject SpawnPlayer(PlayerSpawnSettings settings, GameObject currentInstance)
    {
        if (settings == null)
        {
            return currentInstance;
        }

        GameObject playerInstance = _entitySpawner.Replace(settings.PlayerPrefab, currentInstance, settings.SpawnPoint);
        _playerConfigurator.Configure(playerInstance, settings.ControlScheme);
        return playerInstance;
    }
}
