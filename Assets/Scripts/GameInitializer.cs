using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [SerializeField] private NetworkEntitySpawner multiplayerSpawner;
    [SerializeField] private EntitySpawner singleplayerSpawner;
    [SerializeField] private ScoreBoard scoreBoard;
    [SerializeField] private GoalTrigger leftTrigger;
    [SerializeField] private GoalTrigger rightTrigger;

    private EntitySpawnerBase Spawner =>  GameConfiguration.CurrentMode == GameMode.Multiplayer ? multiplayerSpawner : singleplayerSpawner;

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
        Instantiate(Spawner);
        Spawner.Init();
    }

    private void OnGoalScored(TeamSide scoringTeam)
    {
        Spawner.Respawn();
        scoreBoard.UpdateScore(scoringTeam);
    }
}
