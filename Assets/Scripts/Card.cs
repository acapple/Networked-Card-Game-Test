using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[System.Serializable]
public class Card : NetworkBehaviour
{
    [SerializeField]
    internal string title = "Card";

    // Start is called before the first frame update
    void Initialize()
    {
        
    }

    public void CardPressed()
    {
        Debug.Log(NetworkHud.getType() + "Card is pressed");
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

    internal virtual bool ThisCardPlay()
    {
        Debug.Log(NetworkHud.getType() + "Trying to play card "+title);
        if (!NetworkManager.Singleton.IsServer) return false;
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
