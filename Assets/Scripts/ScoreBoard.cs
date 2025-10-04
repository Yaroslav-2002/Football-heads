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

    internal void UpdateScore(int gateId)
    {
        switch (gateId) {
            case 0:
                _playerLeftPoints += 1;
                break;
            case 1:
                _playerRightPoints += 1;
                break;
        }

        SetScore(_playerLeftPoints, _playerRightPoints);
    }
}
