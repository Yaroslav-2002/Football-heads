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
    [SerializeField] private Button clearButton;
    [SerializeField] private Button backButton;
    [SerializeField] private TextMeshProUGUI joinCodeDisplay;
    [SerializeField] private TextMeshProUGUI statusLabel;
    [SerializeField] private TextMeshProUGUI hostCodeLabel;
    [SerializeField] private int maxJoinCodeLength = 12;

    private readonly StringBuilder joinCodeBuilder = new();
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

        if (clearButton != null)
        {
            clearButton.onClick.AddListener(ClearJoinCode);
        }

        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackClicked);
        }

        UpdateJoinCodeDisplay();
        UpdateStatus(string.Empty);
    }

    public override void Show()
    {
        base.Show();
        isProcessing = false;
        ClearJoinCode();
        UpdateStatus(string.Empty);
        if (hostCodeLabel != null)
        {
            hostCodeLabel.text = string.Empty;
        }
    }

    private void Update()
    {
        if (!isActiveAndEnabled || isProcessing)
        {
            return;
        }

        var keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return;
        }

        if (keyboard.backspaceKey.wasPressedThisFrame || keyboard.deleteKey.wasPressedThisFrame)
        {
            if (joinCodeBuilder.Length > 0)
            {
                joinCodeBuilder.Length--;
                UpdateJoinCodeDisplay();
            }
        }

        ProcessCharacterKeys(keyboard);
    }

    private void ProcessCharacterKeys(Keyboard keyboard)
    {
        foreach (var keyControl in keyboard.allKeys)
        {
            if (!keyControl.wasPressedThisFrame)
            {
                continue;
            }

            if (TryGetCharacterForKey(keyControl.keyCode, out var character))
            {
                HandleCharacterInput(character);
            }
        }
    }

    private void HandleCharacterInput(char character)
    {
        if (!isActiveAndEnabled || isProcessing)
        {
            return;
        }

        if (char.IsControl(character))
        {
            return;
        }

        char normalizedCharacter = char.ToUpperInvariant(character);
        if (!JoinCodeUtility.IsValidCharacter(normalizedCharacter))
        {
            return;
        }

        if (joinCodeBuilder.Length >= maxJoinCodeLength)
        {
            return;
        }

        joinCodeBuilder.Append(normalizedCharacter);
        UpdateJoinCodeDisplay();
    }

    private static bool TryGetCharacterForKey(Key keyCode, out char character)
    {
        switch (keyCode)
        {
            case Key.A: character = 'A'; return true;
            case Key.B: character = 'B'; return true;
            case Key.C: character = 'C'; return true;
            case Key.D: character = 'D'; return true;
            case Key.E: character = 'E'; return true;
            case Key.F: character = 'F'; return true;
            case Key.G: character = 'G'; return true;
            case Key.H: character = 'H'; return true;
            case Key.I: character = 'I'; return true;
            case Key.J: character = 'J'; return true;
            case Key.K: character = 'K'; return true;
            case Key.L: character = 'L'; return true;
            case Key.M: character = 'M'; return true;
            case Key.N: character = 'N'; return true;
            case Key.O: character = 'O'; return true;
            case Key.P: character = 'P'; return true;
            case Key.Q: character = 'Q'; return true;
            case Key.R: character = 'R'; return true;
            case Key.S: character = 'S'; return true;
            case Key.T: character = 'T'; return true;
            case Key.U: character = 'U'; return true;
            case Key.V: character = 'V'; return true;
            case Key.W: character = 'W'; return true;
            case Key.X: character = 'X'; return true;
            case Key.Y: character = 'Y'; return true;
            case Key.Z: character = 'Z'; return true;
            case Key.Digit0:
            case Key.Numpad0:
                character = '0';
                return true;
            case Key.Digit1:
            case Key.Numpad1:
                character = '1';
                return true;
            case Key.Digit2:
            case Key.Numpad2:
                character = '2';
                return true;
            case Key.Digit3:
            case Key.Numpad3:
                character = '3';
                return true;
            case Key.Digit4:
            case Key.Numpad4:
                character = '4';
                return true;
            case Key.Digit5:
            case Key.Numpad5:
                character = '5';
                return true;
            case Key.Digit6:
            case Key.Numpad6:
                character = '6';
                return true;
            case Key.Digit7:
            case Key.Numpad7:
                character = '7';
                return true;
            case Key.Digit8:
            case Key.Numpad8:
                character = '8';
                return true;
            case Key.Digit9:
            case Key.Numpad9:
                character = '9';
                return true;
            default:
                character = default;
                return false;
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

        if (!JoinCodeUtility.TryParseJoinCode(joinCodeBuilder.ToString(), out var address, out var port))
        {
            UpdateStatus("Invalid join code. Use letters A-Z and numbers 2-9.");
            return;
        }

        isProcessing = true;
        GameConfiguration.ConfigureClient(joinCodeBuilder.ToString());
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

    private void ClearJoinCode()
    {
        joinCodeBuilder.Clear();
        UpdateJoinCodeDisplay();
        UpdateStatus(string.Empty);
    }

    private void UpdateJoinCodeDisplay()
    {
        if (joinCodeDisplay == null)
        {
            return;
        }

        joinCodeDisplay.text = joinCodeBuilder.Length == 0 ? "Enter join code" : joinCodeBuilder.ToString();
    }

    private void UpdateStatus(string message)
    {
        if (statusLabel != null)
        {
            statusLabel.text = message;
        }
    }
}

