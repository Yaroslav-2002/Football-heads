using System;
using UnityEngine;

[Serializable]
public class PlayerSpawnSettings
{
    [SerializeField] private string identifier = "Player";
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private PlayerInput.ControlScheme controlScheme = PlayerInput.ControlScheme.PlayerOne;

    public PlayerSpawnSettings()
    {
    }

    public PlayerSpawnSettings(string id, GameObject prefab, Transform spawnLocation, PlayerInput.ControlScheme scheme)
    {
        identifier = id;
        playerPrefab = prefab;
        spawnPoint = spawnLocation;
        controlScheme = scheme;
    }

    public string Identifier => string.IsNullOrWhiteSpace(identifier) ? controlScheme.ToString() : identifier;
    public GameObject PlayerPrefab => playerPrefab;
    public Transform SpawnPoint => spawnPoint;
    public PlayerInput.ControlScheme ControlScheme => controlScheme;
}
