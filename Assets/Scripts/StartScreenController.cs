using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenController : MonoBehaviour
{
    [SerializeField] ViewManager viewManager;
    [SerializeField] TextMeshProUGUI text;

    private AsyncOperation _load;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private IEnumerator Start()
    {
        Instantiate(viewManager);

        _load = SceneManager.LoadSceneAsync(SceneConstants.SCENE_MENU, LoadSceneMode.Single);

        if (!_load.isDone)
            yield return null;

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
