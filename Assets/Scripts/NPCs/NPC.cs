using UnityEngine;

public class NPC : Interactable
{
    private NPCDialogue npcDialogue;
    public static NPC currentlySpeakingNpc;

    /// <summary>The talking NPC's dialogue component, used by <see cref="ShowDialogue"/> to gate the voice loop.</summary>
    public NPCDialogue Voice => npcDialogue;

    protected override void Awake()
    {
        base.Awake();
        npcDialogue = GetComponent<NPCDialogue>();
    }

    private void Update()
    {
        if (currentlySpeakingNpc == this && !DBoxControl.speaking)
        {
            StopVoice();
        }
    }

    protected override void Interact()
    {
        // The voice loop is driven by ShowDialogue.typeDialogue (only while text types);
        // interacting just marks who is speaking.
        currentlySpeakingNpc = this;

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

    public void StopVoice()
    {
        if (npcDialogue != null)
        {
            npcDialogue.StopVoice();
        }

        if (currentlySpeakingNpc == this)
        {
            currentlySpeakingNpc = null;
        }
    }
}
