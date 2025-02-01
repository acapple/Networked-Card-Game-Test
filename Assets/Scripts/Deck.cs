using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
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
        discard = new List<Card>();

        for (int i=0; i<10; i++)
        {
            GameObject c = Instantiate(cardSample, canvas);
            cardsInDeck.Add(c.GetComponent<Card>());
        }
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
        Debug.Log(deckDebug);
    }


    /// <summary>
    /// shuffles discard pile into the deck
    /// </summary>
    internal void shuffleDiscardIntoDeck()
    {
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
        printDeckValues();

        Card temp;
        int cardSwapped;
        for (int i=0; i<cardsInDeck.Count; i++)
        {
            cardSwapped = Random.Range(0, cardsInDeck.Count);
            temp = cardsInDeck[i];
            cardsInDeck[i] = cardsInDeck[cardSwapped];
            cardsInDeck[cardSwapped] = temp;
        }
        printDeckValues();
    }


    /// <summary>
    /// Draws the top card of the deck
    /// </summary>
    /// <returns> the card being drawn </returns>
    internal Card drawCard()
    {
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
        c.gameObject.SetActive(true);
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
