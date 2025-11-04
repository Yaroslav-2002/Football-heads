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
        isProcessing = false;

        if (hostCodeLabel != null)
        {
            hostCodeLabel.text = string.Empty;
        }
    }

    private void OnHostClicked()
    {
        if (isProcessing)
        {
            return;
        }

        isProcessing = true;

        if (!JoinCodeUtility.TryGetLocalIPv4(out var localAddress) || string.IsNullOrEmpty(localAddress))
        {
            localAddress = "127.0.0.1";
        }

        var joinCode = JoinCodeUtility.GenerateJoinCode(localAddress, GameConfiguration.HostPort);
        GameConfiguration.ConfigureHost("0.0.0.0", GameConfiguration.HostPort, joinCode);

        if (hostCodeLabel != null)
        {
            hostCodeLabel.text = $"Share this code: {joinCode}";
        }

        UpdateStatus("Starting host...");
        SceneManager.LoadSceneAsync(SceneConstants.SCENE_GAME);
    }

    private void OnJoinClicked()
    {
        if (isProcessing)
        {
            return;
        }

        var joinCodeText = joinCodeInputField != null ? joinCodeInputField.text : string.Empty;
        var joinCode = string.IsNullOrWhiteSpace(joinCodeText)
            ? string.Empty
            : joinCodeText.Trim().ToUpperInvariant();

        if (joinCodeInputField != null && joinCodeInputField.text != joinCode)
        {
            joinCodeInputField.text = joinCode;
        }

        if (!JoinCodeUtility.TryParseJoinCode(joinCode, out var address, out var port))
        {
            UpdateStatus("Invalid join code. Use letters A-Z and numbers 2-9.");
            return;
        }

        isProcessing = true;
        GameConfiguration.ConfigureClient(joinCode);
        UpdateStatus($"Joining {address}:{port}...");
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

