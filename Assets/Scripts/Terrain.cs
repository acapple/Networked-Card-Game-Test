using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terrain : MonoBehaviour
{
    int sections = 8;
    internal int numSections => sections;
    internal static Terrain terrain;
    internal Vector3 offSet;

    private void Awake()
    {
        terrain = this;
        resize();
    }

    public void resize()
    {
        offSet = new Vector3(0, Screen.height * 0.16f);
        GetComponent<CircleCollider2D>().offset = offSet;
        int radius = (int) Mathf.Floor(Screen.height / 2.5f);
        GetComponent<CircleCollider2D>().radius = radius;
    }

    public void mouseUp()
    {
        //NetworkHud.nh.print("Mouse in section: " + getMapSection(Input.mousePosition) + ", in circle: " + inColosseum(Input.mousePosition));
        resize();
    }

    internal bool inColosseum(Vector2 pos)
    {
        offSet = GetComponent<CircleCollider2D>().offset;
        return Vector2.Distance(transform.position + offSet, pos) < GetComponent<CircleCollider2D>().radius;
    }

    internal int getMapSection(Vector2 pos)
    {
        if (!inColosseum(pos)) return -1;
        offSet = GetComponent<CircleCollider2D>().offset;
        float angle = Vector2.Angle(Vector2.right, new Vector2(pos.y - (transform.position.y + offSet.y), pos.x - (transform.position.x + offSet.x)));
        if (pos.x < transform.position.x + offSet.x) angle = 360 - angle;
        angle = angle / 360 * sections;
        return (int) Mathf.Floor(angle);
    }


    internal void moveImage(UnityEngine.UI.RawImage image, int section)
    {
        double angle = (2 * Mathf.PI) / Terrain.terrain.numSections;
        angle = angle * section + angle * 0.5f;
        Vector3 position = new Vector2((float)System.Math.Sin(angle), (float)System.Math.Cos(angle));
        if (GameManager.gm.repetitiveMessages) NetworkHud.nh.print("moving to position: " + position);
        image.transform.position = Terrain.terrain.transform.position + Terrain.terrain.offSet + (position * Terrain.terrain.GetComponent<CircleCollider2D>().radius * 0.75f);
    }


    internal List<Target> getPlayersInSection(int i)
    {
        List<Target> targets = new List<Target>();
        foreach (Player p in Player.playersInGame.Values)
        {
            if (i == getMapSection(p.image.transform.position))
            {
                targets.Add(p);
            }
        }
        return targets;
    }
}
