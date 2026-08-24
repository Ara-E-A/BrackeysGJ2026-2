using UnityEngine;

public class Interactible : MonoBehaviour
{
    public string dialogueText = "Hello from this object!";

    public void OnInteract()
    {
        ShowDialogue dialogueUI = FindObjectOfType<ShowDialogue>();

        if (dialogueUI != null)
        {
            dialogueUI.showDialogue(dialogueText);
        }
        else
        {
            Debug.LogError("ShowDialogue UI not found in scene!");
        }
    }
}
