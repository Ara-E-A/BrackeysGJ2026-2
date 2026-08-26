using UnityEngine;

public class Interactible : MonoBehaviour
{
    public string dialogueText = "Hello from this object!";

    public void OnInteract()
    {
        ShowDialogue dialogueUI = FindAnyObjectByType<ShowDialogue>();

        if (DBoxControl.speaking)
        {
            dialogueUI.showDialogue(dialogueText);
        }
        else
        {
            DBoxControl.WakeyWakey();         
            dialogueUI = FindAnyObjectByType<ShowDialogue>();
            dialogueUI.showDialogue(dialogueText);
        }
    }
}
