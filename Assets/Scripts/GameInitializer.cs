using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    private static GameInitializer _instance;

    [SerializeField] private EntitySpawner spawner;
    [SerializeField] private ScoreBoard scoreBoard;
    [SerializeField] private GoalTrigger leftTrigger;
    [SerializeField] private GoalTrigger rightTrigger;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        leftTrigger.OnGoalScored += Restart;
        rightTrigger.OnGoalScored += Restart;
    }

    private void OnDisable()
    {
        leftTrigger.OnGoalScored -= Restart;
        rightTrigger.OnGoalScored -= Restart;
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

    internal void GoalScored(TeamSide scoringTeam)
    {
        scoreBoard.UpdateScore(scoringTeam);
    }

    private void Restart(TeamSide scoringTeam)
    {
        spawner.Respawn();
    }
}
