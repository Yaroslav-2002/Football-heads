using Unity.Netcode;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [SerializeField] private NetworkEntitySpawner networkSpawner;
    [SerializeField] private EntitySpawner localSpawner;
    [SerializeField] private ScoreBoard scoreBoard;
    [SerializeField] private GoalTrigger leftTrigger;
    [SerializeField] private GoalTrigger rightTrigger;
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private GameConfiguration configuration;

    private EntitySpawnerBase _spawner;
    private NetworkManager _networkManager;

    private void OnEnable()
    {
        leftTrigger.OnGoalScored += OnGoalScored;
        rightTrigger.OnGoalScored += OnGoalScored;
    }

    private void OnDisable()
    {
        leftTrigger.OnGoalScored -= OnGoalScored;
        rightTrigger.OnGoalScored -= OnGoalScored;
    }

    private void Start()
    {
        InitializeGame();
    }

    public void InitializeGame()
    {
        if (configuration == null)
        {
            Debug.LogError("GameInitializer requires a GameConfiguration asset.", this);
            return;
        }

        Debug.Log($"Game type :{configuration.CurrentMode}");

        switch (configuration.CurrentMode)
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

    private void OnGoalScored(TeamSide scoringTeam)
    {
        _spawner?.Respawn();
        scoreBoard.UpdateScore(scoringTeam);
    }
}
