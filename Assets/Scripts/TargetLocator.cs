using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetLocator
{
    static List<TargetLocator> allTargets;

    internal Target who;
    internal int section;

    internal TargetLocator(Target newTarget)
    {
        who = newTarget;
        section = Terrain.terrain.getMapSection(newTarget.image.transform.position);
        allTargets.Add(this);
    }

    internal bool isPlayer()
    {
        return who is Player;
    }

    internal static void initiate()
    {
        allTargets = new List<TargetLocator>();
    }

    internal static TargetLocator[] getPlayers()
    {
        int count = Player.playersInGame.Count;
        TargetLocator[]  players = new TargetLocator[count];
        count = 0;
        for (int i=0; i<allTargets.Count; i++)
        {
            if (!allTargets[i].isPlayer()) continue;
            players[count] = allTargets[i];
            count++;
            if (count >= players.Length) break;
        }
        return players;
    }


    internal static TargetLocator[] getEnemies()
    {
        int count = allTargets.Count - Player.playersInGame.Count;
        TargetLocator[] Enemies = new TargetLocator[count];
        count = 0;
        for (int i = 0; i < allTargets.Count; i++)
        {
            if (allTargets[i].isPlayer()) continue;
            Enemies[count] = allTargets[i];
            count++;
            if (count >= Enemies.Length) break;
        }
        return Enemies;
    }

    internal static void move(Target who, int where)
    {
        for (int i=0; i<allTargets.Count; i++)
        {
            if (who == allTargets[i].who)
            {
                allTargets[i].section = where;
                break;
            }
        }
    }
}
