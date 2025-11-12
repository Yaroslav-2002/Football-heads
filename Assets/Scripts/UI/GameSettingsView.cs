using System;
using System.Collections;
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
        _load = SceneManager.LoadSceneAsync(SceneConstants.SCENE_MENU, LoadSceneMode.Single);

        if (!_load.isDone)
            yield return null;

        ViewManager.Show<MainMenuView>();
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
