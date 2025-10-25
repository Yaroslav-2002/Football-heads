using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSettingsView : View
{
    [SerializeField] Button restartGameButton;
    [SerializeField] Button exitToMenuButton;
    private EntitySpawner spawner;
    private AsyncOperation _load;

    public override void Init()
    {
        if (restartGameButton != null)
        {
            restartGameButton.onClick.AddListener(() => OnRestartButtonClicked());
        }

        if (exitToMenuButton != null)
        {
            exitToMenuButton.onClick.AddListener(() => OnExitButtonClicked());
        }

        if(spawner == null)
            spawner = FindAnyObjectByType<EntitySpawner>();
    }

    private IEnumerator OnExitButtonClicked()
    {
        _load = SceneManager.LoadSceneAsync(Constants.SCENE_MENU, LoadSceneMode.Single);

        if (_load.isDone)
            yield return null;
        
        ViewManager.Show<MainMenuView>();
    }

    private void OnRestartButtonClicked()
    {
        spawner?.Respawn();
        Hide();
    }
}
