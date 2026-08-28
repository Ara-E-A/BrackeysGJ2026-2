using System;

/// <summary>
/// A single, linear NPC conversation. Not a MonoBehaviour: one instance is created per
/// interaction by <see cref="NPC"/> and kept alive by the button callback closures.
///
/// Flow:
///   greeting  -> [Continue] [Leave]
///     Continue -> clue line -> [Thanks] [I see] [How kind of you] -> end
///     Leave    -> end (no clue)
/// </summary>
public class NPCConversation
{
    private enum Stage { Greeting, Clue, Done }

    private readonly NPCDialogue dialogue;
    private readonly ClueHolder clueHolder;
    private readonly ShowDialogue ui;

    private Stage stage = Stage.Greeting;

    public NPCConversation(NPCDialogue dialogue, ClueHolder clueHolder, ShowDialogue ui)
    {
        this.dialogue = dialogue;
        this.clueHolder = clueHolder;
        this.ui = ui;
    }

    public void Begin()
    {
        stage = Stage.Greeting;
        dialogue.MarkInteracted();

        ui.ShowNPCLine(
            dialogue.GetStartingLine(),
            Choice(PlayerDialogueOption.Continue, ShowClue),
            Choice(PlayerDialogueOption.Leave, End));
    }

    private void ShowClue()
    {
        if (stage != Stage.Greeting)
        {
            return;
        }

        stage = Stage.Clue;

        string line = clueHolder != null && clueHolder.clue != null
            ? dialogue.BuildMessageFromClue(clueHolder.clue)
            : "Sorry, I've got nothing for you.";

        ui.ShowNPCLine(
            line,
            Choice(PlayerDialogueOption.Thanks, End),
            Choice(PlayerDialogueOption.ISee, End),
            Choice(PlayerDialogueOption.HowKind, End));
    }

    private void End()
    {
        if (stage == Stage.Done)
        {
            return;
        }

        stage = Stage.Done;
        ui.EndNPCLine();
    }

    private static InteractEventOption Choice(PlayerDialogueOption option, Action onClick)
    {
        return new InteractEventOption(option.ToLabel(), () => onClick());
    }
}
