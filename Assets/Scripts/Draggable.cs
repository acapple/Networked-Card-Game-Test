using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    private IEnumerator dragging;
    internal Vector3 startPos;
    [SerializeField]
    private bool returning = true;


    public int getMapSection()
    {
        return Terrain.terrain.getMapSection(transform.position);
    }


    public void dragCard()
    {
        dragging = cardBeingDragged();
        startPos = transform.position;
        StartCoroutine(dragging);
    }
    public IEnumerator cardBeingDragged()
    {
        while (true)
        {
            transform.position = Input.mousePosition;
            yield return null;
        }
    }
    public void releaseDrag()
    {
        StopCoroutine(dragging);
        int section = Terrain.terrain.getMapSection(transform.position);
        if (section == -1)
        {
            transform.position = startPos;
        }
        else if (returning) 
        {
            transform.position = startPos;
        }
    }
}
