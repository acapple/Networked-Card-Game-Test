using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class Card : NetworkBehaviour
{
    [SerializeField]
    internal string title = "Card";
    internal int keyInDeck = -1;
    internal Deck startingDeck;
    [SerializeField]
    internal CardScriptableObject[] cardEffects;
    private IEnumerator dragging;
    internal Vector3 startPos;


    /// <summary>
    /// When a client clicks on a card
    /// </summary>
    public void CardReleased()
    {
        if (GameManager.gm.repetitiveMessages) NetworkHud.nh.print("Card is pressed");
        if (NetworkManager.Singleton.IsClient)
        {
            if (NetworkHud.localPlr != null)
            {
                List<Card> hand = NetworkHud.localPlr.hand;
                for (int i=0; i<hand.Count; i++)
                {
                    if (hand[i] == this)
                    {
                        NetworkHud.localPlr.RequestCardPlayedServerRPC(i);
                        break;
                    }
                }
            }
        }
    }

    public void dragCard()
    {
        dragging = cardBeingDragged();
        startPos = transform.position;
        StartCoroutine(dragging);
    }
    public IEnumerator cardBeingDragged()
    {
        while(true)
        {
            transform.position = Input.mousePosition;
            yield return null;
        }
    }
    public void releaseDrag()
    {
        StopCoroutine(dragging);
        int section = Terrain.terrain.getMapSection(transform.position);
        if (section == -1)
        {
            transform.position = startPos;
        } else
        {
            transform.position = startPos;
            CardReleased();
        }
    }


    /// <summary>
    /// Play the card (if its the server)
    /// </summary>
    /// <returns></returns>
    internal virtual bool ThisCardPlay(int player)
    {
        NetworkHud.nh.print("Trying to play card " +title);
        if (!NetworkManager.Singleton.IsServer) return false;
        if (GameManager.gm.State != GameManager.gameState.PlayersTurn)
        {
            NetworkHud.nh.print("Card attempted to be played during enemy turn");
            return false;
        }

        for (int i=0; i<cardEffects.Length; i++)
        {
            if (GameManager.gm.repetitiveMessages) NetworkHud.nh.print("Card effect going into play");
            switch (cardEffects[i].effect)
            {
                case "drawCard":
                    NetworkHud.nh.print("Drawing a card for playerid: "+player);
                    //Server draws a player's card
                    Player.playersInGame[player].AddCardToHand(startingDeck.drawCard());
                    break;
            }
        }
        return true;
    }

    protected virtual void AnyCardPlay()
    {
        if (!NetworkManager.Singleton.IsServer) return;

    }

    protected virtual void ThisCardDraw()
    {
        if (!NetworkManager.Singleton.IsServer) return;

    }

    protected virtual void AnyCardDraw()
    {
        if (!NetworkManager.Singleton.IsServer) return;

    }

    protected virtual void AnyCardDiscard()
    {
        if (!NetworkManager.Singleton.IsServer) return;

    }

    protected virtual void ThisCardDiscard()
    {
        if (!NetworkManager.Singleton.IsServer) return;

    }
}
