using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

[RequireComponent(typeof(NetworkManager))]
public class NetworkBootstrapper : MonoBehaviour
{
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private GameConfiguration configuration;

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

        if (configuration == null)
        {
            Debug.LogError("NetworkBootstrapper requires a GameConfiguration asset.", this);
            return;
        }

        if (configuration.CurrentMode != GameMode.Multiplayer)
        {
            return;
        }

        var transport = networkManager.NetworkConfig?.NetworkTransport as UnityTransport;

        if (configuration.ShouldStartHost)
        {
            ApplyHostTransportSettings(transport);
            if (!networkManager.StartHost())
            {
                Debug.LogError("Failed to start host session.", this);
            }
            else if (!string.IsNullOrEmpty(configuration.HostJoinCode))
            {
                if (JoinCodeUtility.TryParseJoinCode(configuration.HostJoinCode, out var shareAddress, out var sharePort))
                {
                    Debug.Log($"Hosting session at {shareAddress}:{sharePort}. Join code: {configuration.HostJoinCode}");
                }
                else
                {
                    Debug.Log($"Hosting session. Join code: {configuration.HostJoinCode}");
                }
            }

            configuration.ResetNetworkConfiguration();
        }
        else if (configuration.ShouldStartClient)
        {
            if (transport == null)
            {
                Debug.LogError("Unable to configure client transport.", this);
                configuration.ResetNetworkConfiguration();
                return;
            }

            if (!JoinCodeUtility.TryParseJoinCode(configuration.ClientJoinCode, out var address, out var port))
            {
                Debug.LogError("Provided join code is invalid.", this);
                configuration.ResetNetworkConfiguration();
                return;
            }

            transport.SetConnectionData(address, port);
            if (!networkManager.StartClient())
            {
                Debug.LogError("Failed to start client using provided join code.", this);
            }

            configuration.ResetNetworkConfiguration();
        }
    }

    private void ApplyHostTransportSettings(UnityTransport transport)
    {
        if (transport == null)
        {
            return;
        }

        transport.SetConnectionData(configuration.HostAddress, configuration.HostPort, configuration.HostAddress);
    }
}

