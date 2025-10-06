using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuView : View
{
    private const int MAIN_SCENE_NUM = 1;
    [SerializeField] Button exitButton;
    [SerializeField] Button settingsButton;
    [SerializeField] Button startButton;

    public override void Init()
    {
        exitButton.onClick.AddListener(OnExitButtonClick);
        settingsButton.onClick.AddListener(() => ViewManager.Show<SettingsView>());
        
        startButton.onClick.AddListener(() => OnStartButtonClicked());
    }

    private void OnStartButtonClicked()
    {
        SceneManager.LoadSceneAsync(MAIN_SCENE_NUM);
    }

    void OnExitButtonClick()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
