using UnityEngine;

/// <summary>
/// Fills every <see cref="NPCDialogue"/> in the scene from <see cref="SoulsDialogueBank"/>.
///
/// Driven by <see cref="GameManager"/> (via <see cref="Populate"/>) rather than its own
/// Start() so ordering is deterministic - same pattern as <see cref="ClueDistributor"/>.
/// </summary>
public class NPCDialoguePopulator : MonoBehaviour
{
    [Tooltip("Replace phrase banks even on NPCs that already have custom lines. " +
             "Off = only fill banks that are empty or still hold placeholder stubs.")]
    [SerializeField] private bool overwriteExisting = true;

    public void Populate()
    {
        NPCDialogue[] all = FindObjectsByType<NPCDialogue>(FindObjectsInactive.Include);

        int filled = 0;
        foreach (NPCDialogue dialogue in all)
        {
            if (!overwriteExisting && !dialogue.IsUnpopulated())
            {
                continue;
            }

            dialogue.LoadLibrary(
                SoulsDialogueBank.StartingLines,
                SoulsDialogueBank.PartsA,
                SoulsDialogueBank.PartsB,
                SoulsDialogueBank.PartsC);
            filled++;
        }

        Debug.Log($"NPCDialoguePopulator: filled {filled}/{all.Length} NPCDialogue bank(s).");
    }
}
