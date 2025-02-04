using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkHud : NetworkBehaviour
{
    internal static Player localPlr;
    internal static NetworkHud nh;


    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 200, 200));
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            StartButtons();
        } else
        {
            StatusLabels();
        }
        GUILayout.EndArea();
    }

    private void Awake()
    {
        nh = this;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }


    static void StartButtons()
    {
        if (GUILayout.Button("Host"))
        {
            NetworkManager.Singleton.StartHost();
        }
        if (GUILayout.Button("Client"))
        {
            NetworkManager.Singleton.StartClient();
        }
        if (GUILayout.Button("Server"))
        {
            NetworkManager.Singleton.StartServer();
        }
    }

    internal static string getType()
    {
        if (NetworkManager.Singleton.IsHost) return "[Host] ";
        if (NetworkManager.Singleton.IsServer) return "[Server] ";
        if (NetworkManager.Singleton.IsConnectedClient) return "[Client - Connected] ";
        if (NetworkManager.Singleton.IsClient) return "[Client - Disconnected] ";
        return "";
    }

    
    /// <summary>
    /// Prints a given message to both the client and the server.
    /// </summary>
    /// <param name="msg">The message to be printed</param>
    /// <param name="warning"> Is it logged as a warning? </param>
    internal void print(string msg, bool warning = false)
    {
        if (!GameManager.gm.debugMessages) return;
        string s = getType() + msg;
        if (warning) Debug.LogWarning(s);
        else Debug.Log(s);
        attemptLocalPlayer();
        if (localPlr != null) s = "[Client " + localPlr.playerID.Value + "] " + msg;
        if (!NetworkManager.Singleton.IsServer) printServerRPC(s, warning);
    }
    [ServerRpc(RequireOwnership = false)]
    private void printServerRPC(string msg, bool warning)
    {
        if (warning) Debug.LogWarning(msg);
        else Debug.Log(msg);
    }


    static void StatusLabels()
    {
        string mode;
        if (NetworkManager.Singleton.IsHost)
            mode = "Host";
        else if (NetworkManager.Singleton.IsServer)
            mode = "Server";
        else if (NetworkManager.Singleton.IsConnectedClient)
            mode = "Client - Connected";
        else
            mode = "Unknown";
        GUILayout.Label("Transport: " + NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
        GUILayout.Label("Mode: " + mode);
        if (NetworkManager.Singleton.IsServer)
            GUILayout.Label("# Connected: " + NetworkManager.Singleton.ConnectedClients.Count);

        if (NetworkManager.Singleton.IsConnectedClient)
        {

            if (localPlr == null)
            {
                NetworkObject playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
                if (playerObject != null)
                {
                    if (localPlr == null) localPlr = playerObject.GetComponent<Player>();
                }
            }
            GUILayout.Label("Health: " + localPlr.health.Value);
        }
    }


    void attemptLocalPlayer()
    {
        if (NetworkManager.Singleton.IsConnectedClient)
        {
            if (localPlr == null)
            {
                NetworkObject playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
                if (playerObject != null)
                {
                    if (localPlr == null) localPlr = playerObject.GetComponent<Player>();
                }
            }
        }
    }
}
