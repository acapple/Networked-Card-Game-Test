using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class Player : Target
{
    #region variables
    //Static variables
    internal static Dictionary<int, Player> playersInGame;

    [Header("Networking & UI variables")]
    [SerializeField]
    internal int playerID;
    [SerializeField]
    float cardSpacing = 95;
    
    [Header("Gameplay variables")]
    [SerializeField] [Tooltip("(Number of actions remaining this turn, number of actions next turn)")]
    internal Vector2 actions;
    [SerializeField] [Tooltip("Hand size at the start of the game -> to move to game manager")]
    int StartingHandSize = 5; // TODO: Export this variable to gamemanager
    [SerializeField] [Tooltip("What cards are currently in the player's hand")]
    internal List<Card> hand;

    [Header("References")]
    [SerializeField]
    Canvas canvas;
    /*[SerializeField]
    GameObject cardHandSample;*/

    #endregion variables

    #region setup
    /// <summary>
    /// Adds this player to the network hud's local player
    /// </summary>
    public override void OnNetworkSpawn()
    {
        if (NetworkHud.localPlr == null && NetworkManager.IsConnectedClient) {

            NetworkHud.localPlr = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().GetComponent<Player>();
        }
        base.OnNetworkSpawn();
        StartCoroutine(gameSetup());
    }


    /// <summary>
    /// Any setup needed to be done to actually play the game.
    /// </summary>
    /// <returns></returns>
    private IEnumerator gameSetup()
    {
        yield return new WaitForSeconds(0.5f);
        playerRequestMoveServerRPC(0, true);
        drawHandOfCards();
        actions = new Vector2Int(0, GameManager.gm.numberOfActions);
    }

    

    /// <summary>
    /// Initializes and adds this player to the server side dictionary of players in the game
    /// </summary>
    void Start()
    {
        if (IsServer)
        {
            if (playersInGame == null) playersInGame = new Dictionary<int, Player>();
            do
            {
                playerID = Random.Range(0, System.Int32.MaxValue);
            } while (playersInGame.ContainsKey(playerID));
            updatePlayerIDClientRPC(playerID);
            playersInGame.Add(playerID, this);
            image.color = new Color(Random.value, Random.value, Random.value);
            new TargetLocator(this);
        }
    }


    /// <summary>
    /// Removes this player from the dictionary of local players
    /// </summary>
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (IsServer)
        {
            playersInGame.Remove(playerID);
        }
    }

    [Rpc(SendTo.NotServer)]
    internal void updatePlayerIDClientRPC(int id)
    {
        if (!IsLocalPlayer || !IsOwner) return;
        playerID = id;
    }
    #endregion setup


    #region movement
    /// <summary>
    /// Initial call of player being dragged to get the player to move sections
    /// </summary>
    public void movePlayer()
    {
        if (!IsLocalPlayer) return;
        int moveto = Terrain.terrain.getMapSection(image.transform.position);
        if (moveto == -1)
        {
            NetworkHud.nh.print("Tried moving off the section");
            return;
        }
        NetworkHud.nh.print("Requesting move to section " + moveto);
        playerRequestMoveServerRPC(moveto);
    }


    /// <summary>
    /// Player requests to move. 
    /// 
    /// TODO: Sync positioning in a new function
    /// </summary>
    /// <param name="section">Where to move to</param>
    /// <param name="moveOverride">ignore not being player's turn</param>
    [Rpc(SendTo.Server)]
    public void playerRequestMoveServerRPC(int section, bool moveOverride = false)
    {
        if (GameManager.gm.State == GameManager.gameState.PlayersTurn || moveOverride)
        {
            NetworkHud.nh.print("Moving to section " + section);
            movePlayerClientRPC(section);
            Terrain.terrain.moveImage(image, section);
            TargetLocator.move(this, section);
        } else
        {
            NetworkHud.nh.print("Invalid move action. Is not player turn.");
        }
    }

    
    /// <summary>
    /// Server tells client where to move the player to
    /// </summary>
    /// <param name="section">where to go</param>
    [Rpc(SendTo.NotServer)]
    public void movePlayerClientRPC(int section)
    {
        Terrain.terrain.moveImage(image, section);
    }

    #endregion movement

    #region card hand management
    /// <summary>
    /// [Server Only] Draws a hand of cards equal to the starting hand size
    /// </summary>
    void drawHandOfCards()
    {
        if (!IsServer) return;
        NetworkHud.nh.print("Creating a hand of cards");
        deck.ShuffleDeck();
        for (int i = 0; i < StartingHandSize; i++)
        {
            AddCardToHand(deck.drawCard());
        }
    }


    /// <summary>
    /// [Server Only] Adds a card to the player's hand
    /// </summary>
    /// <param name="c"> the card added to the players hand</param>
    internal void AddCardToHand(Card c)
    {
        if (!IsServer) return;
        if (GameManager.gm.repetitiveMessages) NetworkHud.nh.print("Adding a card to the hand");

        if (c == null)
        {
            NetworkHud.nh.print("A card to be added to the hand doesn't exist", true);
            return;
        }

        hand.Add(c);
        hand[hand.Count-1].GetComponent<Image>().enabled = true;
        c.transform.GetChild(0).gameObject.SetActive(true);
        hand[hand.Count - 1].GetComponent<Image>().color = new Color(Random.Range(0.0f, 1), Random.Range(0.0f, 1), Random.Range(0.0f, 1)); // Temporary
        for (int i=0; i<hand.Count; i++)
        {
            int x = (int) Mathf.Floor(canvas.pixelRect.width * 0.5f + cardSpacing * i - 0.5f * cardSpacing * (hand.Count - 1));
            hand[i].transform.position = new Vector3(x, 50, 0);
        }
        addCardToHandClientRPC(c.keyInDeck);
        return;
    }


    /// <summary>
    /// Adds a card to the clientside hand
    /// </summary>
    /// <param name="cardKey"> the dictionary key of the card to be added to the hand </param>
    [Rpc(SendTo.NotServer)]
    internal void addCardToHandClientRPC(int cardKey)
    {
        if (!IsLocalPlayer || !IsOwner) return;
        Card c = deck.cardsFromThisDeck[cardKey];
        hand.Add(c);
        hand[hand.Count - 1].GetComponent<Image>().enabled = true;
        c.transform.GetChild(0).gameObject.SetActive(true);
        hand[hand.Count - 1].GetComponent<Image>().color = new Color(Random.Range(0.0f, 1), Random.Range(0.0f, 1), Random.Range(0.0f, 1)); // Temporary
        for (int i = 0; i < hand.Count; i++)
        {
            int x = (int)Mathf.Floor(canvas.pixelRect.width * 0.5f + cardSpacing * i - 0.5f * cardSpacing * (hand.Count - 1));
            hand[i].transform.position = new Vector3(x, 50, 0);
        }
    }


    /// <summary>
    /// removes a card from the client's hand 
    /// </summary>
    /// <param name="result"></param>
    /// <param name="card"></param>
    [Rpc(SendTo.NotServer)]
    internal void RemoveCardFromHandClientRPC(int card)
    {
        if (!IsLocalPlayer || !IsOwner) return;

        if (GameManager.gm.repetitiveMessages) NetworkHud.nh.print("Removing a card to the hand");
        Card c = hand[card];
        hand.RemoveAt(card);
        c.GetComponent<Image>().enabled = false;
        c.transform.GetChild(0).gameObject.SetActive(false);
        deck.discard.Add(c);
        for (int i = 0; i < hand.Count; i++)
        {
            //hand[i].GetComponent<Image>().color = new Color(Random.Range(0.0f, 1), Random.Range(0.0f, 1), Random.Range(0.0f, 1)); // Temporary
            int x = (int)Mathf.Floor(canvas.pixelRect.width * 0.5f + cardSpacing * i - 0.5f * cardSpacing * (hand.Count - 1));
            hand[i].transform.position = new Vector3(x, 50, 0);
        }
    }


    /// <summary>
    /// [Server only] Starts the process of playing a card
    /// 
    /// TODO: change cardnum to the key in the deck dictionary.
    /// </summary>
    /// <param name="cardNum"> the card number, based on the positioning in the hand. </param>
    /// <param name="section"></param>
    [Rpc(SendTo.Server)]
    internal override void RequestCardPlayedServerRPC(int cardNum, int section)
    {
        //NetworkHud.nh.print("Got asked to play card numbered: " + cardNum + " out of "+hand.Count+" cards"/*" named: " + hand[cardNum].title*/);
        //Debug.Log("Player asked to play card numbered: " + cardNum + " named: " + hand[cardNum].name);
        //NetworkHud.nh.print("Player ID during the request card played: " + playerID + ", " + playersInGame.Values);
        if (hand[cardNum].ThisCardPlay(this, section))
        {
            RemoveCardFromHandClientRPC(cardNum);
            //NetworkHud.nh.print("Removing a card to the hand");
            Card c = hand[cardNum];
            hand.RemoveAt(cardNum);
            c.GetComponent<Image>().enabled = false;
            c.transform.GetChild(0).gameObject.SetActive(false);
            deck.discard.Add(c);
            for (int i = 0; i < hand.Count; i++)
            {
                //hand[i].GetComponent<Image>().color = new Color(Random.Range(0.0f, 1), Random.Range(0.0f, 1), Random.Range(0.0f, 1)); // Temporary
                int x = (int)Mathf.Floor(canvas.pixelRect.width * 0.5f + cardSpacing * i - 0.5f * cardSpacing * (hand.Count - 1));
                hand[i].transform.position = new Vector3(x, 50, 0);
            }
        }
    }
    #endregion card hand management
}
