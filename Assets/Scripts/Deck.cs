using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class Deck : NetworkBehaviour
{
    [SerializeField]
    GameObject cardSample;
    [SerializeField]
    Transform canvas;
    [SerializeReference]
    List<Card> cardsInDeck = new List<Card>();
    internal List<Card> discard;
    internal Dictionary<int, Card> cardsFromThisDeck;
    [SerializeField]
    private DeckScriptableObject startingDeck;
    


    /// <summary>
    /// Initializes deck
    /// </summary>
    void Awake()
    {
        //Initialize lists
        //NetworkHud.nh.print("Deck being created! ");
        discard = new List<Card>();
        if (cardsFromThisDeck == null) cardsFromThisDeck = new Dictionary<int, Card>();

        //Create all cards & populate deck
        //  TODO: Separate card from card scriptable object. Card should only exist when a card needs a
        //  physical representation (inhand), not all the time (as is the case rn)
        for (int i=0; i< startingDeck.startingCards.Length; i++)
        {
            GameObject c = Instantiate(cardSample, canvas);
            cardsInDeck.Add(c.GetComponent<Card>());
            cardsInDeck[i].title = "C#"+i;
            cardsInDeck[i].cardReference = startingDeck.startingCards[i];
            if (GameManager.gm.repetitiveMessages) NetworkHud.nh.print("Creating Cards");
            if (!cardsFromThisDeck.ContainsKey(i))
            {
                cardsFromThisDeck.Add(i, cardsInDeck[i]);
                cardsInDeck[i].keyInDeck = i;
                cardsInDeck[i].startingDeck = this;
            }
        }
        printDeckValues();
    }


    /// <summary>
    /// Prints the deck values
    /// </summary>
    void printDeckValues()
    {
        if (!GameManager.gm.debugMessages) return;

        string deckDebug = "Deck order: ";
        for (int i = 0; i < cardsInDeck.Count; i++)
        {
            //cardsInDeck[i].ThisCardPlay();
            deckDebug += cardsInDeck[i].title + ", ";
        }
        NetworkHud.nh.print(deckDebug);
    }


    /// <summary>
    /// [Server Only] shuffles discard pile into the deck
    /// </summary>
    internal void shuffleDiscardIntoDeck()
    {
        if (!IsServer) return;
        NetworkHud.nh.print("Discard is being shuffled into the deck");
        for (int i=0; i<discard.Count;)
        {
            cardsInDeck.Add(discard[i]);
            discard.RemoveAt(i);
        }
        ShuffleDeck();
    }


    /// <summary>
    /// [Server Only]
    /// The server shuffles a deck and then calls the client to shuffle the deck.
    /// 
    /// Shuffles the list of cards and an array of integers at the same time. 
    /// List of integers is passed to the client to sync their deck so both decks are shuffled in the same way.
    /// 
    /// </summary>
    internal void ShuffleDeck()
    {
        if (!IsServer) return;
        //printDeckValues();
        NetworkHud.nh.print("Deck is being shuffled");
        Card temp;
        int cardSwapped;
        int[] cardOrder = new int[cardsInDeck.Count];
        int temp2;
        for (int i = 0; i < cardOrder.Length; i++) cardOrder[i] = i;
        for (int i=0; i<cardsInDeck.Count; i++)
        {
            cardSwapped = Random.Range(0, cardsInDeck.Count);
            temp = cardsInDeck[i];
            cardsInDeck[i] = cardsInDeck[cardSwapped];
            cardsInDeck[cardSwapped] = temp;
            temp2 = cardOrder[i];
            cardOrder[i] = cardOrder[cardSwapped];
            cardOrder[cardSwapped] = temp2;
        }
        if (GameManager.gm.repetitiveMessages) printDeckValues();
        shuffleDeckClientRPC(cardOrder);
    }


    /// <summary>
    /// The server tells the client the order of the cards (Used for shuffling the deck)
    /// </summary>
    /// <param name="order"> The array of integers the server passes to the client for the order to shuffle the deck </param>
    [Rpc(SendTo.NotServer)]
    internal void shuffleDeckClientRPC(int[] order)
    {
        NetworkHud.nh.print("Server called a deck shuffle");
        //printDeckValues();
        List<Card> tempDeck = new List<Card>();
        for (int i=0; i<cardsInDeck.Count; i++) tempDeck.Add(cardsInDeck[i]);
        for (int i=0; i<order.Length; i++)
        {
            cardsInDeck[i] = tempDeck[order[i]];
        }
        if (GameManager.gm.repetitiveMessages) printDeckValues();
    }


    /// <summary>
    /// [Server Only]
    /// Draws the top card of the deck
    /// </summary>
    /// <returns> the card being drawn </returns>
    internal Card drawCard(int cardLocationInDeck = 0)
    {
        if (!IsServer) return null;
        if (GameManager.gm.repetitiveMessages) NetworkHud.nh.print("Drawing a card");
        // If there are no cards to draw
        if (cardsInDeck.Count <= 0)
        {
            if (discard.Count <= 0)
            {
                NetworkHud.nh.print("Deck and discard empty! No cards to draw!", true);
                return null;
            }
            else
            {
                shuffleDiscardIntoDeck();
            }
        }
        Card c = cardsInDeck[cardLocationInDeck];
        c.GetComponent<Image>().enabled = true;
        TMP_Text t = c.transform.GetChild(0).GetComponent<TMP_Text>();
        t.text = c.title;
        cardsInDeck.RemoveAt(cardLocationInDeck);
        removeCardFromDeckClientRpc(cardLocationInDeck);
        return c;
    }

    /// <summary>
    /// removes a card from the client deck
    /// 
    /// Note: May have a race condition with shuffling. TBD
    /// </summary>
    /// <param name="cardLocationInDeck"></param>
    [Rpc(SendTo.NotServer)]
    internal void removeCardFromDeckClientRpc(int cardLocationInDeck)
    {
        if (GameManager.gm.repetitiveMessages) NetworkHud.nh.print("Drawing a card");
        Card c = cardsInDeck[cardLocationInDeck];
        c.GetComponent<Image>().enabled = true;
        TMP_Text t = c.transform.GetChild(0).GetComponent<TMP_Text>();
        t.text = c.title;
        cardsInDeck.RemoveAt(cardLocationInDeck);
    }


    internal void searchDeck()
    {

    }

    internal void searchDiscard()
    {

    }
}
