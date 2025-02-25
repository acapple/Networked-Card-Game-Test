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
        offSet = GetComponent<CircleCollider2D>().offset;
    }

    public void mouseUp()
    {
        NetworkHud.nh.print("Mouse in section: " + getMapSection(Input.mousePosition) + ", in circle: " + inColosseum(Input.mousePosition));
    }

    internal bool inColosseum(Vector2 pos)
    {
        return Vector2.Distance(transform.position + offSet, pos) < GetComponent<CircleCollider2D>().radius;
    }

    internal int getMapSection(Vector2 pos)
    {
        if (!inColosseum(pos)) return -1;
        float angle = Vector2.Angle(Vector2.right, new Vector2(pos.y - (transform.position.y + offSet.y), pos.x - (transform.position.x + offSet.x)));
        if (pos.x < transform.position.x + offSet.x) angle = 360 - angle;
        angle = angle / 360 * sections;
        return (int) Mathf.Floor(angle);
    }
}
