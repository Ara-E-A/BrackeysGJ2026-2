using UnityEngine;

/// <summary>
/// A readable world object - a note, a stained table, a flickering screen. Clicking it
/// opens the shared dialogue window with one plain informational clue and a single Leave
/// option (see <see cref="Interactable.ShowInfo"/>).
///
/// Text source:
///   1. <see cref="overrideText"/> if set - arbitrary hand-authored info.
///   2. otherwise <see cref="ClueInterpreter"/> on a sibling <see cref="ClueHolder"/>'s
///      clue. The clue's <see cref="ClueTruth"/> decides whether it reads true / false /
///      half-true / misleading; a missing clue falls back to a vague line.
/// </summary>
public class Thing : Interactable
{
    // Flavour tag only (used by Thingamabob and the inspector); no longer drives behaviour.
    public enum type { Note, Table, Screen, NPC }
    public type thingType;

    [TextArea]
    [Tooltip("Shown verbatim. If empty, a sibling ClueHolder's clue is interpreted instead.")]
    public string overrideText;

    protected override void Interact()
    {
        ShowInfo(GetInfoText());
    }

    public string GetInfoText()
    {
        if (!string.IsNullOrWhiteSpace(overrideText))
        {
            return overrideText;
        }

        Clue clue = TryGetComponent(out ClueHolder holder) ? holder.clue : null;
        return ClueInterpreter.Interpret(clue);
    }
}
