using Unity.VisualScripting;
using UnityEngine;

public class Thing : Interactable
{
    private InteractEvent interactEvent;

    public enum type
    {
        Note,
        Table,
        Screen
    }
    public type thingType;

    public void Start()
    {
        interactEvent = getEventForType();
    }

    protected override void Interact()
    {

        if (interactEvent == null)
        {
            Debug.LogError($"No InteractEvent configured on {name}.");
            return;
        }

        if (evUI == null)
        {
            evUI = FindAnyObjectByType<EventUI>();
        }

        if (evUI == null)
        {
            Debug.LogError("EventUI not found in scene!");
            return;
        }

        evUI.showEvent(interactEvent);
    }

    public InteractEvent getEventForType()
    {
        switch(thingType){
            case type.Note:
            return InteractEventCatalog.templates[0];
            case type.Table:
            return InteractEventCatalog.templates[1];
            case type.Screen:
            return InteractEventCatalog.templates[2];
        }
        
        return null;
    }
}
