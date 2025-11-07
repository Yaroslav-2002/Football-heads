using System;
using UnityEngine;

public class GoalManager : MonoBehaviour
{
    [SerializeField] private GoalTrigger leftTrigger;
    [SerializeField] private GoalTrigger rightTrigger;

    public Action<TeamSide> GoalScored;

    private void OnGoalScored(TeamSide scoringTeam)
    {
        GoalScored.Invoke(scoringTeam);
    }

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
}
