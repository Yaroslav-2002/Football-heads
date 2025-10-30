using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class EntitySpawner : MonoBehaviour
{
    [SerializeField] private List<PlayerSpawnSettings> playerSpawnSettings = new();
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Transform ballSpawnPoint;

    private readonly Dictionary<string, GameObject> _playerInstances = new();
    private GameObject _ballInstance;

    public IReadOnlyDictionary<string, GameObject> PlayerInstances => _playerInstances;
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
            
             var playerInstance = Spawn(playerPrefab, settings.SpawnPoint);
            playerInstance.GetComponent<Player>().Init(settings);

            _playerInstances.Add(settings.Identifier, playerInstance);
        }
    }

    public GameObject GetPlayer(string id)
    {
        return _playerInstances.TryGetValue(id, out GameObject instance) ? instance : null;
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

    public void Respawn()
    {
        foreach(var settings in playerSpawnSettings) {
            var playerTransfrom = GetPlayer(settings.Identifier).transform;
            playerTransfrom.position = settings.SpawnPoint.position;
            playerTransfrom.rotation = settings.SpawnPoint.rotation;
        }

        BallInstance.transform.position = ballSpawnPoint.position;
        BallInstance.transform.rotation = ballSpawnPoint.rotation;
    }
}
