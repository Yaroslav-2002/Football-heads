using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D _rigidbody;
    private float _moveInput;
    private bool _jumpRequested;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _moveInput = ReadHorizontalInput();

        if (IsJumpTriggered() && IsGrounded())
        {
            _jumpRequested = true;
        }
    }

    private void FixedUpdate()
    {
        Vector2 velocity = _rigidbody.velocity;
        velocity.x = _moveInput * moveSpeed;
        _rigidbody.velocity = velocity;

        if (_jumpRequested)
        {
            _jumpRequested = false;
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0f);
            _rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    private bool IsGrounded()
    {
        if (groundCheck == null)
        {
            return true;
        }

        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
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
