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
        EnsureRuntimeUI();

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

    private void EnsureRuntimeUI()
    {
        if (hostButton != null && joinButton != null && clearButton != null && backButton != null &&
            joinCodeDisplay != null && statusLabel != null && hostCodeLabel != null)
        {
            return;
        }

        var rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            rectTransform = gameObject.AddComponent<RectTransform>();
        }

        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = new Vector2(960f, 640f);
        rectTransform.anchoredPosition = Vector2.zero;

        if (GetComponent<CanvasRenderer>() == null)
        {
            gameObject.AddComponent<CanvasRenderer>();
        }

        var background = GetComponent<Image>();
        if (background == null)
        {
            background = gameObject.AddComponent<Image>();
        }

        background.color = new Color(0f, 0f, 0f, 0.75f);

        CreateTitle();

        hostButton = CreateButton("HostButton", new Vector2(0f, 200f), "HOST GAME");
        joinButton = CreateButton("JoinButton", new Vector2(0f, 60f), "JOIN GAME");
        clearButton = CreateButton("ClearButton", new Vector2(0f, -80f), "CLEAR CODE");
        backButton = CreateButton("BackButton", new Vector2(0f, -220f), "BACK");

        joinCodeDisplay = CreateLabel("JoinCodeDisplay", new Vector2(0f, -20f), 48f, TextAlignmentOptions.Midline);
        hostCodeLabel = CreateLabel("HostCodeLabel", new Vector2(0f, 300f), 40f, TextAlignmentOptions.Midline);
        statusLabel = CreateLabel("StatusLabel", new Vector2(0f, -320f), 36f, TextAlignmentOptions.Midline);
        statusLabel.color = new Color32(255, 255, 255, 200);

        joinCodeDisplay.text = "Enter join code";
        hostCodeLabel.text = string.Empty;
        statusLabel.text = string.Empty;
    }

    private void CreateTitle()
    {
        var title = CreateLabel("Title", new Vector2(0f, 300f), 72f, TextAlignmentOptions.Midline);
        title.text = "MULTIPLAYER";
        title.color = new Color32(255, 255, 255, 255);
    }

    private Button CreateButton(string name, Vector2 anchoredPosition, string label)
    {
        var buttonObject = new GameObject(name);
        buttonObject.transform.SetParent(transform, false);

        var rectTransform = buttonObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = new Vector2(520f, 110f);
        rectTransform.anchoredPosition = anchoredPosition;

        buttonObject.AddComponent<CanvasRenderer>();
        var image = buttonObject.AddComponent<Image>();
        image.sprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/UISprite.psd");
        image.type = Image.Type.Sliced;
        image.color = Color.white;

        var button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;

        var textComponent = CreateTextChild(buttonObject, label, 56f);
        textComponent.color = new Color32(50, 50, 50, 255);

        return button;
    }

    private TextMeshProUGUI CreateLabel(string name, Vector2 anchoredPosition, float fontSize, TextAlignmentOptions alignment)
    {
        var labelObject = new GameObject(name);
        labelObject.transform.SetParent(transform, false);

        var rectTransform = labelObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = new Vector2(800f, 100f);
        rectTransform.anchoredPosition = anchoredPosition;

        labelObject.AddComponent<CanvasRenderer>();
        return CreateTextComponent(labelObject, string.Empty, fontSize, alignment);
    }

    private TextMeshProUGUI CreateTextChild(GameObject parent, string label, float fontSize)
    {
        var child = new GameObject("Label");
        child.transform.SetParent(parent.transform, false);

        var rectTransform = child.AddComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = Vector2.zero;

        child.AddComponent<CanvasRenderer>();
        return CreateTextComponent(child, label, fontSize, TextAlignmentOptions.Midline);
    }

    private TextMeshProUGUI CreateTextComponent(GameObject owner, string content, float fontSize, TextAlignmentOptions alignment)
    {
        var text = owner.AddComponent<TextMeshProUGUI>();
        text.text = content;
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.enableWordWrapping = false;
        text.font = TMP_Settings.defaultFontAsset;
        text.color = new Color32(50, 50, 50, 255);
        return text;
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

