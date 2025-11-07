using Unity.Netcode;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private NetworkEntitySpawner networkSpawner;
    [SerializeField] private EntitySpawner localSpawner;
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private GoalManager goalManager;
    [SerializeField] private GameSettingsView gameSettingsView;

    private EntitySpawnerBase _spawner;
    private NetworkManager _networkManager;

    private void Start()
    {
        Debug.Log($"Game type :{GameConfiguration.CurrentMode}");
        InitializeSpawner();
    }

    public void InitializeSpawner()
    {
        Debug.Log($"Game type :{GameConfiguration.CurrentMode}");

        switch (GameConfiguration.CurrentMode)
        {
            case GameMode.Singleplayer:
                _spawner = Instantiate(localSpawner);
                break;
            case GameMode.Multiplayer:
                _networkManager = Instantiate(networkManager);
                _spawner = Instantiate(networkSpawner);
                ((NetworkEntitySpawner)_spawner).SetNetworkManager(_networkManager);
                break;
        }

        _spawner.Init();
    }

    private void OnEnable()
    {
        gameSettingsView.OnRestart += OnRestartButtonClicked;
        goalManager.GoalScored += OnGoalScored;
    }

    private void OnDisable()
    {
        gameSettingsView.OnRestart -= OnRestartButtonClicked;
        goalManager.GoalScored -= OnGoalScored;
    }

    private void OnGoalScored(TeamSide scoringTeam)
    {
        _spawner.Respawn(scoringTeam);
    }

    private void OnRestartButtonClicked()
    {
        _spawner.Respawn();
    }
}
