using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class InteractEventOption
{
    public string label;
    public UnityEvent EventAction = new UnityEvent();

    public InteractEventOption()
    {
    }

    public InteractEventOption(string label, UnityAction action)
    {
        this.label = label;
        EventAction.AddListener(action);
    }
}

[System.Serializable]
public class InteractEvent
{
    [TextArea]
    public string eventText;
    [Range(2, 4)]
    public InteractEventOption[] options = new InteractEventOption[2]
    {
        new InteractEventOption(),
        new InteractEventOption()
    };
}