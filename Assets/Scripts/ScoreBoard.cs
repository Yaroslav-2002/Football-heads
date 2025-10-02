using TMPro;
using UnityEngine;

public class ScoreBoard : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _scoreLeft;
    [SerializeField] TextMeshProUGUI _scoreRight;

    public void SetScore(int scoreLeft, int scoreRight)
    {
        _scoreLeft.text = scoreLeft.ToString();
        _scoreRight.text = scoreRight.ToString();
    }
}
