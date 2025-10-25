using System.Collections;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenController : MonoBehaviour
{
    [SerializeField] ViewManager viewManager;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] GameObject camera;

    private AsyncOperation _load;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        DontDestroyOnLoad(camera);
    }

    private IEnumerator Start()
    {
        _load = SceneManager.LoadSceneAsync((int)SceneType.Menu, LoadSceneMode.Single);

        Instantiate(viewManager);

        if (_load.isDone)
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
