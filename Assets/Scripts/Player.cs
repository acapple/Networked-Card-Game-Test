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
    Deck deck;
    [SerializeField]
    GameObject cardHandSample;
    [SerializeField]
    Canvas canvas;
    [SerializeField]
    float cardSpacing = 95;
    [SerializeField]
    int StartingHandSize = 5;

    // Start is called before the first frame update
    void Start()
    {
        if (!IsLocalPlayer || !IsOwner) return;
        deck.ShuffleDeck();
        for (int i=0; i<StartingHandSize; i++)
        {
            AddCardToHand(deck.drawCard());
        }
        //SortHand();
    }


    internal void AddCardToHand(Card c)
    {
        if (!IsLocalPlayer || !IsOwner) return;
        hand.Add(c);
        for (int i=0; i<hand.Count; i++)
        {
            hand[i].GetComponent<Image>().color = new Color(Random.Range(0.0f, 1), Random.Range(0.0f, 1), Random.Range(0.0f, 1)); // Temporary
            int x = (int) Mathf.Floor(canvas.pixelRect.width * 0.5f + cardSpacing * i - 0.5f * cardSpacing * (hand.Count - 1));
            hand[i].transform.position = new Vector3(x, 0, 0);
        }
    }


    [ClientRpc]
    internal void RemoveCardFromHandClientRPC(string result, int card)
    {
        if (!IsLocalPlayer || !IsOwner) return;
        Card c = hand[card];
        hand.RemoveAt(card);
        c.gameObject.SetActive(false);
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
        Debug.Log("Player asked to play card numbered: " + cardNum + " named: " + hand[cardNum].name);
        if (hand[cardNum].ThisCardPlay()) RemoveCardFromHandClientRPC("true", cardNum);
    }
}
