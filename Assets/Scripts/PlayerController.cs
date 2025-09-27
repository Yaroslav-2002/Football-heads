using System.Collections;
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
    [Header("Kick Settings")]
    [SerializeField] private Transform bootTransform;
    [SerializeField] private float kickForwardAngle = -75f;
    [SerializeField] private float kickOutDuration = 0.08f;
    [SerializeField] private float kickReturnDuration = 0.15f;
    [SerializeField] private float kickCooldown = 0.3f;

    private Rigidbody2D _rigidbody;
    private float _moveInput;
    private bool _jumpRequested;
    private bool _kickRequested;
    private bool _isKicking;
    private float _nextKickAllowedTime;
    private Quaternion _bootDefaultLocalRotation;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();

        if (bootTransform != null)
        {
            _bootDefaultLocalRotation = bootTransform.localRotation;
        }
    }

    private void OnValidate()
    {
        if (bootTransform != null)
        {
            _bootDefaultLocalRotation = bootTransform.localRotation;
        }
    }

    private void Update()
    {
        _moveInput = ReadHorizontalInput();

        if (IsJumpTriggered() && IsGrounded())
        {
            _jumpRequested = true;
        }

        if (IsKickTriggered())
        {
            QueueKick();
        }

        if (_kickRequested && !_isKicking)
        {
            _kickRequested = false;
            StartCoroutine(KickRoutine());
        }
    }

    private void FixedUpdate()
    {
        Vector2 velocity = _rigidbody.linearVelocity;
        velocity.x = _moveInput * moveSpeed;
        _rigidbody.linearVelocity = velocity;

        if (_jumpRequested)
        {
            _jumpRequested = false;
            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, 0f);
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

    private bool IsKickTriggered()
    {
        bool kick = false;

        if (Keyboard.current != null)
        {
            kick |= Keyboard.current.kKey.wasPressedThisFrame ||
                    Keyboard.current.rightCtrlKey.wasPressedThisFrame ||
                    Keyboard.current.rightAltKey.wasPressedThisFrame;
        }

        if (Gamepad.current != null)
        {
            kick |= Gamepad.current.buttonEast.wasPressedThisFrame ||
                    Gamepad.current.rightTrigger.wasPressedThisFrame;
        }

        return kick;
    }

    private void QueueKick()
    {
        if (bootTransform == null)
        {
            return;
        }

        if (_isKicking || Time.time < _nextKickAllowedTime)
        {
            return;
        }

        _kickRequested = true;
    }

    private IEnumerator KickRoutine()
    {
        _isKicking = true;

        Quaternion startRotation = _bootDefaultLocalRotation;
        Quaternion forwardRotation = startRotation * Quaternion.Euler(0f, 0f, kickForwardAngle);

        float elapsed = 0f;

        while (elapsed < kickOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = kickOutDuration > 0f ? Mathf.Clamp01(elapsed / kickOutDuration) : 1f;
            bootTransform.localRotation = Quaternion.Lerp(startRotation, forwardRotation, t);
            yield return null;
        }

        elapsed = 0f;

        while (elapsed < kickReturnDuration)
        {
            elapsed += Time.deltaTime;
            float t = kickReturnDuration > 0f ? Mathf.Clamp01(elapsed / kickReturnDuration) : 1f;
            bootTransform.localRotation = Quaternion.Lerp(forwardRotation, startRotation, t);
            yield return null;
        }

        bootTransform.localRotation = startRotation;
        _nextKickAllowedTime = Time.time + kickCooldown;
        _isKicking = false;
    }
}
