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
    //Static & enums
    internal static GameManager gm;
    public enum gameState { notInGame, PlayersTurn, EnemyTurn }

    //Variables
    [Header("Developper tools")]
    [SerializeField] [Tooltip("Show messages in the inspector")]
    internal bool debugMessages = false;
    [SerializeField] [Tooltip("For the more repetative messages, like specific card actions")]
    internal bool repetitiveMessages = false;
    [SerializeField] [Tooltip("Current game state.")]
    private gameState state = gameState.notInGame;

    [Header("Gameplay variables")]
    [SerializeField] [Tooltip("How long the players can play cards for")]
    int playerTurnTime = 10;
    [SerializeField] [Tooltip("How many actions a player gets on their turn by default")]
    internal int numberOfActions = 1;
    [SerializeField]
    internal int endOfTurnExtraDraw = 1;

    [Header("References")]
    [SerializeField]
    TMP_Text timer;
    [SerializeField]
    internal EnemyManager enemyManager;

    //Getters & hidden variables
    private IEnumerator playerTurnCoroutine;
    internal gameState State => state;


    // Start is called before the first frame update
    void Awake()
    {
        if (gm == null) gm = this;
    }


    #region player turn

    /// <summary>
    /// [Server Only] Starts the player's turn
    /// </summary>
    public void initiatePlayersTurn()
    {
        if (!NetworkManager.Singleton.IsServer || State == gameState.PlayersTurn) return;
        NetworkHud.nh.print("Starting the players' turn.");
        double startingTime = NetworkManager.Singleton.ServerTime.Time;
        //reset player action counts to their default values.
        //Improvement for next time: Players start at 0 actions. Actions count up. How many actions "done".
        //That way, the variable can be changed at runtime, and it'll instantly update for all players
        for (int i =0; i< Player.playersInGame.Count; i++)
        {
            Player.playersInGame.ElementAt(i).Value.actions.x = Player.playersInGame.ElementAt(i).Value.actions.y;
            Player.playersInGame.ElementAt(i).Value.actions.y = numberOfActions;
        }
        //Call turn start for everyone
        startPlayerTurnClientRPC(startingTime);
        if (playerTurnCoroutine == null)
        {
            playerTurnCoroutine = playerTurnTimer(startingTime);
            StartCoroutine(playerTurnCoroutine);
        }
    }


    /// <summary>
    /// [Client Only] start the player's turn
    /// </summary>
    /// <param name="startingTime">The starting milisecond server time</param>
    [Rpc(SendTo.NotServer)]
    internal void startPlayerTurnClientRPC(double startingTime)
    {
        if (repetitiveMessages) NetworkHud.nh.print("Timer called by server at " + startingTime);
        if (playerTurnCoroutine == null)
        {
            playerTurnCoroutine = playerTurnTimer(startingTime);
            StartCoroutine(playerTurnCoroutine);
        }
    }


    /// <summary>
    /// Timer for how long the player's turn lasts. ran on everyone
    /// </summary>
    /// <param name="startingTime">The starting milisecond server time</param>
    public IEnumerator playerTurnTimer(double startingTime)
    {
        if (repetitiveMessages) NetworkHud.nh.print("Local time is " + NetworkManager.Singleton.LocalTime.Time + ". Server time is " + NetworkManager.Singleton.ServerTime.Time);
        state = gameState.PlayersTurn;
        while (startingTime + playerTurnTime > NetworkManager.Singleton.ServerTime.Time)
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
    /// TODO: for every action remaining, plus a variable int, every player draws a card
    /// </summary>
    internal void playerTurnOver()
    {
        timer.text = "";
        playerTurnCoroutine = null;
        TargetLocator[] players = TargetLocator.getPlayers();
        for (int i = 0; i < players.Length; i++)
        {
            Player p = (Player)players[i].who;
            for (int j=0; j<p.endTurnExtraCardDraw+endOfTurnExtraDraw; j++)
                p.AddCardToHand(p.deck.drawCard());
            p.endTurnExtraCardDraw = 0;
        }
        startEnemyTurn();
    }

    #endregion player turn


    #region enemy Turn
    internal void startEnemyTurn()
    {
        state = gameState.EnemyTurn;
        StartCoroutine(enemyTurn());
    }

    internal IEnumerator enemyTurn()
    {
        List<Enemy> enemies = enemyManager.enemyList;
        for (int i = 0; i < enemies.Count; i++)
        {
            for (int j = 0; j < enemies[i].actions[0]; j++)
            {
                yield return new WaitForSeconds(0.5f);
                //Debug.Log("enemy "+i+ ": "+enemies[i].name);
                if (enemies[i] != null) enemies[i].RequestCardPlayedServerRPC(1, Terrain.terrain.getMapSection(enemies[i].image.transform.position) - 1);
                
            }
            enemies[i].actions[0] = enemies[i].actions[1];
            enemies[i].actions[1] = 0;
            yield return new WaitForSeconds(0.5f);
        }
        endEnemyTurn();
    }

    internal void endEnemyTurn()
    {

    }
    #endregion enemy turn
}
