using UnityEngine;

public class NPC : Interactible
{
    private NPCDialogue npcDialogue;
    private NPCConversation conversation;

    protected override void Awake()
    {
        base.Awake();
        npcDialogue = GetComponent<NPCDialogue>();
    }

    protected override void Interact()
    {
        ShowDialogue dialogueUI = FindAnyObjectByType<ShowDialogue>();
        if (dialogueUI == null)
        {
            Debug.LogError("ShowDialogue UI not found in scene!");
            return;
        }

        if (npcDialogue == null)
        {
            dialogueUI.showDialogue("NPC has no dialogue.");
            return;
        }

        conversation = new NPCConversation(npcDialogue, GetComponent<ClueHolder>(), dialogueUI);
        conversation.Begin();
    }
}
