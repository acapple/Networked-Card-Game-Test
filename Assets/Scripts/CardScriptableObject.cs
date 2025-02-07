using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CardScriptableObject", menuName = "ScriptableObjects/Card")]
public class CardScriptableObject : ScriptableObject
{
    public string title = "Not a card.";
    public delegate void CardEffect(CardEffectVariable variables);
    public CardEffect onPlayEffects;
}

public class CardEffectVariable
{
    public Player player;
    public int strength;
    public Target[] targets;

    public CardEffectVariable(Player plr, Target[] trgts, int amnt = 1)
    {
        if (trgts == null) targets = new Target[0];
        else
        {
            targets = new Target[trgts.Length];
            for (int i=0; i<trgts.Length; i++)
            {
                targets[i] = trgts[i];
            }
            player = plr;
            strength = amnt;
        }
    } 
}
