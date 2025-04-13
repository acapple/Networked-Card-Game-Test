using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A scriptable object, so I can make one effect of "You draw a card" and then just drag it in to all cards for cardeffects for draw 1 card as an example
[CreateAssetMenu(fileName = "CardEffectScriptableObject", menuName = "ScriptableObjects/Card/CardEffect")]
public class CardEffect : ScriptableObject
{
    public cardTargetEnum who;

    public cardEffectEnum what;
    public int amount;

    public cardRangeEnum where;
    [Tooltip("(min,max,width) width only used for pick section, so you can pick a section 7 it gets the w sections next to it.")]
    public Vector3Int distances;

    public cardDurationEnum when;
    //whenTriggers triggers;
}

public enum cardTargetEnum
{
    none,
    self,
    oneAlly,
    allAllies,
    oneEnemy,
    allEnemies,
    everyone
}

public enum cardEffectEnum
{
    drawCard,
    dealDamage,
    heal,
    move,
    extraAction,
    push
}

public enum cardRangeEnum
{
    self,
    distancePickSection,
    distanceDeterminedSelection
}

public enum cardDurationEnum
{
    instant,
    ongoingTimed,
    ongoingEvent
}
