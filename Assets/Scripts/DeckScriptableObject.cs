using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DeckScriptableObject", menuName = "ScriptableObjects/Card/Deck")]
public class DeckScriptableObject : ScriptableObject
{
    public CardScriptableObject[] startingCards;
}
