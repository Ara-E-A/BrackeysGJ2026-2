using System.Collections.Generic;
using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    [TextArea]
    public string[] startingLines;

    [Header("Message building blocks (Souls-style)")]
    [Tooltip("A - opening beat. Complete sentence, sets mood.")]
    [TextArea] public string[] messagePartsA;
    [Tooltip("B - testimony beat. Complete sentence; speaker claims knowledge of the rules.")]
    [TextArea] public string[] messagePartsB;
    [Tooltip("C - closing kicker. Short sign-off spoken just before the hint.")]
    [TextArea] public string[] messagePartsC;

    [Tooltip("Re-roll phrasing on every interaction. Off = each NPC keeps one fixed phrasing " +
             "for its lifetime (Souls messages are immutable).")]
    public bool rerollEachInteraction = false;

    [Header("Voice")]
    [Tooltip("Looping blip that sounds ONLY while a line is typing out. Auto-bound to the " +
             "AudioSource on this object when left empty.")]
    [SerializeField] private AudioSource voiceLoop;

    private bool hasInteracted = false;

    private void Awake()
    {
        if (voiceLoop == null)
        {
            voiceLoop = GetComponent<AudioSource>();
        }
    }

    /// <summary>
    /// Start the typing blip loop. Called by <see cref="ShowDialogue"/> at the top of
    /// <c>typeDialogue()</c>; safe to call again to restart it for the next line.
    /// </summary>
    public void StartVoice()
    {
        if (voiceLoop == null)
        {
            return;
        }

        voiceLoop.loop = true;
        voiceLoop.Stop();
        voiceLoop.Play();
    }

    /// <summary>
    /// Stop the typing blip loop. Called when the final character of a line is rendered and
    /// when the dialogue ends, so the loop never sounds while the player is reading/choosing.
    /// </summary>
    public void StopVoice()
    {
        if (voiceLoop != null)
        {
            voiceLoop.Stop();
        }
    }

    // Per-instance phrasing seed, fixed for the object's lifetime (re-drawn each Play session).
    private int phrasingSeed = -1;
    private int PhrasingSeed =>
        phrasingSeed >= 0 ? phrasingSeed : (phrasingSeed = Random.Range(1, int.MaxValue));

    public bool HasInteracted => hasInteracted;
    public void MarkInteracted() => hasInteracted = true;

    /// <summary>Replaces all four phrase banks. Used by <see cref="NPCDialoguePopulator"/>.</summary>
    public void LoadLibrary(string[] starts, string[] a, string[] b, string[] c)
    {
        startingLines = starts;
        messagePartsA = a;
        messagePartsB = b;
        messagePartsC = c;
    }

    /// <summary>True when any bank is missing or still holds only placeholder stubs.</summary>
    public bool IsUnpopulated()
    {
        return IsPlaceholder(startingLines) || IsPlaceholder(messagePartsA)
            || IsPlaceholder(messagePartsB) || IsPlaceholder(messagePartsC);
    }

    public string GetStartingLine()
    {
        return Pick(startingLines, "...", NewRng(0));
    }

    /// <summary>
    /// Souls-style hint: "{A} {B} {C}" then a blank line then the interpreted clue.
    /// Each part is self-contained, so any A/B/C combination is coherent; empty parts and
    /// a null clue are tolerated.
    /// </summary>
    public string BuildMessageFromClue(Clue clue)
    {
        System.Random rng = NewRng(1);

        List<string> parts = new List<string>(3);
        AddIfPresent(parts, Pick(messagePartsA, null, rng));
        AddIfPresent(parts, Pick(messagePartsB, null, rng));
        AddIfPresent(parts, Pick(messagePartsC, null, rng));

        string wrapper = string.Join(" ", parts);
        string clueText = ClueInterpreter.Interpret(clue);

        return string.IsNullOrWhiteSpace(wrapper) ? clueText : $"{wrapper}\n\n{clueText}";
    }

    // A fresh RNG each call so a non-rerolling NPC always produces the SAME phrasing.
    // Seed is per-instance; salt keeps greeting and hint selections independent.
    private System.Random NewRng(int salt)
    {
        return rerollEachInteraction
            ? new System.Random()
            : new System.Random(unchecked(PhrasingSeed * 397 ^ salt));
    }

    private static string Pick(string[] pool, string fallback, System.Random rng)
    {
        if (pool == null || pool.Length == 0)
        {
            return fallback;
        }

        return pool[rng.Next(pool.Length)];
    }

    private static void AddIfPresent(List<string> list, string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            list.Add(value.Trim());
        }
    }

    private static bool IsPlaceholder(string[] pool)
    {
        if (pool == null || pool.Length == 0)
        {
            return true;
        }

        foreach (string s in pool)
        {
            if (!string.IsNullOrEmpty(s) && s.Trim().Length > 2)
            {
                return false;
            }
        }

        return true;
    }
}
