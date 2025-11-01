using Unity.Netcode;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInput))]
public class NetworkPlayer : NetworkBehaviour, IControllable
{
    [SerializeField] private BootSwing boot;
    [SerializeField] private Movement movement;
    [SerializeField] private PlayerInput input;

    private string _id;
    public string Id => _id;
    
    private TeamSide _team;
    public TeamSide Team => _team;

    private void Start()
    {
        if (input == null)
        {
            return;
        }

        if (IsOwner)
        {
            if (!input.enabled)
            {
                input.enabled = true;
            }
        }
        else
        {
            input.enabled = false;
        }
    }

    public void Init(PlayerSpawnSettings playerSpawnSettings)
    {
        _id = playerSpawnSettings.Identifier;
        _team = playerSpawnSettings.Team;
        input.Configure(playerSpawnSettings.ControlScheme);
    }

    public void ReleaseKick() => boot.EndKickPress();
    public void StartKick() => boot.BeginKickPress();
    public void Move(float direction) => movement.Move(direction);
    public void Jump() => movement.Jump();
}
