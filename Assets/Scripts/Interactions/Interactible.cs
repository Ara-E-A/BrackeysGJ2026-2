using UnityEngine;

public abstract class Interactible : MonoBehaviour
{
    public EventUI evUI;

    protected virtual void Awake()
    {
        evUI = FindAnyObjectByType<EventUI>();
    }

    public void OnInteract()
    {
        if (DBoxControl.speaking)
        {
            Debug.Log("An event or dialogue is already open.");
            return;
        }

        Interact();
    }

    protected abstract void Interact();
}
