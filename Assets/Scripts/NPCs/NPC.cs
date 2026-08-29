using UnityEngine;

public class NPC : Interactable
{
    private NPCDialogue npcDialogue;
    private AudioSource audioSource;
    public static NPC currentlySpeakingNpc;

    protected override void Awake()
    {
        base.Awake();
        npcDialogue = GetComponent<NPCDialogue>();
        audioSource = GetComponent<AudioSource>();
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
        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.Play();
            currentlySpeakingNpc = this;
        }

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
        if (audioSource != null)
        {
            audioSource.Stop();
        }

        if (currentlySpeakingNpc == this)
        {
            currentlySpeakingNpc = null;
        }
    }
}
