using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


/// <summary>
/// This class contains the code needed to execute the cards. 
/// 
/// TODO: Separate it so it only exists when a certain card is in the player's hand. Cards that aren't should be just a cardReference
/// </summary>
public class Card : NetworkBehaviour
{
    #region variables


    [SerializeField]
    internal string title = "Card";
    internal int keyInDeck = -1;
    internal Deck startingDeck;
    [SerializeField]
    internal CardScriptableObject cardReference;


    #endregion variables

    #region playing card

    #region [PC] client functions
    /// <summary>
    /// [Client Only] When a client clicks on a card
    /// </summary>
    public void CardReleased()
    {
        if (GameManager.gm.repetitiveMessages) NetworkHud.nh.print("Card is pressed");
        if (NetworkManager.Singleton.IsClient)
        {
            if (NetworkHud.localPlr != null)
            {
                List<Card> hand = NetworkHud.localPlr.hand;
                for (int i=0; i<hand.Count; i++)
                {
                    if (hand[i] == this)
                    {
                        NetworkHud.localPlr.RequestCardPlayedServerRPC(i, Terrain.terrain.getMapSection(transform.position));
                        break;
                    }
                }
            }
        }
    }

    #endregion client playing card subregion

    #region  [PC] server functions

    /// <summary>
    /// [Server Only] First, checks to see if the card can be played, 
    /// then calls organizeCardEffect for each effect to put the card effects into play
    /// 
    /// TODO: Add more checks to make sure card can be played here BEFORE starting to put effects into play
    /// </summary>
    /// <returns></returns>
    internal virtual bool ThisCardPlay(Target user, int section)
    {
        NetworkHud.nh.print("Trying to play card " +title);
        if (!NetworkManager.Singleton.IsServer) return false;
        if (GameManager.gm.State != GameManager.gameState.PlayersTurn)
        {
            NetworkHud.nh.print("Card attempted to be played during enemy turn");
            return false;
        }
        if (user is Player)
        {
            if (((Player)user).actions.x <= 0) {

                NetworkHud.nh.print("Card attempted to be played, but the player is out of actions");
                return false;
            }

            ((Player)user).actions.x--;
        }
        for (int i=0; i<cardReference.effects.Length; i++)
        {
            if (GameManager.gm.repetitiveMessages) NetworkHud.nh.print("Card effect going into play");
            organizeCardEffect(cardReference.effects[i], user, section);
        }
        return true;
    }

    internal static bool playCard(CardScriptableObject card, Target user, int section)
    {
        NetworkHud.nh.print("Trying to play card " + card.cardName);
        if (!NetworkManager.Singleton.IsServer) return false;
        if (GameManager.gm.State != GameManager.gameState.EnemyTurn)
        {
            NetworkHud.nh.print("Card attempted to be played during player turn");
            return false;
        }
        for (int i = 0; i < card.effects.Length; i++)
        {
            if (GameManager.gm.repetitiveMessages) NetworkHud.nh.print("Card effect going into play");
            organizeCardEffect(card.effects[i], user, section);
        }
        return true;
    }


