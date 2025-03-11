using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is used to create precon decks. 
/// </summary>
[CreateAssetMenu(fileName = "DeckScriptableObject", menuName = "ScriptableObjects/Card/Deck")]
public class DeckScriptableObject : ScriptableObject
{
    public CardScriptableObject[] startingCards;
}
