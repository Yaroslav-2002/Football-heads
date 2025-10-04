using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour, IControllable
{
    [SerializeField] private BootSwing boot;
    [SerializeField] private Movement movement;
    
    public void ReleaseKick() => boot.EndKickPress();
    public void StartKick() => boot.BeginKickPress();
    public void Move(float direction) => movement.Move(direction);
    public void Jump() => movement.Jump();
}
