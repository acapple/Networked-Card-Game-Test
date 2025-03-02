using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "CardScriptableObject", menuName = "ScriptableObjects/Card/Card")]
public class CardScriptableObject : ScriptableObject
{
    public string cardName = "Not a card.";
    public int power;
    public CardEffect[] effects;
}
