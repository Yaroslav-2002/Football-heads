using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSettingsView : View
{
    [SerializeField] private Button restartGameButton;
    [SerializeField] private Button exitToMenuButton;
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
}
