using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour, IControllable
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private GameObject boot;

    private Rigidbody2D _rigidbody;
    private BootSwing _boot;

    private void Awake()
    {
        _boot = boot.GetComponent<BootSwing>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public void Move(float direction)
    {
        Vector2 velocity = _rigidbody.linearVelocity;
        velocity.x = direction * moveSpeed;
        _rigidbody.linearVelocity = velocity;
    }

    public void Jump()
    {
        if (!IsGrounded())
        {
            return;
        }

        _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, 0f);
        _rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    public void StartKick()
    {
        _boot.BeginKickPress();
    }

    public void ReleaseKick()
    {
        _boot.EndKickPress();
    }

    private bool IsGrounded()
    {
        if (groundCheck == null)
        {
            return true;
        }

        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }
}
