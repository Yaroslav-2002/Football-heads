public static class GameConfiguration
{
    public static GameMode CurrentMode { get; set; } = GameMode.Singleplayer;

    public static bool ShouldStartHost { get; private set; }

    public static bool ShouldStartClient { get; private set; }

    public static string HostAddress { get; private set; } = "0.0.0.0";

    public static ushort HostPort { get; private set; } = 7777;

    public static string HostJoinCode { get; private set; } = string.Empty;

    public static string ClientJoinCode { get; private set; } = string.Empty;

    public static void ConfigureHost(string listenAddress, ushort port, string joinCode)
    {
        CurrentMode = GameMode.Multiplayer;
        HostAddress = listenAddress;
        HostPort = port;
        HostJoinCode = joinCode;
        ShouldStartHost = true;
        ShouldStartClient = false;
        ClientJoinCode = string.Empty;
    }

    public static void ConfigureClient(string joinCode)
    {
        CurrentMode = GameMode.Multiplayer;
        ClientJoinCode = joinCode;
        ShouldStartClient = true;
        ShouldStartHost = false;
    }

    public static void ResetNetworkConfiguration()
    {
        ShouldStartHost = false;
        ShouldStartClient = false;
        HostJoinCode = string.Empty;
        ClientJoinCode = string.Empty;
    }
}

