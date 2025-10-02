using TMPro;
using UnityEngine;

public class ScoreBoard : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _scoreLeft;
    [SerializeField] TextMeshProUGUI _scoreRight;

    private void Awake()
    {
        EnsureReferences();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            EnsureReferences();
        }
    }
#endif

    public void SetScore(int scoreLeft, int scoreRight)
    {
        EnsureReferences();

        if (_scoreLeft != null)
        {
            _scoreLeft.text = scoreLeft.ToString();
        }

        if (_scoreRight != null)
        {
            _scoreRight.text = scoreRight.ToString();
        }
    }

    private void EnsureReferences()
    {
        if (_scoreLeft != null && _scoreRight != null)
        {
            return;
        }

        TextMeshProUGUI left = null;
        TextMeshProUGUI right = null;

        foreach (Transform child in transform)
        {
            TextMeshProUGUI candidate = child.GetComponent<TextMeshProUGUI>();
            if (candidate == null)
            {
                continue;
            }

            if (left == null || candidate.rectTransform.anchoredPosition.x < left.rectTransform.anchoredPosition.x)
            {
                left = candidate;
            }

            if (right == null || candidate.rectTransform.anchoredPosition.x > right.rectTransform.anchoredPosition.x)
            {
                right = candidate;
            }
        }

        _scoreLeft ??= left;
        _scoreRight ??= right;
    }
}
