using System;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class PlayerInput : MonoBehaviour
{
    public float MoveInput { get; private set; }
    public Action OnJumpTriggered;

    private IControllable _controllable;
    private InputActions _inputActions;


    private bool _waitingForKickRelease;

    private void Awake()
    {
        _inputActions = new InputActions();
        _inputActions.Enable();

        _controllable = GetComponent<IControllable>();

        if(_controllable == null)
        {
            Debug.Log($"There's no IControllable component on the object: {gameObject.name}");
        }
    }

    private void Update()
    {
        ReadMovement();
    }

    private void ReadMovement()
    {
        float direction = _inputActions.Player.Move.ReadValue<Vector2>().x;

        _controllable.Move(direction);
    }

    private void OnEnable()
    {
        _inputActions.Player.Jump.performed += OnJumpActionTriggered;
        _inputActions.Player.Kick.started += OnKickStarted;
    }

    private void OnKickStarted(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    private void OnDisable()
    {
        _inputActions.Player.Jump.performed -= OnJumpActionTriggered;
    }

    private void OnJumpActionTriggered(InputAction.CallbackContext context)
    {
        _controllable.Jump();
    }

}
