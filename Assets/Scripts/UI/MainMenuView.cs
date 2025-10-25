using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuView : View
{
    [SerializeField] Button exitButton;
    [SerializeField] Button settingsButton;
    [SerializeField] Button startButton;

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
    }

    private void OnStartButtonClicked()
    {
        SceneManager.LoadSceneAsync(SceneNames.Map[SceneType.Game]);
    }

    void OnExitButtonClick()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
