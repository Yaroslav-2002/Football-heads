using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [SerializeField] private EntitySpawner spawner;
    [SerializeField] private ScoreBoard scoreBoard;
    [SerializeField] private GoalTrigger leftTrigger;
    [SerializeField] private GoalTrigger rightTrigger;

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
        spawner.SpawnPlayers();
        spawner.SpawnBall();
    }

    private void OnGoalScored(TeamSide scoringTeam)
    {
        spawner.Respawn();
        scoreBoard.UpdateScore(scoringTeam);
    }
}
