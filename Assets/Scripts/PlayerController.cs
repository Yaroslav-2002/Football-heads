using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInputController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D _rigidbody;
    private PlayerInputController _inputController;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _inputController = GetComponent<PlayerInputController>();
    }

    private void FixedUpdate()
    {
        Vector2 velocity = _rigidbody.linearVelocity;
        velocity.x = _inputController.MoveInput * moveSpeed;
        _rigidbody.linearVelocity = velocity;

        if (IsGrounded() && _inputController.JumpRequested)
        {
            _inputController.ConsumeJumpRequest();
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

        int layerMaskValue = groundLayer.value;

        if (layerMaskValue == 0)
        {
            return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius) != null;
        }

        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, layerMaskValue) != null;
    }
}
