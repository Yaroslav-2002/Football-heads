using System;
using UnityEngine;

[Serializable]
public class PlayerSpawnSettings
{
    [SerializeField] private string identifier = "Player";
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private PlayerInput.ControlScheme controlScheme = PlayerInput.ControlScheme.PlayerOne;
    [SerializeField] private TeamSide team;
    //Texture

    public PlayerSpawnSettings()
    {
    }

    public PlayerSpawnSettings(string id, GameObject prefab, Transform spawnLocation, TeamSide teamSide, PlayerInput.ControlScheme scheme)
    {
        identifier = id;
        spawnPoint = spawnLocation;
        controlScheme = scheme;
        team = teamSide;
    }

    public TeamSide Team => team;
    public string Identifier => string.IsNullOrWhiteSpace(identifier) ? controlScheme.ToString() : identifier;
    public Transform SpawnPoint => spawnPoint;
    public PlayerInput.ControlScheme ControlScheme => controlScheme;
}

public enum TeamSide
{
    Left,
    Right
}