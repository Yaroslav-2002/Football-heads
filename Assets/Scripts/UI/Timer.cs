using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _text;
    [SerializeField] private float _remainingTime;
    public float RemainingTime
    {
        get => _remainingTime;
        set => _remainingTime = value;
    }

    private void Update()
    {
        if(_remainingTime <= 0)
        {
            _remainingTime = 0;
        }
        else
        {
            _remainingTime -= Time.deltaTime;
        }
        
        int minutes = Mathf.FloorToInt(_remainingTime / 60);
        int seconds = Mathf.FloorToInt(_remainingTime % 60);
        _text.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
