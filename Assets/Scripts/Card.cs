using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Card : MonoBehaviour
{
    [SerializeField]
    internal string title = "Card";

    // Start is called before the first frame update
    void Initialize()
    {
        
    }

    internal virtual void OnPlay()
    {
        Debug.Log("Card played: "+title);
    }

    protected virtual void OnPlayAnyCard()
    {
        
    }

    protected virtual void OnDraw()
    {

    }

    protected virtual void OnDiscard()
    {

    }
}
