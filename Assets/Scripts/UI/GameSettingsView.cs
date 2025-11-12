using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSettingsView : View
{
    [SerializeField] private Button restartGameButton;
    [SerializeField] private Button exitToMenuButton;
    [SerializeField] private Button resumeButton;
    private AsyncOperation _load;

    public Action OnRestart;

    public override void Init()
    {
        if (restartGameButton != null)
        {
            restartGameButton.onClick.AddListener(() => OnRestartButtonClicked());
        }

        if (exitToMenuButton != null)
        {
            exitToMenuButton.onClick.AddListener(() => StartCoroutine(OnExitButtonClicked()));
        }

        if (exitToMenuButton != null)
        {
            resumeButton.onClick.AddListener(() => OnResumeButtonClicked());
        }
    }

    private IEnumerator OnExitButtonClicked()
    {
        ShutdownActiveNetworkSession();

        _load = SceneManager.LoadSceneAsync(SceneConstants.SCENE_MENU, LoadSceneMode.Single);

        if (!_load.isDone)
            yield return null;

        ViewManager.Show<MainMenuView>();
    }

    private static void ShutdownActiveNetworkSession()
    {
        var networkManager = NetworkManager.Singleton;
        if (networkManager == null)
        {
            return;
        }

        if (networkManager.IsListening || networkManager.IsClient)
        {
            networkManager.Shutdown();
        }

        if (networkManager.gameObject != null)
        {
            UnityEngine.Object.Destroy(networkManager.gameObject);
        }
    }

    private void OnRestartButtonClicked()
    {
        OnRestart.Invoke();
        ViewManager.Show<GUIView>();
    }

    private void OnResumeButtonClicked()
    {
        ViewManager.Show<GUIView>();
    }
}
