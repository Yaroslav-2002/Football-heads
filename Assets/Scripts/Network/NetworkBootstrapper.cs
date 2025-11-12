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

        var transport = networkManager.NetworkConfig?.NetworkTransport as UnityTransport;

        if (GameConfiguration.ShouldStartHost)
        {
            ApplyHostTransportSettings(transport);
            if (!networkManager.StartHost())
            {
                Debug.LogError("Failed to start host session.", this);
            }
            else if (!string.IsNullOrEmpty(GameConfiguration.HostJoinCode))
            {
                if (JoinCodeUtility.TryParseJoinCode(GameConfiguration.HostJoinCode, out var shareAddress, out var sharePort))
                {
                    Debug.Log($"Hosting session at {shareAddress}:{sharePort}. Join code: {GameConfiguration.HostJoinCode}");
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

    private void ApplyHostTransportSettings(UnityTransport transport)
    {
        if (transport == null)
        {
            return;
        }

        transport.SetConnectionData(GameConfiguration.HostAddress, GameConfiguration.HostPort, GameConfiguration.HostAddress);
    }
}