    /// <summary>
    /// [Server Only] Decompile the card's effect, in the order where, who, what. 
    /// 
    /// TODO: 
    ///     Add Boost effects; 
    ///     after enemies implemented, add enemy to who; 
    ///     add extra action after implementing action limit
    ///     add missing cases as I think of them; 
    /// </summary>
    /// <param name="effect">The effect to be decompiled</param>
    /// <param name="user">the player playing the card</param>
    /// <param name="section">the section that card is played in</param>
    private static void organizeCardEffect(CardEffect effect, Target user, int section)
    {
        if (!NetworkManager.Singleton.IsServer) return;
        //Figure out what sections
        bool[] validSections = new bool[Terrain.terrain.numSections];
        for (int i = 0; i<validSections.Length; i++) validSections[i] = false;
        int playerPosition = Terrain.terrain.getMapSection(user.image.transform.position);
        switch (effect.where)
        {
            case cardRangeEnum.self:
                if (playerPosition == -1) return;
                validSections[playerPosition] = true;
                break;
            case cardRangeEnum.distanceDeterminedSelection:
                for (int i=effect.distances.x; i<effect.distances.y; i++)
                    validSections[(i + playerPosition) % Terrain.terrain.numSections] = true;
                break;
            case cardRangeEnum.distancePickSection:
                //If the selected section is between the min and max sections
                if ((effect.distances.x + playerPosition) % Terrain.terrain.numSections <= section && (effect.distances.y + playerPosition) % Terrain.terrain.numSections >= section)
                {
                    validSections[section] = true;
                }
                break;
        }

        //Get targets
        List<Target> allTargetsOfCard = new List<Target>();
        switch (effect.who)
        {
            case cardTargetEnum.self:
                allTargetsOfCard.Add(user);
                break;
            case cardTargetEnum.oneAlly:
            case cardTargetEnum.allAllies:
                //figure out a way to pick an ally. For now, may do random of all selected allies
                List<Target> temp;
                int min = -1;
                for (int i=0; i<validSections.Length; i++)
                {
                    if (!validSections[i] && min == -1) continue;
                    if (validSections[i] && min == -1)
                    {
                        min = i;
                    }
                    if (validSections[i] && (i+1 == validSections.Length || !validSections[i+1]))
                    {
                        temp = Terrain.terrain.getPlayersBetweenSection(min, i);
                        min = -1;
                        for (int j = 0; j < temp.Count; j++)
                        {
                            allTargetsOfCard.Add(temp[j]);
                        }
                    }
                    
                }
                if (effect.who == cardTargetEnum.allAllies) break;
                if (allTargetsOfCard.Count > 1)
                {
                    int playerPicked = Random.Range(0, allTargetsOfCard.Count);
                    for (int i= allTargetsOfCard.Count-1; i>=0; i--)
                    {
                        if (i == playerPicked) continue;
                        allTargetsOfCard.RemoveAt(i);
                    }
                }
                break;
            case cardTargetEnum.everyone:
                List<Target> temp2;
                int min2 = -1;
                for (int i = 0; i < validSections.Length; i++)
                {
                    if (!validSections[i] && min2 == -1) continue;
                    if (validSections[i] && min2 == -1)
                    {
                        min2 = i;
                    }
                    if (validSections[i] && (i + 1 == validSections.Length || !validSections[i + 1]))
                    {
                        temp2 = Terrain.terrain.getPlayersBetweenSection(min2, i);
                        min2 = -1;
                        for (int j = 0; j < temp2.Count; j++)
                        {
                            allTargetsOfCard.Add(temp2[j]);
                        }
                    }
                }
                min2 = -1;
                for (int i = 0; i < validSections.Length; i++)
                {
                    if (!validSections[i] && min2 == -1) continue;
                    if (validSections[i] && min2 == -1)
                    {
                        min2 = i;
                    }
                    if (validSections[i] && (i + 1 == validSections.Length || !validSections[i + 1]))
                    {
                        temp2 = Terrain.terrain.getEnemiesBetweenSection(min2, i);
                        min2 = -1;
                        for (int j = 0; j < temp2.Count; j++)
                        {
                            allTargetsOfCard.Add(temp2[j]);
                        }
                    }

                }
                break;
            case cardTargetEnum.oneEnemy:
            case cardTargetEnum.allEnemies:
                List<Target> temp3;
                int min3 = -1;
                for (int i = 0; i < validSections.Length; i++)
                {
                    if (!validSections[i] && min3 == -1) continue;
                    if (validSections[i] && min3 == -1)
                    {
                        min3 = i;
                    }
                    if (validSections[i] && (i + 1 == validSections.Length || !validSections[i + 1]))
                    {
                        temp3 = Terrain.terrain.getEnemiesBetweenSection(min3, i);
                        min3 = -1;
                        for (int j = 0; j < temp3.Count; j++)
                        {
                            allTargetsOfCard.Add(temp3[j]);
                        }
                    }

                }
                if (effect.who == cardTargetEnum.allEnemies || effect.who == cardTargetEnum.everyone) break;
                if (allTargetsOfCard.Count > 1)
                {
                    int playerPicked = Random.Range(0, allTargetsOfCard.Count);
                    for (int i = allTargetsOfCard.Count - 1; i >= 0; i--)
                    {
                        if (i == playerPicked) continue;
                        allTargetsOfCard.RemoveAt(i);
                    }
                }
                break;
        }

        // bool[] validSections          :: A boolean array where validSections[i] is if the card is hitting section i
        // List<Target> allTargetsOfCard :: A list of all targets a card is affecting

        //call effect
        switch (effect.what)
        {
            case cardEffectEnum.drawCard:
                NetworkHud.nh.print("Drawing a card for playerid: " + user);
                //For each target, if they're a player, they draw a card from their own deck equal to the effect ammount
                for (int i=0; i<allTargetsOfCard.Count; i++)
                {
                    if (allTargetsOfCard[i] is Player) {
                        for (int j=0; j<effect.amount; j++) 
                            ((Player)allTargetsOfCard[i]).AddCardToHand(((Player)allTargetsOfCard[i]).deck.drawCard());
                    }
                }
                //Server draws a player's card
                //Player.playersInGame[player].AddCardToHand(startingDeck.drawCard());
                break;
            case cardEffectEnum.dealDamage:
                for (int i = 0; i < allTargetsOfCard.Count; i++)
                {
                    allTargetsOfCard[i].health.Value -= effect.amount;
                }
                break;
            case cardEffectEnum.heal:
                for (int i = 0; i < allTargetsOfCard.Count; i++)
                {
                    allTargetsOfCard[i].health.Value += effect.amount;
                }
                break;
            case cardEffectEnum.move:
                for (int i = 0; i < allTargetsOfCard.Count; i++)
                {
                    if (allTargetsOfCard[i] is Player)
                    {
                        NetworkHud.nh.print("Requesting move to section "+section);
                        ((Player)allTargetsOfCard[i]).playerRequestMoveServerRPC(section);
                    }
                }
                break;
            case cardEffectEnum.extraAction:
                for (int i = 0; i < allTargetsOfCard.Count; i++)
                {
                    if (allTargetsOfCard[i] is Player)
                    {
                        ((Player)allTargetsOfCard[i]).actions.y += effect.amount;
                    }
                }
                break;
            case cardEffectEnum.push:
                for (int i = 0; i < allTargetsOfCard.Count; i++)
                {
                    if (allTargetsOfCard[i] is Player)
                    {
                        NetworkHud.nh.print("Requesting move to section " + Terrain.terrain.getMapSection(allTargetsOfCard[i].image.transform.position));
                        ((Player)allTargetsOfCard[i]).playerRequestMoveServerRPC(Terrain.terrain.getMapSection(allTargetsOfCard[i].image.transform.position) + effect.amount);
                    }
                }
                break;
        }
    }

    #endregion

    #endregion
}
