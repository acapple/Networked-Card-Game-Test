using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField]
    List<string> cardsInDeck;
    [SerializeReference]
    List<Card> cardsInDeckProper;
    List<int> deckOrder;


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
            cardsInDeckProper[deckOrder[i]].OnPlay();
            deckDebug += cardsInDeckProper[deckOrder[i]].name + ", ";
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

    internal void drawCard()
    {

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
