using UnityEngine;

public abstract class GameInitializerBase : MonoBehaviour
{
    [SerializeField] private EntitySpawnerBase spawnerPrefab;
    [SerializeField] private ScoreBoard scoreBoard;
    [SerializeField] private GoalTrigger leftTrigger;
    [SerializeField] private GoalTrigger rightTrigger;
    [SerializeField] private GameConfiguration configuration;

    private EntitySpawnerBase SpawnerPrefab;

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
            Debug.LogError("GameInitializerBase requires a GameConfiguration asset.", this);
            return;
        }

        Debug.Log($"Game type :{configuration.CurrentMode}");

        SpawnerPrefab = Instantiate(spawnerPrefab);

        SpawnerPrefab.Init();
    }

    private void OnGoalScored(TeamSide scoringTeam)
    {
        SpawnerPrefab?.Respawn();
        scoreBoard.UpdateScore(scoringTeam);
    }
}
