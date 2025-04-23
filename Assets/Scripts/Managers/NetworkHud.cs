using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System.Net;
using System.Linq;

public class NetworkHud : NetworkBehaviour
{
    internal static Player localPlr;
    internal static NetworkHud nh;
    [SerializeField]
    private GameObject gameManager;
    private static string localIPAddress;
    private static string goalIPAddress = "127.0.0.1";


    /// <summary>
    /// Update to draw the UI at the top of the screen
    /// </summary>
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 200, 200));
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
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
        localIPAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
        Instantiate(gameManager);
    }


    /// <summary>
    /// The buttons to select what type of instance you are running
    /// </summary>
    static void StartButtons()
    {
        goalIPAddress = GUILayout.TextField(goalIPAddress); 
        if (GUILayout.Button("Host"))
        {
            //NetworkManager.Singleton.StartHost();
            GameNetworkManager.GMN.StartHost(6);
            GameManager.gm.enemyManager.spawnEnemies();
        }
        if (GUILayout.Button("Client"))
        {
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(goalIPAddress, (ushort)7777);
            NetworkManager.Singleton.StartClient();
        }
        if (GUILayout.Button("Server"))
        {
            NetworkManager.Singleton.StartServer();
            GameManager.gm.enemyManager.spawnEnemies();
        }
        
    }


    /// <summary>
    /// Tool to get the type of instance you are running.
    /// </summary>
    /// <returns></returns>
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
        if (localPlr != null) s = "[Client " + localPlr.playerID + "] " + msg;
        if (!NetworkManager.Singleton.IsServer) printServerRPC(s, warning);
    }
    //The server part of it.
    [ServerRpc(RequireOwnership = false)]
    private void printServerRPC(string msg, bool warning)
    {
        if (warning) Debug.LogWarning(msg);
        else Debug.Log(msg + ". Player ID: " + GameObject.FindObjectOfType<Player>().playerID);
    }


    /// <summary>
    /// The status labels at the top of the screen
    /// </summary>
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
        GUILayout.Label("Local IP: " + localIPAddress);
        if (NetworkManager.Singleton.IsServer)
        {
            GUILayout.Label("# Connected: " + NetworkManager.Singleton.ConnectedClients.Count);
            if (GUILayout.Button("Start Turn Timer")) GameManager.gm.initiatePlayersTurn();
        }

        if (NetworkManager.Singleton.IsConnectedClient)
        {
            attemptLocalPlayer();
            GUILayout.Label("Player ID: " + localPlr.playerID);
            GUILayout.Label("Health: " + localPlr.health.Value);
            GUILayout.Label("Player Section: "+Terrain.terrain.getMapSection(localPlr.image.transform.position));
            //GUILayout.Label("Mouse Section: " + Terrain.terrain.getMapSection(Input.mousePosition));
        }
    }


    /// <summary>
    /// checks to see if the local player is null, and tries to fill out the local player if they are null
    /// </summary>
    static void attemptLocalPlayer()
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
