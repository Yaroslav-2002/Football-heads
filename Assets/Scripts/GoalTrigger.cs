using System;
using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    [SerializeField] private TeamSide scoringTeam;
    private const string BallName = "Ball";

    public Action<TeamSide> OnGoalScored;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(BallName))
        {
            OnGoalScored(scoringTeam);
        }
    }
}