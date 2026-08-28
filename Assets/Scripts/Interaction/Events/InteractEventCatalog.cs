using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InteractEventCatalog", menuName = "Events/Interact Event Catalog")]
public class InteractEventCatalog : ScriptableObject
{
    public static List<InteractEvent> templates = new List<InteractEvent>
    {
        new InteractEvent
        {
            eventText = "There's a crusty old piece of paper on the floor, it calls to you as it would to any normal individual.",
            options = new InteractEventOption[]
            {
                new InteractEventOption("Grab it", EventActions.addNameInfo),
                new InteractEventOption("Leave it where it is", EventActions.turnAway)
            }
        },
        new InteractEvent
        {
            eventText = "There are some scribbles etched into the stains on this crusty old table...",
            options = new InteractEventOption[]
            {
                new InteractEventOption("Get Closer", EventActions.addOriginInfo),
                new InteractEventOption("Smells like coffee. ekh. (Turn Away)", EventActions.turnAway)
            }
        },
        new InteractEvent
        {
            eventText = "That old monitor is flashing like it's got something to tell you...",
            options = new InteractEventOption[]
            {
                new InteractEventOption("Try to make out what it's blinking at you", EventActions.addOriginInfo),
                new InteractEventOption("Decide you're not getting haunted today", EventActions.turnAway)
            }
        }
    };
}
