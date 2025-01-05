using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public static EventManager instance;


    // NOTICE because these events are private they must be initialized in awake, otherwise they produce a nulref error
    private UnityEvent<EventParams> onCardDrawEarly;
    private UnityEvent<EventParams> onCardDraw;
    private UnityEvent<EventParams> onCardDrawLate;

    private void Awake()
    {
        if (instance == null) instance = this;

        onCardDrawEarly = new UnityEvent<EventParams>();
        onCardDraw = new UnityEvent<EventParams>();
        onCardDrawLate = new UnityEvent<EventParams>();
    }

    /// <summary>
    /// adds an action to the specified event with the given priority
    /// </summary>
    /// <param name="eventType"> the type of event that is to be added to </param>
    /// <param name="eventPriority"> which priority of the event that is being added to, 0 -> early, 1 -> regular, 2 -> late </param>
    /// <param name="action"> the action that is being added, the function being called from the invoking of the event </param>
    public void addListener(Events eventType, UnityAction<EventParams> action, int eventPriority = 1)
    {
        switch (eventType)
        {
            case Events.onCardDraw:
                switch (eventPriority)
                {
                    case 0:
                        onCardDrawEarly.AddListener(action);
                        break;
                    case 1:
                        onCardDraw.AddListener(action);
                        break;
                    case 2:
                        onCardDrawLate.AddListener(action);
                        break;
                }
                break;
        }
    }

    /// <summary>
    /// removes an action of the specified event with the given priority
    /// </summary>
    /// <param name="eventType"> the type of event that is to be removed from </param>
    /// <param name="eventPriority">  which priority of the event that is being removed, 0 -> early, 1 -> regular, 2 -> late  </param>
    /// <param name="action"> the action that is being removed, the function being removed from the invoking of the event </param>
    public void removeListener(Events eventType, UnityAction<EventParams> action, int eventPriority = 1)
    {
        switch (eventType)
        {
            case Events.onCardDraw:
                switch (eventPriority)
                {
                    case 0:
                        onCardDrawEarly.RemoveListener(action);
                        break;
                    case 1:
                        onCardDraw.RemoveListener(action);
                        break;
                    case 2:
                        onCardDrawLate.RemoveListener(action);
                        break;
                }
                break;
        }
    }

    /// <summary>
    /// invokes the specified event
    /// </summary>
    /// <param name="eventType"> the event which is to be invoked </param>
    public void invokeEvent(Events eventType, EventParams param = new EventParams())
    {
        //Debug.Log("Invoking event " + eventType);
        switch (eventType)
        {
            case Events.onCardDraw:
                onCardDrawEarly.Invoke(param);
                onCardDraw.Invoke(param);
                onCardDrawLate.Invoke(param);
                break;
            
        }
    }

}

/// <summary>
/// collection of all game events
/// </summary>
public enum Events
{
    onCardDraw
}

//Struct for variables to be passed
public struct EventParams
{
    
}
