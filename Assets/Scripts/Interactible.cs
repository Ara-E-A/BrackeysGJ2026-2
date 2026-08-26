using UnityEngine;

public class Interactible : MonoBehaviour
{
    public enum InteractibleType
    {
        NPC,
        Sign,
        Poster,
        Note,
        Table,
        Display,
        Terminal
    }
    public InteractibleType type;

    private NPCDialogue npcDialogue;

    private void Awake()
    {
        npcDialogue = GetComponent<NPCDialogue>();
    }

    public void OnInteract()
    {
        ShowDialogue dialogueUI = FindAnyObjectByType<ShowDialogue>();
        if (dialogueUI == null)

        if (DBoxControl.speaking)
        {
            Debug.LogError("ShowDialogue UI not found in scene!");
            return;
        }

        if (type == InteractibleType.NPC)
        {
            HandleNPCDialogue(dialogueUI);
        }
        else
        {
            dialogueUI.showDialogue("This object is not an NPC.");
        }
    }

    private void HandleNPCDialogue(ShowDialogue ui)
    {
        if (npcDialogue == null)
        {
            ui.showDialogue("NPC has no dialogue.");
            return;
        }

        ClueHolder holder = GetComponent<ClueHolder>();

        if (!npcDialogue.HasInteracted)
        {
            string line = npcDialogue.GetStartingLine();
            npcDialogue.MarkInteracted();
            ui.ShowNPCDialogue(line, PlayerDialogueOption.Continue);
        }
        else
        {
            string message = npcDialogue.BuildMessageFromClue(holder.clue);
            ui.ShowNPCDialogue(message, PlayerDialogueOption.Leave);

            DBoxControl.WakeyWakey();

            ShowDialogue refreshedUI = FindAnyObjectByType<ShowDialogue>();
            refreshedUI.showDialogue(message); 
        }
    }

}
