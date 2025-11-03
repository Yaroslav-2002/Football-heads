using UnityEngine;

public abstract class GameInitializerBase : MonoBehaviour
{
    [SerializeField] private EntitySpawnerBase spawnerPrefab;
    [SerializeField] private ScoreBoard scoreBoard;
    [SerializeField] private GoalTrigger leftTrigger;
    [SerializeField] private GoalTrigger rightTrigger;

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
        Debug.Log($"Game type :{GameConfiguration.CurrentMode}");

        SpawnerPrefab = Instantiate(spawnerPrefab);

        SpawnerPrefab.Init();
    }

    private void OnGoalScored(TeamSide scoringTeam)
    {
        SpawnerPrefab?.Respawn();
        scoreBoard.UpdateScore(scoringTeam);
    }
}
