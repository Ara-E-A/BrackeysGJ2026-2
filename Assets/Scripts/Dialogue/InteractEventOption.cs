using UnityEngine.Events;

/// <summary>One labelled choice button in the dialogue window (label + click action).</summary>
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