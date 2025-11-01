using System;
using TMPro;
using UnityEngine;

public class ScoreBoard : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreLeft;
    [SerializeField] TextMeshProUGUI scoreRight;

    private int _playerLeftPoints;
    private int _playerRightPoints;

    private void Awake()
    {
        _playerLeftPoints = 0;
        _playerRightPoints = 0;
    }

    public void SetScore(int scoreLeftPoints, int scoreRightPoints)
    {
        scoreLeft.text = scoreLeftPoints.ToString();
        scoreRight.text = scoreRightPoints.ToString();
    }

    internal void UpdateScore(TeamSide scoringTeam)
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
