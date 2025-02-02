using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class Deck : NetworkBehaviour
{
    //[SerializeField]
    //List<string> cardsInDeck;
    [SerializeField]
    GameObject cardSample;
    [SerializeField]
    Transform canvas;
    [SerializeReference]
    List<Card> cardsInDeck = new List<Card>();
    internal List<Card> discard;


    /// <summary>
    /// Initializes deck
    /// </summary>
    void Awake()
    {
        Debug.Log(NetworkHud.getType() + "Deck being created! ");
        discard = new List<Card>();

        for (int i=0; i<10; i++)
        {
            GameObject c = Instantiate(cardSample, canvas);
            cardsInDeck.Add(c.GetComponent<Card>());
            cardsInDeck[i].title = "C#"+i;
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
        Debug.Log(NetworkHud.getType() + deckDebug);
    }


    /// <summary>
    /// shuffles discard pile into the deck
    /// </summary>
    internal void shuffleDiscardIntoDeck()
    {
        Debug.Log(NetworkHud.getType() + "Discard is being shuffled into the deck");
        for (int i=0; i<discard.Count;)
        {
            cardsInDeck.Add(discard[i]);
            discard.RemoveAt(i);
        }
        ShuffleDeck();
    }


    /// <summary>
    /// Shuffles the deck
    /// </summary>
    internal void ShuffleDeck()
    {
        if (!IsServer) return;
        printDeckValues();
        Debug.Log(NetworkHud.getType() + "Deck is being shuffled");
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
        printDeckValues();
        shuffleDeckClientRPC(cardOrder);
    }


    [ClientRpc]
    internal void shuffleDeckClientRPC(int[] order)
    {
        Debug.Log(NetworkHud.getType() + "Server called a deck shuffle");
        printDeckValues();
        List<Card> tempDeck = new List<Card>();
        for (int i=0; i<cardsInDeck.Count; i++) tempDeck.Add(cardsInDeck[i]);
        for (int i=0; i<order.Length; i++)
        {
            cardsInDeck[i] = tempDeck[order[i]];
        }
        printDeckValues();
    }


    /// <summary>
    /// Draws the top card of the deck
    /// </summary>
    /// <returns> the card being drawn </returns>
    internal Card drawCard()
    {
        Debug.Log(NetworkHud.getType() + "Drawing a card");
        // If there are no cards to draw
        if (cardsInDeck.Count <= 0)
        {
            if (discard.Count <= 0)
            {
                Debug.LogWarning("Deck and discard empty! No cards to draw!");
                return null;
            }
            else
            {
                ShuffleDeck();
            }
        }
        Card c = cardsInDeck[0];
        c.GetComponent<Image>().enabled = true;
        TMP_Text t = c.transform.GetChild(0).GetComponent<TMP_Text>();
        Debug.Log(NetworkHud.getType() + (t== null));
        t.text = c.title;
        cardsInDeck.RemoveAt(0);
        return c;
    }


    internal void searchDeck()
    {

    }

    internal void searchDiscard()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
