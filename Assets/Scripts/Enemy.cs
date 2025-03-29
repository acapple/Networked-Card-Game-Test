using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Enemy : Target
{

    [Rpc(SendTo.Server)]
    internal override void RequestCardPlayedServerRPC(int cardNum, int section)
    {
        Card c = deck.drawCard();
        if (Card.playCard(c.cardReference, this, Terrain.terrain.getMapSection(image.transform.position)))
        {
            deck.discardCard(c);
        } else
        {

        }


        /*NetworkHud.nh.print("Got asked to play card numbered: " + cardNum + " out of " + hand.Count + " cards"/*" named: " + hand[cardNum].title);
        //Debug.Log("Player asked to play card numbered: " + cardNum + " named: " + hand[cardNum].name);
        NetworkHud.nh.print("Player ID during the request card played: " + playerID + ", " + playersInGame.Values);
        if (hand[cardNum].ThisCardPlay(playerID, section))
        {
            RemoveCardFromHandClientRPC("true", cardNum);
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
        }*/
    }
}
