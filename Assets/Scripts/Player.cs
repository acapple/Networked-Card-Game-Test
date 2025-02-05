using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class Player : Target
{
    [SerializeField]
    internal List<Card> hand;
    [SerializeField]
    internal NetworkVariable<int> playerID;
    [SerializeField]
    Deck deck;
    [SerializeField]
    GameObject cardHandSample;
    [SerializeField]
    Canvas canvas;
    [SerializeField]
    float cardSpacing = 95;
    [SerializeField]
    int StartingHandSize = 5;

    public override void OnNetworkSpawn()
    {
        if (NetworkHud.localPlr == null && NetworkManager.IsConnectedClient) NetworkHud.localPlr = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().GetComponent<Player>();
        if (IsServer) playerID.Value = Random.Range(0, System.Int32.MaxValue);
        base.OnNetworkSpawn();
    }

    void Start()
    {
        if (IsServer)
        {
            playerID = new NetworkVariable<int>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
            
        }
        drawHandOfCards();
    }


    /// <summary>
    /// Draws a hand of cards equal to the starting hand size
    /// </summary>
    void drawHandOfCards()
    {
        NetworkHud.nh.print("Creating a hand of cards");
        deck.ShuffleDeck();
        for (int i = 0; i < StartingHandSize; i++)
        {
            AddCardToHand(deck.drawCard());
        }
    }


    /// <summary>
    /// Adds a card to the player's hand
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
        for (int i=0; i<hand.Count; i++)
        {
            hand[i].GetComponent<Image>().color = new Color(Random.Range(0.0f, 1), Random.Range(0.0f, 1), Random.Range(0.0f, 1)); // Temporary
            int x = (int) Mathf.Floor(canvas.pixelRect.width * 0.5f + cardSpacing * i - 0.5f * cardSpacing * (hand.Count - 1));
            hand[i].transform.position = new Vector3(x, 50, 0);
        }
        addCardToHandClientRPC(c.keyInDeck);
        return;
    }
    [ClientRpc]
    internal void addCardToHandClientRPC(int cardKey)
    {
        Debug.Log("Using dictionary");
        Card c = deck.cardsFromThisDeck[cardKey];
        hand.Add(c);
        for (int i = 0; i < hand.Count; i++)
        {
            hand[i].GetComponent<Image>().color = new Color(Random.Range(0.0f, 1), Random.Range(0.0f, 1), Random.Range(0.0f, 1)); // Temporary
            int x = (int)Mathf.Floor(canvas.pixelRect.width * 0.5f + cardSpacing * i - 0.5f * cardSpacing * (hand.Count - 1));
            hand[i].transform.position = new Vector3(x, 50, 0);
        }
    }


    [ClientRpc]
    internal void RemoveCardFromHandClientRPC(string result, int card)
    {
        if (!IsLocalPlayer || !IsOwner) return;

        if (GameManager.gm.repetitiveMessages) NetworkHud.nh.print("Removing a card to the hand");
        Card c = hand[card];
        hand.RemoveAt(card);
        c.GetComponent<Image>().enabled = false;
        deck.discard.Add(c);
        for (int i = 0; i < hand.Count; i++)
        {
            //hand[i].GetComponent<Image>().color = new Color(Random.Range(0.0f, 1), Random.Range(0.0f, 1), Random.Range(0.0f, 1)); // Temporary
            int x = (int)Mathf.Floor(canvas.pixelRect.width * 0.5f + cardSpacing * i - 0.5f * cardSpacing * (hand.Count - 1));
            hand[i].transform.position = new Vector3(x, 0, 0);
        }
    }


    [ServerRpc]
    internal void RequestCardPlayedServerRPC(int cardNum)
    {

        NetworkHud.nh.print("Got asked to play card numbered: " + cardNum + " out of "+hand.Count+" cards"/*" named: " + hand[cardNum].title*/);
        //Debug.Log("Player asked to play card numbered: " + cardNum + " named: " + hand[cardNum].name);
        if (hand[cardNum].ThisCardPlay())
        {
            RemoveCardFromHandClientRPC("true", cardNum);
            //NetworkHud.nh.print("Removing a card to the hand");
            Card c = hand[cardNum];
            hand.RemoveAt(cardNum);
            c.GetComponent<Image>().enabled = false;
            deck.discard.Add(c);
            for (int i = 0; i < hand.Count; i++)
            {
                //hand[i].GetComponent<Image>().color = new Color(Random.Range(0.0f, 1), Random.Range(0.0f, 1), Random.Range(0.0f, 1)); // Temporary
                int x = (int)Mathf.Floor(canvas.pixelRect.width * 0.5f + cardSpacing * i - 0.5f * cardSpacing * (hand.Count - 1));
                hand[i].transform.position = new Vector3(x, 0, 0);
            }
        }
    }
}
