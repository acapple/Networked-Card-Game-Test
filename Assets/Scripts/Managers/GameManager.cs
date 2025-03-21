using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Unity.Netcode;
using TMPro;
using System.Linq;

/// <summary>
/// Manager running the game
/// </summary>
public class GameManager : NetworkBehaviour
{
    internal static GameManager gm;
    [SerializeField]
    internal bool debugMessages = false;
    [SerializeField]
    internal bool repetitiveMessages = false;
    [SerializeField]
    TMP_Text timer;
    [SerializeField]
    int playerTurnTime = 10;
    [SerializeField]
    int numberOfActions = 1;
    public enum gameState { notInGame, PlayersTurn, EnemyTurn }

    private gameState state = gameState.notInGame;
    internal gameState State => state;
    

    // Start is called before the first frame update
    void Awake()
    {
        if (gm == null)gm = this;
    }

    /// <summary>
    /// [Server Only] Starts the player's turn
    /// </summary>
    public void playersTurn()
    {
        if (!NetworkManager.Singleton.IsServer) return;
        NetworkHud.nh.print("Timer called!");
        double startingTime = NetworkManager.Singleton.ServerTime.Time;
        for (int i =0; i< Player.playersInGame.Count; i++)
        {
            Player.playersInGame.ElementAt(i).Value.actions.x = Player.playersInGame.ElementAt(i).Value.actions.y;
            Player.playersInGame.ElementAt(i).Value.actions.y = numberOfActions;
        }
        startPlayerTurnClientRPC(startingTime);
        StartCoroutine(playerTurnTimer(startingTime));
    }

    /// <summary>
    /// Server tells clients to start the player's turn
    /// </summary>
    /// <param name="startingTime">The starting milisecond server time</param>
    [Rpc(SendTo.NotServer)]
    internal void startPlayerTurnClientRPC(double startingTime)
    {
        NetworkHud.nh.print("Timer called by server at " + startingTime);
        StartCoroutine(playerTurnTimer(startingTime));
    }

    /// <summary>
    /// Run through the player's turn
    /// </summary>
    /// <param name="startingTime">The starting milisecond server time</param>
    public IEnumerator playerTurnTimer(double startingTime)
    {
        NetworkHud.nh.print("Local time is " + NetworkManager.Singleton.LocalTime.Time + ". Server time is " + NetworkManager.Singleton.ServerTime.Time);
        state = gameState.PlayersTurn;
        while (startingTime+ playerTurnTime > NetworkManager.Singleton.ServerTime.Time)
        {
            timer.text = "" + (Mathf.Round((float)(NetworkManager.Singleton.ServerTime.Time - startingTime)*100)*0.01f);
            yield return null;
        }
        timer.text = "" + playerTurnTime;
        yield return new WaitForSeconds(1);
        playerTurnOver();
    }

    /// <summary>
    /// Function to deal with the end of the player's turn
    /// 
    /// TODO: for every action remaining, plus a variable int, draw a card
    /// </summary>
    internal void playerTurnOver()
    {
        state = gameState.EnemyTurn;
        timer.text = "";
    }
}
