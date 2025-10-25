using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenController : MonoBehaviour
{
    [SerializeField] ViewManager viewManager;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] GameObject mainCamera;

    private AsyncOperation _load;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        DontDestroyOnLoad(mainCamera);
    }

    private IEnumerator Start()
    {
        _load = SceneManager.LoadSceneAsync(Constants.SCENE_MENU, LoadSceneMode.Single);

        Instantiate(viewManager);

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
