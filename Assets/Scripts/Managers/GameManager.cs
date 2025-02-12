using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Unity.Netcode;
using TMPro;

public class GameManager : NetworkBehaviour
{
    internal static GameManager gm;
    [SerializeField]
    internal bool debugMessages = false;
    [SerializeField]
    internal bool repetitiveMessages = false;
    [SerializeField]
    TMP_Text timer;
    public enum gameState { notInGame, PlayersTurn, EnemyTurn }

    private gameState state = gameState.notInGame;
    internal gameState State => state;
    

    // Start is called before the first frame update
    void Awake()
    {
        if (gm == null)gm = this;
    }

    public void playersTurn()
    {
        if (!NetworkManager.Singleton.IsServer) return;
        NetworkHud.nh.print("Timer called!");
        double startingTime = NetworkManager.Singleton.ServerTime.Time;
        startPlayerTurnClientRPC(startingTime);
        StartCoroutine(playerTurnTimer(startingTime));
    }

    [Rpc(SendTo.NotServer)]
    internal void startPlayerTurnClientRPC(double startingTime)
    {
        NetworkHud.nh.print("Timer called by server at " + startingTime);
        StartCoroutine(playerTurnTimer(startingTime));
    }

    public IEnumerator playerTurnTimer(double startingTime)
    {
        NetworkHud.nh.print("Local time is " + NetworkManager.Singleton.LocalTime.Time + ". Server time is " + NetworkManager.Singleton.ServerTime.Time);
        state = gameState.PlayersTurn;
        while (startingTime+10 > NetworkManager.Singleton.ServerTime.Time)
        {
            timer.text = "" + (Mathf.Round((float)(NetworkManager.Singleton.ServerTime.Time - startingTime)*100)*0.01f);
            yield return null;
        }
        timer.text = "" +10;
        yield return new WaitForSeconds(1);
        state = gameState.EnemyTurn;
        timer.text = "";
    }
}
