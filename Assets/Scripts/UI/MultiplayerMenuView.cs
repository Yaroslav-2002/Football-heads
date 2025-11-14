using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MultiplayerMenuView : View
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private Button backButton;
    [SerializeField] private TMP_InputField joinCodeInputField;
    [SerializeField] private TextMeshProUGUI statusLabel;
    [SerializeField] private TextMeshProUGUI hostCodeLabel;
    [SerializeField] GameConfiguration GameConfiguration;

    private bool isProcessing;

    public override void Init()
    {

        if (hostButton != null)
        {
            hostButton.onClick.AddListener(OnHostClicked);
        }

        if (joinButton != null)
        {
            joinButton.onClick.AddListener(OnJoinClicked);
        }

        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackClicked);
        }

        UpdateStatus(string.Empty);
    }

    public override void Show()
    {
        base.Show();

        if (hostCodeLabel != null)
        {
            hostCodeLabel.text = string.Empty;
        }
    }

    private void OnHostClicked()
    {
        
        SceneManager.LoadSceneAsync(SceneConstants.SCENE_GAME);
    }

    private void OnJoinClicked()
    {
        
        SceneManager.LoadSceneAsync(SceneConstants.SCENE_GAME);
    }

    private void OnBackClicked()
    {
        if (isProcessing)
        {
            return;
        }

        ViewManager.ShowLast();
    }

    private void UpdateStatus(string message)
    {
        if (statusLabel != null)
        {
            statusLabel.text = message;
        }
    }
}

