using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class Card : NetworkBehaviour
{
    [SerializeField]
    internal string title = "Card";
    internal int keyInDeck = -1;
    internal Deck startingDeck;
    [SerializeField]
    internal CardScriptableObject cardEffects;


    /// <summary>
    /// When a client clicks on a card
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

    


    /// <summary>
    /// Play the card (if its the server)
    /// </summary>
    /// <returns></returns>
    internal virtual bool ThisCardPlay(int player, int section)
    {
        NetworkHud.nh.print("Trying to play card " +title);
        if (!NetworkManager.Singleton.IsServer) return false;
        if (GameManager.gm.State != GameManager.gameState.PlayersTurn)
        {
            NetworkHud.nh.print("Card attempted to be played during enemy turn");
            return false;
        }

        for (int i=0; i<cardEffects.effects.Length; i++)
        {
            if (GameManager.gm.repetitiveMessages) NetworkHud.nh.print("Card effect going into play");
            organizeCardEffect(cardEffects.effects[i], player, section);
        }
        return true;
    }


    /// <summary>
    /// Decompile the card's effect
    /// </summary>
    /// <param name="effect">The effect to be decompiled</param>
    /// <param name="player">the player playing the card</param>
    /// <param name="section">the section that card is played in</param>
    private void organizeCardEffect(CardEffect effect, int player, int section)
    {
        //Figure out what sections
        bool[] validSections = new bool[Terrain.terrain.numSections];
        for (int i = 0; i<validSections.Length; i++) validSections[i] = false;
        int playerPosition = Terrain.terrain.getMapSection(Player.playersInGame[player].transform.position);
        switch (effect.where)
        {
            case cardRangeEnum.self:
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
                allTargetsOfCard.Add(Player.playersInGame[player]);
                break;
            case cardTargetEnum.oneAlly:
            case cardTargetEnum.allAllies:
                //figure out a way to pick an ally. For now, may do random of all selected allies
                List<Target> temp;
                for (int i=0; i<validSections.Length; i++)
                {
                    if (!validSections[i]) continue;
                    temp = Terrain.terrain.getPlayersInSection(i);
                    for (int j=0; j<temp.Count; j++) {
                        allTargetsOfCard.Add(temp[j]);
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
        }

        // bool[] validSections          :: A boolean array where validSections[i] is if the card is hitting section i
        // List<Target> allTargetsOfCard :: A list of all targets a card is affecting

        //call effect
        switch (effect.what)
        {
            case cardEffectEnum.drawCard:
                NetworkHud.nh.print("Drawing a card for playerid: " + player);
                //Server draws a player's card
                Player.playersInGame[player].AddCardToHand(startingDeck.drawCard());
                break;
        }
    }
}
