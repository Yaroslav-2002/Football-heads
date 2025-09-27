using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class PlayerInputController : MonoBehaviour
{
    public float MoveInput { get; private set; }
    public bool JumpRequested => _jumpRequested;

    private bool _jumpRequested;

    private void Update()
    {
        MoveInput = ReadHorizontalInput();

        if (IsJumpTriggered())
        {
            _jumpRequested = true;
        }
    }

    public void ConsumeJumpRequest()
    {
        _jumpRequested = false;
    }

    private float ReadHorizontalInput()
    {
        float keyboardInput = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed)
            {
                keyboardInput -= 1f;
            }

            if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed)
            {
                keyboardInput += 1f;
            }
        }

        float gamepadInput = 0f;

        if (Gamepad.current != null)
        {
            gamepadInput = Gamepad.current.leftStick.ReadValue().x;
        }

        float input = Mathf.Abs(gamepadInput) > Mathf.Abs(keyboardInput) ? gamepadInput : keyboardInput;

        return Mathf.Clamp(input, -1f, 1f);
    }

    private bool IsJumpTriggered()
    {
        bool jump = false;

        if (Keyboard.current != null)
        {
            jump |= Keyboard.current.spaceKey.wasPressedThisFrame ||
                    Keyboard.current.wKey.wasPressedThisFrame ||
                    Keyboard.current.upArrowKey.wasPressedThisFrame;
        }

        if (Gamepad.current != null)
        {
            jump |= Gamepad.current.buttonSouth.wasPressedThisFrame;
        }

        return jump;
    }
}
