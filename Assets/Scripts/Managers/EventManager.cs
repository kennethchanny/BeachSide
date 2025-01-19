using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    //Handles Events

    public static EventManager current;

    //Initialize Event manager
    private void Awake()
    {
        current = this;
    }


    //FUNCTIONS TO NOTE
    //Calling the event--------------------
    //EventManager.current.EventTriggered()

    //Subscribing to the event ------------ (Start Func)
    //EventManager.current.onEventTriggered += (Function);

    //Unsubscribing to the event ---------- (OnDestroy Func)
    //EventManager.current.onEventTriggered -= (Function);

    //If u want a specific ID of an object to trigger it
    //declare an ID
    //private int id = "take from name or instance count';

    //Calling the event--------------------
    // EventManager.current.LeverTriggered(id);


    /* public event Action<int> onLeverTriggered;
     public void LeverTriggered(int id)
     {
         if (onLeverTriggered != null)
         {
             onLeverTriggered(id);
         }

     }*/

    // Event for digging


    #region PlayerActionEvents



    // Event to trigger the onDig event
    public event Action onDig;
    public void DigEvent()
    {
        onDig?.Invoke(); // Trigger dig event for subscribers
    }

    //Event to trigger the onBury event
    public event Action onBury;
    public void BuryEvent()
    {
        onBury?.Invoke(); // Trigger burying hole event for subscribers
    }
    #endregion

    #region GlobalUtilEvents

    // Event for spawning FX with an ID
    public event Action<string> onSpawnFXEvent;
    public void SpawnFXEvent(string id)
    {
        onSpawnFXEvent?.Invoke(id); // Trigger the event with the specified ID
    }
    #endregion
}


