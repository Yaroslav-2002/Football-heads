using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSettingsView : View
{
    [SerializeField] Button restartGameButton;
    [SerializeField] Button exitToMenuButton;
    private EntitySpawner spawner;

    public override void Init()
    {
        if (restartGameButton != null)
        {
            restartGameButton.onClick.AddListener(() => OnRestartButtonClicked());
        }

        if (exitToMenuButton != null)
        {
            exitToMenuButton.onClick.AddListener(() => SceneManager.LoadSceneAsync(SceneNames.Map[SceneType.Menu]));
        }

        if(spawner == null)
            spawner = GameObject.FindAnyObjectByType<EntitySpawner>();
    }

    private void OnRestartButtonClicked()
    {
        spawner?.Respawn();
        Hide();
    }
}
