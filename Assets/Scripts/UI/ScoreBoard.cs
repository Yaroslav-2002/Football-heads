using System;
using TMPro;
using UnityEngine;

public class ScoreBoard : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _scoreText;
    [SerializeField] private GoalManager goalManager;

    private int _playerLeftPoints;
    private int _playerRightPoints;

    private void Awake()
    {
        _playerLeftPoints = 0;
        _playerRightPoints = 0;
    }

    private void OnEnable()
    {
        goalManager.GoalScored += OnGoalScored;
    }

    private void OnDisable()
    {
        goalManager.GoalScored -= OnGoalScored;
    }

    public void SetScore(int scoreLeftPoints, int scoreRightPoints)
    {
        string scoreText = $"{scoreLeftPoints}:{scoreRightPoints}";
        _scoreText.text = scoreText;
    }

    internal void OnGoalScored(TeamSide scoringTeam)
    {
        switch (scoringTeam)
        {
            case TeamSide.Left:
                _playerLeftPoints += 1;
                break;
            case TeamSide.Right:
                _playerRightPoints += 1;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(scoringTeam), scoringTeam, null);
        }

        SetScore(_playerLeftPoints, _playerRightPoints);
    }
}
