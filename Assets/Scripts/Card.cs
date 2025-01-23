using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card 
{
    // Start is called before the first frame update
    void Initialize()
    {
        
    }

    protected virtual void OnPlay()
    {
        
    }

    protected virtual void OnDraw()
    {

    }

    protected virtual void OnDiscard()
    {

    }
}
