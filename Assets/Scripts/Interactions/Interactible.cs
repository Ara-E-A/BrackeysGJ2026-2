using UnityEngine;

public abstract class Interactible : MonoBehaviour
{
    public EventUI evUI;

    protected virtual void Awake()
    {
        evUI = FindAnyObjectByType<EventUI>();
    }

    //Definitely not clean but for some reason it was not finding the evUI specifically for table objects..?
    protected virtual void Update() {
        if(evUI == null)
        {
            evUI = FindAnyObjectByType<EventUI>();
        } 
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
