using System;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _text;
    [SerializeField] private float _remainingTime;

    public Action OnGameTimeExpired;
    private bool _hasExpired;

    public float RemainingTime
    {
        get => _remainingTime;
        set
        {
            _remainingTime = value;
            if (_remainingTime > 0f)
            {
                _hasExpired = false;
            }
        }
    }

    private bool _pause;
    public void Pause()
    {
        _pause = true;
    }

    public void UnPause()
    {
        _pause = false;
    }

    private void Update()
    {
        if(_remainingTime <= 0)
        {
            if (!_hasExpired)
            {
                _remainingTime = 0;
                _hasExpired = true;
                OnGameTimeExpired?.Invoke();
            }
        }
        else
        {
            if(!_pause)
                _remainingTime -= Time.deltaTime;
        }
        
        int minutes = Mathf.FloorToInt(_remainingTime / 60);
        int seconds = Mathf.FloorToInt(_remainingTime % 60);
        _text.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
