using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    private static GameInitializer _instance;

    [SerializeField] private BoxCollider2D gatesColliderLeft;
    [SerializeField] private BoxCollider2D gatesColliderRight;
    [SerializeField] private EntitySpawner spawner;
    [SerializeField] private ScoreBoard scoreBoard;

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
}
