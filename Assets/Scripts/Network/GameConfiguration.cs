using UnityEngine;

[CreateAssetMenu(fileName = "GameConfiguration", menuName = "Configuration/Game Configuration")]
public class GameConfiguration : ScriptableObject
{
    [SerializeField] private GameMode currentMode = GameMode.Singleplayer;
    [SerializeField] private string hostAddress = "0.0.0.0";
    [SerializeField] [Min(0)] private int hostPort = 7777;
    [SerializeField] private string hostJoinCode = string.Empty;
    [SerializeField] private string clientJoinCode = string.Empty;
    [SerializeField] private bool shouldStartHost;
    [SerializeField] private bool shouldStartClient;

    public GameMode CurrentMode => currentMode;
    public bool ShouldStartHost => shouldStartHost;
    public bool ShouldStartClient => shouldStartClient;
    public string HostAddress => hostAddress;
    public ushort HostPort => (ushort)Mathf.Clamp(hostPort, 0, ushort.MaxValue);
    public string HostJoinCode => hostJoinCode;
    public string ClientJoinCode => clientJoinCode;

    public void SetGameMode(GameMode mode)
    {
        currentMode = mode;
    }

    public void ConfigureHost(string listenAddress, ushort port, string joinCode)
    {
        currentMode = GameMode.Multiplayer;
        hostAddress = listenAddress;
        hostPort = Mathf.Clamp(port, 0, ushort.MaxValue);
        hostJoinCode = joinCode;
        shouldStartHost = true;
        shouldStartClient = false;
        clientJoinCode = string.Empty;
    }

    public void ConfigureClient(string joinCode)
    {
        currentMode = GameMode.Multiplayer;
        clientJoinCode = joinCode;
        shouldStartClient = true;
        shouldStartHost = false;
    }

    public void ResetNetworkConfiguration()
    {
        shouldStartHost = false;
        shouldStartClient = false;
        hostJoinCode = string.Empty;
        clientJoinCode = string.Empty;
    }

    private void OnValidate()
    {
        hostPort = Mathf.Clamp(hostPort, 0, ushort.MaxValue);
    }
}

