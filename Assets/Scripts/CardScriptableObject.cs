using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A scriptable object holding the card in it's entirety. 
/// 
/// TODO: Any card not in players hand should only need detail from here. 
/// </summary>
[CreateAssetMenu(fileName = "CardScriptableObject", menuName = "ScriptableObjects/Card/Card")]
public class CardScriptableObject : ScriptableObject
{
    public string cardName = "Not a card.";
    //public int power;
    public CardEffect[] effects;
}
