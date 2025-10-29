using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ViewManager : MonoBehaviour
{
    private static ViewManager instance;
    [SerializeField] private View startingView;
    [SerializeField] private View[] views;
    [SerializeField] private Camera cameraPrefab;

    private Camera _camera;

    private View currentView;

    private readonly Stack<View> history = new();

    private void Awake()
    {
        instance = this;

        if (views == null || views.Length == 0)
        {
            Debug.LogWarning("ViewManager has no registered views.");
        }
        else
        {
            foreach (var view in views)
            {
                if (view == null)
                {
                    continue;
                }

                view.Initialize();
                view.Hide();
            }
        }

        DontDestroyOnLoad(gameObject);

        if (cameraPrefab)
        {
            InitCamera();
        }

        if (startingView != null)
        {
            Show(startingView, false);
        }
    }
    private void InitCamera()
    {
        _camera = Instantiate(cameraPrefab);
        var canvas = GetComponent<Canvas>();
        canvas.worldCamera = _camera;
        canvas.planeDistance = _camera.farClipPlane / 2;

        DontDestroyOnLoad(_camera);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == SceneConstants.SCENE_MAIN)
        {
            Show<GUI>();
        }
    }

    public static T GetView<T>() where T : View
    {
        if (instance == null)
        {
            Debug.LogError("ViewManager is not initialised yet.");
            return null;
        }

        foreach (var view in instance.views)
        {
            if (view is T typedView)
            {
                return typedView;
            }
        }

        Debug.LogWarning($"No such view {typeof(T).Name}");
        return null;
    }

    public static void Show<T>(bool remember = true) where T : View
    {
        if (instance == null)
        {
            Debug.LogError("ViewManager is not initialised yet.");
            return;
        }

        foreach (var view in instance.views)
        {
            if (view is T typedView)
            {
                ShowInternal(typedView, remember);
                return;
            }
        }

        Debug.LogWarning($"No such view {typeof(T).Name}");
    }

    public static void Show(View view, bool remember = true)
    {
        if (instance == null)
        {
            Debug.LogError("ViewManager is not initialised yet.");
            return;
        }

        if (view == null)
        {
            Debug.LogError("Trying to show a null view.");
            return;
        }

        ShowInternal(view, remember);
    }

    public static void ShowLast()
    {
        if (instance == null)
        {
            Debug.LogError("ViewManager is not initialised yet.");
            return;
        }

        if (instance.history.Count == 0)
        {
            return;
        }

        Show(instance.history.Pop(), false);
    }

    private static void ShowInternal(View view, bool remember)
    {
        if (instance.currentView != null)
        {
            if (remember)
            {
                instance.history.Push(instance.currentView);
            }

            instance.currentView.Hide();
        }

        view.Initialize();
        view.Show();
        instance.currentView = view;
    }
}

