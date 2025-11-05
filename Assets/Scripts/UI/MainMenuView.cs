using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuView : View
{
    [SerializeField] Button exitButton;
    [SerializeField] Button settingsButton;
    [SerializeField] Button startButton;
    [SerializeField] Button multiplayerButton;
    [SerializeField] private GameConfiguration configuration;

    public override void Init()
    {
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(OnExitButtonClick);
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(() => ViewManager.Show<SettingsView>());
        }

        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClicked);
        }

        if (multiplayerButton != null)
        {
            multiplayerButton.onClick.AddListener(OnMultiplayerButtonClicked);
        }
    }

    private void OnStartButtonClicked()
    {
        if (configuration == null)
        {
            Debug.LogError("MainMenuView requires a GameConfiguration asset.", this);
            return;
        }

        configuration.SetGameMode(GameMode.Singleplayer);
        configuration.ResetNetworkConfiguration();
        SceneManager.LoadSceneAsync(SceneConstants.SCENE_GAME);
    }

    private void OnMultiplayerButtonClicked()
    {
        ViewManager.Show<MultiplayerMenuView>();
    }

    void OnExitButtonClick()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
