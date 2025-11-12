using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

[RequireComponent(typeof(NetworkManager))]
public class NetworkBootstrapper : MonoBehaviour
{
    [SerializeField] private NetworkManager networkManager;

    private void Awake()
    {
        if (networkManager == null)
        {
            networkManager = GetComponent<NetworkManager>();
        }
    }

    private void Start()
    {
        if (networkManager == null)
        {
            Debug.LogError("NetworkBootstrapper requires a NetworkManager instance.", this);
            return;
        }

        if (GameConfiguration.CurrentMode != GameMode.Multiplayer)
        {
            return;
        }

        if (GameConfiguration.ShouldStartHost)
        {
            var transport = networkManager.NetworkConfig?.NetworkTransport as UnityTransport;
            string advertisedAddress = null;
            ushort advertisedPort = 0;

            if (!string.IsNullOrEmpty(GameConfiguration.HostJoinCode) &&
                JoinCodeUtility.TryParseJoinCode(GameConfiguration.HostJoinCode, out var shareAddress, out var sharePort))
            {
                advertisedAddress = shareAddress;
                advertisedPort = sharePort;
            }

            ApplyHostTransportSettings(transport, advertisedAddress);
            if (!networkManager.StartHost())
            {
                Debug.LogError("Failed to start host session.", this);
            }
            else if (!string.IsNullOrEmpty(GameConfiguration.HostJoinCode))
            {
                if (!string.IsNullOrEmpty(advertisedAddress))
                {
                    Debug.Log($"Hosting session at {advertisedAddress}:{advertisedPort}. Join code: {GameConfiguration.HostJoinCode}");
                }
                else
                {
                    Debug.Log($"Hosting session. Join code: {GameConfiguration.HostJoinCode}");
                }
            }

            GameConfiguration.ResetNetworkConfiguration();
        }
        else if (GameConfiguration.ShouldStartClient)
        {
            var transport = networkManager.NetworkConfig?.NetworkTransport as UnityTransport;
            if (transport == null)
            {
                Debug.LogError("Unable to configure client transport.", this);
                GameConfiguration.ResetNetworkConfiguration();
                return;
            }

            if (!JoinCodeUtility.TryParseJoinCode(GameConfiguration.ClientJoinCode, out var address, out var port))
            {
                Debug.LogError("Provided join code is invalid.", this);
                GameConfiguration.ResetNetworkConfiguration();
                return;
            }

            transport.SetConnectionData(address, port);
            if (!networkManager.StartClient())
            {
                Debug.LogError("Failed to start client using provided join code.", this);
            }

            GameConfiguration.ResetNetworkConfiguration();
        }
    }

    private void OnDestroy()
    {
        if (networkManager == null)
        {
            return;
        }

        if (!Application.isPlaying)
        {
            return;
        }

        if (networkManager.IsListening || networkManager.IsClient || networkManager.IsServer)
        {
            networkManager.Shutdown();
        }
    }

    private void ApplyHostTransportSettings(UnityTransport transport, string advertisedAddress)
    {
        if (transport == null)
        {
            return;
        }

        string listenAddress = string.IsNullOrWhiteSpace(GameConfiguration.HostAddress)
            ? "0.0.0.0"
            : GameConfiguration.HostAddress;

        string connectionAddress = advertisedAddress;

        if (string.IsNullOrWhiteSpace(connectionAddress))
        {
            connectionAddress = listenAddress;
        }

        if (string.IsNullOrWhiteSpace(connectionAddress) || connectionAddress == "0.0.0.0")
        {
            connectionAddress = "127.0.0.1";
        }

        transport.SetConnectionData(listenAddress, GameConfiguration.HostPort, connectionAddress);
    }
}

