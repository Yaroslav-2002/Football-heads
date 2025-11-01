using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [SerializeField] private NetworkEntitySpawner multiplayerSpawner;
    [SerializeField] private EntitySpawner singleplayerSpawner;
    [SerializeField] private ScoreBoard scoreBoard;
    [SerializeField] private GoalTrigger leftTrigger;
    [SerializeField] private GoalTrigger rightTrigger;

    private EntitySpawnerBase SpawnerPrefab => GameConfiguration.CurrentMode == GameMode.Multiplayer ? multiplayerSpawner : singleplayerSpawner;

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
        if (SpawnerPrefab != null)
        {
            return;
        }

        SpawnerPrefab.Init();
    }

    private void OnGoalScored(TeamSide scoringTeam)
    {
        SpawnerPrefab?.Respawn();
        scoreBoard.UpdateScore(scoringTeam);
    }
}
