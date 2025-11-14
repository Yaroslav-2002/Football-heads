using UnityEngine;

[DisallowMultipleComponent]
public class PlayerInputUIController : MonoBehaviour
{
    [SerializeField] private GameobjectsRuntimeSet gameobjectsRuntimeSet;
    [SerializeField] private PlayerInput.ControlScheme controlScheme = PlayerInput.ControlScheme.PlayerOne;
    [SerializeField] private UIButtonHoldHandler jumpButton;
    [SerializeField] private UIButtonHoldHandler kickButton;
    [SerializeField] private UIButtonHoldHandler moveLeftButton;
    [SerializeField] private UIButtonHoldHandler moveRightButton;

    private PlayerInput _playerInput;

    private void Awake()
    {
#if UNITY_STANDALONE_WIN
        gameObject.SetActive(true);
#else
        gameObject.SetActive(false);
#endif
    }

    private void OnEnable()
    {
        Subscribe();
        TryResolvePlayerInput();
    }

    private void OnDisable()
    {
        Unsubscribe();
        ResetHeldInputs();
    }

    private void Update()
    {
        if (_playerInput != null && _playerInput.isActiveAndEnabled && _playerInput.CurrentScheme == controlScheme)
        {
            return;
        }

        TryResolvePlayerInput();
    }

    private void Subscribe()
    {
        if (jumpButton != null)
        {
            jumpButton.Pressed += OnJumpButtonPressed;
        }

        if (kickButton != null)
        {
            kickButton.Pressed += OnKickButtonPressed;
            kickButton.Released += OnKickButtonReleased;
        }

        if (moveLeftButton != null)
        {
            moveLeftButton.Pressed += OnMoveLeftButtonPressed;
            moveLeftButton.Released += OnMoveLeftButtonReleased;
        }

        if (moveRightButton != null)
        {
            moveRightButton.Pressed += OnMoveRightButtonPressed;
            moveRightButton.Released += OnMoveRightButtonReleased;
        }
    }

    private void Unsubscribe()
    {
        if (jumpButton != null)
        {
            jumpButton.Pressed -= OnJumpButtonPressed;
        }

        if (kickButton != null)
        {
            kickButton.Pressed -= OnKickButtonPressed;
            kickButton.Released -= OnKickButtonReleased;
        }

        if (moveLeftButton != null)
        {
            moveLeftButton.Pressed -= OnMoveLeftButtonPressed;
            moveLeftButton.Released -= OnMoveLeftButtonReleased;
        }

        if (moveRightButton != null)
        {
            moveRightButton.Pressed -= OnMoveRightButtonPressed;
            moveRightButton.Released -= OnMoveRightButtonReleased;
        }
    }

    private void TryResolvePlayerInput()
    {
        if (_playerInput != null)
        {
            if (_playerInput.isActiveAndEnabled && _playerInput.CurrentScheme == controlScheme)
            {
                return;
            }

            ReleaseHeldInputs(_playerInput);
            _playerInput = null;
        }

        if (gameobjectsRuntimeSet != null)
        {
            foreach (GameObject item in gameobjectsRuntimeSet.items)
            {
                if (item == null)
                {
                    continue;
                }

                if (item.TryGetComponent(out PlayerInput input) && input.CurrentScheme == controlScheme)
                {
                    _playerInput = input;
                    break;
                }
            }
        }

        if (_playerInput != null)
        {
            return;
        }

        foreach (PlayerInput input in FindObjectsOfType<PlayerInput>())
        {
            if (input.CurrentScheme != controlScheme)
            {
                continue;
            }

            if (!input.isActiveAndEnabled)
            {
                continue;
            }

            _playerInput = input;
            break;
        }
    }

    private bool EnsurePlayerInput()
    {
        if (_playerInput != null && _playerInput.isActiveAndEnabled && _playerInput.CurrentScheme == controlScheme)
        {
            return true;
        }

        TryResolvePlayerInput();
        return _playerInput != null && _playerInput.isActiveAndEnabled && _playerInput.CurrentScheme == controlScheme;
    }

    private void ResetHeldInputs()
    {
        if (_playerInput == null)
        {
            return;
        }

        ReleaseHeldInputs(_playerInput);
    }

    private static void ReleaseHeldInputs(PlayerInput input)
    {
        if (input == null)
        {
            return;
        }

        input.StopMoveLeftFromUI();
        input.StopMoveRightFromUI();
        input.StopKickFromUI();
    }

    private void OnJumpButtonPressed()
    {
        if (!EnsurePlayerInput())
        {
            return;
        }

        _playerInput.JumpFromUI();
    }

    private void OnKickButtonPressed()
    {
        if (!EnsurePlayerInput())
        {
            return;
        }

        _playerInput.StartKickFromUI();
    }

    private void OnKickButtonReleased()
    {
        if (!EnsurePlayerInput())
        {
            return;
        }

        _playerInput.StopKickFromUI();
    }

    private void OnMoveLeftButtonPressed()
    {
        if (!EnsurePlayerInput())
        {
            return;
        }

        _playerInput.StartMoveLeftFromUI();
    }

    private void OnMoveLeftButtonReleased()
    {
        if (!EnsurePlayerInput())
        {
            return;
        }

        _playerInput.StopMoveLeftFromUI();
    }

    private void OnMoveRightButtonPressed()
    {
        if (!EnsurePlayerInput())
        {
            return;
        }

        _playerInput.StartMoveRightFromUI();
    }

    private void OnMoveRightButtonReleased()
    {
        if (!EnsurePlayerInput())
        {
            return;
        }

        _playerInput.StopMoveRightFromUI();
    }
}
