using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkHud : NetworkBehaviour
{
    internal static Target localPlr;


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


    private void Start()
    {
        
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
                Debug.Log("Getting players");
                NetworkObject playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
                if (playerObject != null)
                {
                    if (localPlr == null) localPlr = playerObject.GetComponent<Target>();
                }
            }
            GUILayout.Label("Health: " + localPlr.health.Value);
        }
    }
}
