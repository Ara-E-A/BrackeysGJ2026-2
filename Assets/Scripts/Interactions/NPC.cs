using UnityEngine;

public class NPC : Interactible
{
    private NPCDialogue npcDialogue;

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

        ClueHolder clueHolder = GetComponent<ClueHolder>();
        Clue clue = clueHolder != null ? clueHolder.clue : null;

        string greeting = npcDialogue.GetStartingLine();
        string clueText = npcDialogue.BuildMessageFromClue(clue);
        npcDialogue.MarkInteracted();

        DialogManager.Instance.StartNPCDialogue(greeting, clueText);
    }
}
