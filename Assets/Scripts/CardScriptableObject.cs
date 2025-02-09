using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "CardScriptableObject", menuName = "ScriptableObjects/Card")]
public class CardScriptableObject : ScriptableObject
{
    //public string title = "Not a card.";
    public int power;
    public string effect;
}
