using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;

    private AsyncOperation _load;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private IEnumerator Start()
    {
        _load = SceneManager.LoadSceneAsync(SceneConstants.SCENE_MENU, LoadSceneMode.Single);

        while (!_load.isDone)
        {
            yield return null;
        }

        Destroy(gameObject);
    }

    private float _lastProgressBarTime;
    private void Update()
    {
        if (Time.time - _lastProgressBarTime > 1f)
        {
            text.text += ".";
            _lastProgressBarTime = Time.time;
        }
    }
}
