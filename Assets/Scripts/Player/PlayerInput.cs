using System;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class PlayerInput : MonoBehaviour
{
    private InputActions _inputActions;

    public enum ControlScheme
    {
        PlayerOne,
        PlayerTwo
    }

    private ControlScheme _controlScheme;
    private IControllable _controllable;
    private InputActionMap _activeActionMap;
    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _kickAction;
    private bool _isInitialized;
    private bool _uiMoveLeftHeld;
    private bool _uiMoveRightHeld;

    private void Awake()
    {
        _controllable = GetComponent<IControllable>();
        if (_controllable == null)
        {
            Debug.LogError($"PlayerInput: No IControllable implementation found on {name}.", this);
            enabled = false;
            return;
        }
        _inputActions = new InputActions();

        Configure(_controlScheme);
    }

    private void OnEnable()
    {
        if (!_isInitialized)
        {
            return;
        }

        EnableActions();
    }

    private void OnDisable()
    {
        if (!_isInitialized)
        {
            return;
        }

        DisableActions();
    }

    private void OnDestroy()
    {
        if (_activeActionMap != null)
        {
            DisableActions();
        }

        _inputActions?.Dispose();
    }

    public void Configure(ControlScheme scheme)
    {
        bool shouldReactivate = _isInitialized && enabled && gameObject.activeInHierarchy;
        if (shouldReactivate)
        {
            DisableActions();
        }

        _controlScheme = scheme;
        (_activeActionMap, _moveAction, _jumpAction, _kickAction) = ResolveActions(scheme);
        _isInitialized = _activeActionMap != null;

        if (shouldReactivate)
        {
            EnableActions();
        }
    }

    private void Update()
    {
        if (!_isInitialized)
        {
            return;
        }

        Vector2 moveInput = _moveAction.ReadValue<Vector2>();
        float uiMoveInput = GetUIMoveInput();
        float horizontalInput = Mathf.Approximately(uiMoveInput, 0f) ? moveInput.x : uiMoveInput;
        _controllable.Move(horizontalInput);
    }

    private void EnableActions()
    {
        if (!_isInitialized)
        {
            return;
        }

        _activeActionMap.Enable();
        _jumpAction.performed += OnJumpPerformed;
        _kickAction.started += OnKickStarted;
        _kickAction.canceled += OnKickCanceled;
    }

    private void DisableActions()
    {
        if (!_isInitialized)
        {
            return;
        }

        _jumpAction.performed -= OnJumpPerformed;
        _kickAction.started -= OnKickStarted;
        _kickAction.canceled -= OnKickCanceled;
        _activeActionMap.Disable();
    }

    private (InputActionMap map, InputAction move, InputAction jump, InputAction kick) ResolveActions(ControlScheme scheme)
    {
        InputActions.PlayerOneActions playerOne = _inputActions.PlayerOne;
        InputActions.PlayerTwoActions playerTwo = _inputActions.PlayerTwo;

        return scheme switch
        {
            ControlScheme.PlayerOne => (playerOne.Get(), playerOne.Move, playerOne.Jump, playerOne.Kick),
            ControlScheme.PlayerTwo => (playerTwo.Get(), playerTwo.Move, playerTwo.Jump, playerTwo.Kick),
            _ => throw new ArgumentOutOfRangeException(nameof(scheme), scheme, null)
        };
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        _controllable.Jump();
    }

    private void OnKickStarted(InputAction.CallbackContext context)
    {
        _controllable.StartKick();
    }

    private void OnKickCanceled(InputAction.CallbackContext context)
    {
        _controllable.ReleaseKick();
    }

    public ControlScheme CurrentScheme => _controlScheme;

    public void StartMoveLeftFromUI()
    {
        _uiMoveLeftHeld = true;
    }

    public void StopMoveLeftFromUI()
    {
        _uiMoveLeftHeld = false;
    }

    public void StartMoveRightFromUI()
    {
        _uiMoveRightHeld = true;
    }

    public void StopMoveRightFromUI()
    {
        _uiMoveRightHeld = false;
    }

    public void JumpFromUI()
    {
        if (!_isInitialized)
        {
            return;
        }

        _controllable.Jump();
    }

    public void StartKickFromUI()
    {
        if (!_isInitialized)
        {
            return;
        }

        _controllable.StartKick();
    }

    public void StopKickFromUI()
    {
        if (!_isInitialized)
        {
            return;
        }

        _controllable.ReleaseKick();
    }

    private float GetUIMoveInput()
    {
        float input = 0f;

        if (_uiMoveLeftHeld)
        {
            input -= 1f;
        }

        if (_uiMoveRightHeld)
        {
            input += 1f;
        }

        return Mathf.Clamp(input, -1f, 1f);
    }
}
