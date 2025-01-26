using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    //[SerializeField]
    //List<string> cardsInDeck;
    [SerializeReference]
    List<Card> cardsInDeck;
    List<int> deckOrder;
    int posInDeck = 0;


    /// <summary>
    /// Initializes deck
    /// </summary>
    void Awake()
    {
        deckOrder = new List<int>();
        for (int i = 0; i < cardsInDeck.Count; i++) deckOrder.Add(i);
    }


    /// <summary>
    /// Prints the deck order as integers
    /// </summary>
    void printDeckOrder()
    {
        if (!GameManager.gm.debugMessages) return;

        string deckDebug = "Deck order before: ";
        for (int i = 0; i < deckOrder.Count; i++) deckDebug += deckOrder[i] + ", ";
        Debug.Log(deckDebug);
    }

    /// <summary>
    /// Prints the deck values
    /// </summary>
    void printDeckValues()
    {
        if (!GameManager.gm.debugMessages) return;

        string deckDebug = "Deck order before: ";
        for (int i = 0; i < deckOrder.Count; i++)
        {
            cardsInDeck[deckOrder[i]].OnPlay();
            deckDebug += cardsInDeck[deckOrder[i]].name + ", ";
        }
        Debug.Log(deckDebug);
    }


    /// <summary>
    /// Shuffles the deck
    /// </summary>
    internal void shuffle()
    {
        printDeckOrder();

        int temp, cardSwapped;
        for (int i=0; i<deckOrder.Count; i++)
        {
            temp = deckOrder[i];
            cardSwapped = Random.Range(0, deckOrder.Count);
            deckOrder[i] = deckOrder[cardSwapped];
            deckOrder[cardSwapped] = temp;
        }

        printDeckOrder();
        printDeckValues();
    }

    /// <summary>
    /// Draws the top card of the deck
    /// </summary>
    /// <returns> the card being drawn </returns>
    internal Card drawCard()
    {
        Card c = null;
        // If there are no cards to draw
        if (posInDeck >= deckOrder.Count)
        {
            shuffle();
            posInDeck = 0;
        }
        //Look for the next card
        for (int i=0; i<deckOrder.Count; i++)
        {
            if (posInDeck == deckOrder[i])
            {
                c = cardsInDeck[i];
                posInDeck++;
                break;
            }
            //Safety net incase the card isn't in the deck for some reason, just look for the next card
            if (i == deckOrder.Count-1)
            {
                i = 0;
                posInDeck++;
                if (posInDeck >= deckOrder.Count)
                {
                    shuffle();
                    posInDeck = 0;
                }
            }
        }
        if (c == null)
        {
            Debug.LogError("No cards found! card draw function broke!");
        }
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
