using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuView : View
{
    [SerializeField] Button exitButton;
    [SerializeField] Button settingsButton;
    [SerializeField] Button startButton;
    [SerializeField] Button hostButton;
    [SerializeField] Button joinButton;

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
        GameConfiguration.CurrentMode = GameMode.Singleplayer;
        SceneManager.LoadSceneAsync(SceneConstants.SCENE_GAME);
    }

    private void OnMultiplayerButtonClicked()
    {
        GameConfiguration.CurrentMode = GameMode.Multiplayer;
        SceneManager.LoadSceneAsync(SceneConstants.SCENE_GAME);
    }

    void OnExitButtonClick()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
