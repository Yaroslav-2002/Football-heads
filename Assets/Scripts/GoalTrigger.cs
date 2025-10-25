using System;
using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    [SerializeField] private TeamSide scoringTeam;

    public Action<TeamSide> OnGoalScored;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ball"))
        {
            OnGoalScored(scoringTeam);
        }
    }
}