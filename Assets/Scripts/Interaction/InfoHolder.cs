using UnityEngine;

/// <summary>
/// A readable world object (sign, poster, scrap of paper) built on <see cref="Interactable"/>.
/// Shows ONE line of information through the same dialogue box as NPCs
/// (<see cref="ShowDialogue"/> / <see cref="DBoxControl"/>), with a single Leave button.
///
/// Text source, in priority order:
///   1. <see cref="overrideText"/> if set - arbitrary hand-authored info.
///   2. A sibling <see cref="NPCDialogue"/>'s Souls-wrapped clue, if that component is present.
///   3. Otherwise <see cref="ClueInterpreter"/> on a sibling <see cref="ClueHolder"/>'s
///      clue. The clue's <see cref="ClueTruth"/> controls whether it reads true / false /
///      half-true / misleading. Falls back to a vague line when nothing is wired.
/// </summary>
public class InfoHolder : Interactable
{
    [TextArea]
    [Tooltip("Shown verbatim. If empty, a sibling ClueHolder's clue is interpreted instead.")]
    public string overrideText;

    protected override void Interact()
    {
        ShowDialogue ui = FindAnyObjectByType<ShowDialogue>();
        if (ui == null)
        {
            Debug.LogError("ShowDialogue UI not found in scene!");
            return;
        }

        string text = GetInfoText();
        if (string.IsNullOrWhiteSpace(text))
        {
            text = "There is nothing to read here.";
        }

        ui.ShowNPCLine(text, new InteractEventOption(
            PlayerDialogueOption.Leave.ToLabel(),
            ui.EndNPCLine));
    }

    public string GetInfoText()
    {
        if (!string.IsNullOrWhiteSpace(overrideText))
        {
            return overrideText;
        }

        Clue clue = TryGetComponent(out ClueHolder holder) ? holder.clue : null;

        return TryGetComponent(out NPCDialogue dialogue)
            ? dialogue.BuildMessageFromClue(clue)
            : ClueInterpreter.Interpret(clue);
    }
}
