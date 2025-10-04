using System.Collections.Generic;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    [SerializeField] private List<PlayerSpawnSettings> playerSpawnSettings = new();
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Transform ballSpawnPoint;

    private readonly Dictionary<TeamSide, GameObject> _playerInstances = new();
    private GameObject _ballInstance;

    public IReadOnlyDictionary<TeamSide, GameObject> PlayerInstances => _playerInstances;
    public GameObject BallInstance => _ballInstance;

    public GameObject Spawn(GameObject prefab, Transform spawnPoint)
    {
        Vector3 position = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        Quaternion rotation = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;

        return Instantiate(prefab, position, rotation);
    }

    public void SpawnPlayers()
    {
        foreach (PlayerSpawnSettings settings in playerSpawnSettings)
        {
            if (settings == null)
                continue;

            GameObject playerInstance = Spawn(playerPrefab, settings.SpawnPoint);

            if (playerInstance == null)
                continue;
            Configure(playerInstance, settings.ControlScheme);
            _playerInstances[settings.Team] = playerInstance;
        }
    }

    private void Configure(GameObject playerObject, PlayerInput.ControlScheme controlScheme)
    {
        if (playerObject == null)
        {
            return;
        }

        PlayerInput playerInput = playerObject.GetComponent<PlayerInput>();
        if (playerInput == null)
        {
            Debug.LogWarning($"PlayerConfigurator: {playerObject.name} is missing a PlayerInput component.", playerObject);
            return;
        }

        playerInput.Configure(controlScheme);
    }

    public GameObject GetPlayer(TeamSide team)
    {
        return _playerInstances.TryGetValue(team, out GameObject instance) ? instance : null;

    }

    public void SpawnBall()
    {
        _ballInstance = Spawn(ballPrefab, ballSpawnPoint);
    }

    internal void SpawnEntities()
    {
        SpawnPlayers();
        SpawnBall();
    }
}
