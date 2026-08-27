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

        ClueHolder holder = GetComponent<ClueHolder>();

        if (!npcDialogue.HasInteracted)
        {
            string line = npcDialogue.GetStartingLine();
            npcDialogue.MarkInteracted();
            dialogueUI.ShowNPCDialogue(line, PlayerDialogueOption.Continue);
        }
        else
        {
            string message = npcDialogue.BuildMessageFromClue(holder.clue);
            dialogueUI.ShowNPCDialogue(message, PlayerDialogueOption.Leave);

            DBoxControl.WakeyWakey();

            ShowDialogue refreshedUI = FindAnyObjectByType<ShowDialogue>();
            refreshedUI.showDialogue(message);
        }
    }
}
