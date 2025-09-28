using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class PlayerInput : MonoBehaviour
{
    private IControllable _controllable;
    private InputActions _inputActions;
    private InputActions.PlayerActions _playerActions;

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
        _playerActions = _inputActions.Player;
    }

    private void OnEnable()
    {
        if (_inputActions == null)
        {
            return;
        }

        _playerActions.Enable();
        _playerActions.Jump.performed += OnJumpPerformed;
        _playerActions.Kick.started += OnKickStarted;
        _playerActions.Kick.canceled += OnKickCanceled;
    }

    private void OnDisable()
    {
        if (_inputActions == null)
        {
            return;
        }

        _playerActions.Jump.performed -= OnJumpPerformed;
        _playerActions.Kick.started -= OnKickStarted;
        _playerActions.Kick.canceled -= OnKickCanceled;
        _playerActions.Disable();
    }

    private void OnDestroy()
    {
        _inputActions?.Dispose();
    }

    private void Update()
    {
        if (_controllable == null || _inputActions == null)
        {
            return;
        }

        Vector2 moveInput = _playerActions.Move.ReadValue<Vector2>();
        _controllable.Move(moveInput.x);
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
}
