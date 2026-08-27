using UnityEngine;

public static class EventActions
{
    //TODO: Implement other event functions.
    public static void addNameInfo()
    {}

    public static void addHeightInfo()
    {}

    public static void addOriginInfo()
    {}

    public static void addSexInfo()
    {}

    public static void addIDInfo()
    {}

    public static void turnAway()
    {
        EventUI eventUI = Object.FindAnyObjectByType<EventUI>();
        if (eventUI == null)
        {
            Debug.LogError("EventUI not found in scene!");
            return;
        }

        eventUI.unshowEvent();
    }

    

}