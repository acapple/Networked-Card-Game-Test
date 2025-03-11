using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    private IEnumerator dragging;
    internal Vector3 startPos;
    enum whenReturn { never, always, notPlayerTurn}
    [SerializeField]
    private whenReturn returning = whenReturn.always;


    /// <summary>
    /// get the section of the map this object is in
    /// </summary>
    /// <returns> the integer representing what section of the map this object is in</returns>
    public int getMapSection()
    {
        return Terrain.terrain.getMapSection(transform.position);
    }

    /// <summary>
    /// Start a coroutine to keep this object with the mouse as it's being dragged
    /// </summary>
    public void dragCard()
    {
        dragging = cardBeingDragged();
        startPos = transform.position;
        StartCoroutine(dragging);
    }

    /// <summary>
    /// Coroutine to move the object with the mouse
    /// </summary>
    public IEnumerator cardBeingDragged()
    {
        while (true)
        {
            transform.position = Input.mousePosition;
            yield return null;
        }
    }

    /// <summary>
    /// Stop the mouse drag coroutine
    /// </summary>
    public void releaseDrag()
    {
        StopCoroutine(dragging);
        int section = Terrain.terrain.getMapSection(transform.position);
        if (section == -1)
        {
            transform.position = startPos;
        }
        else if (doReturn()) 
        {
            transform.position = startPos;
        }
    }


    /// <summary>
    /// Default is never return
    /// </summary>
    /// <returns> true if the draggable object should return to it's start position</returns>
    private bool doReturn()
    {
        switch (returning)
        {
            case whenReturn.never:
                return false;
            case whenReturn.always:
                return true;
            case whenReturn.notPlayerTurn:
                return GameManager.gm.State == GameManager.gameState.PlayersTurn;
        }
        return false;
    }
}
