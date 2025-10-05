using System;
using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    [SerializeField] private TeamSide scoringTeam;
    [SerializeField] private ScoreBoard scoreBoard;

    public Action<TeamSide> OnGoalScored;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ball"))
        {
            scoreBoard.UpdateScore(scoringTeam);
            OnGoalScored(scoringTeam);
        }
    }
}