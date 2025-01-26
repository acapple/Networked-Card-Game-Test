using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Target
{
    [SerializeField]
    List<Card> hand;
    [SerializeField]
    Deck deck;

    // Start is called before the first frame update
    void Start()
    {
        deck.ShuffleDeck();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
