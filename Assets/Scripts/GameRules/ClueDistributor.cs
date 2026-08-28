using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Hands generated clues out to the <see cref="ClueHolder"/> components in the scene.
///
/// Driven by <see cref="GameManager"/> right after clue generation (via
/// <see cref="Distribute"/>) so the ordering is deterministic — do not rely on Start()
/// order here.
/// </summary>
public class ClueDistributor : MonoBehaviour
{
    [Tooltip("Deal a random clue to each holder; otherwise deal them in generation order.")]
    [SerializeField] private bool randomize = true;

    [Tooltip("Leave holders that already have a real clue (populated payload) untouched, " +
             "e.g. ones set by NPC_TestSetup.")]
    [SerializeField] private bool skipPreassigned = true;

    public void Distribute(List<Clue> clues)
    {
        if (clues == null || clues.Count == 0)
        {
            Debug.LogWarning("ClueDistributor: no clues to distribute.");
            return;
        }

        ClueHolder[] holders = FindObjectsByType<ClueHolder>(FindObjectsInactive.Include);
        if (holders.Length == 0)
        {
            return;
        }

        List<Clue> pool = new List<Clue>(clues);

        foreach (ClueHolder holder in holders)
        {
            if (skipPreassigned && holder.clue != null && holder.clue.payload != null)
            {
                continue;
            }

            if (pool.Count == 0)
            {
                pool.AddRange(clues); // wrap around when there are more holders than clues
            }

            int index = randomize ? Random.Range(0, pool.Count) : 0;
            holder.clue = pool[index];
            pool.RemoveAt(index);
        }

        Debug.Log($"ClueDistributor: assigned clues to {holders.Length} holder(s).");
    }
}
