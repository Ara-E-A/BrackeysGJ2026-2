using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    // Kept so subclasses can still call base.Awake(); no shared setup any more.
    protected virtual void Awake() { }

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

    /// <summary>
    /// The non-NPC info-display pipeline: opens the shared dialogue window
    /// (<see cref="ShowDialogue"/> / <see cref="DBoxControl"/>) with one message and a
    /// single Leave option. Used by <see cref="Thing"/>.
    /// </summary>
    protected void ShowInfo(string message)
    {
        ShowDialogue ui = FindAnyObjectByType<ShowDialogue>();
        if (ui == null)
        {
            Debug.LogError("Interactable.ShowInfo: no ShowDialogue in the scene.");
            return;
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            message = "There is nothing to read here.";
        }

        ui.ShowNPCLine(message, new InteractEventOption(
            PlayerDialogueOption.Leave.ToLabel(),
            ui.EndNPCLine));
    }
}
