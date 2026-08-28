/// <summary>
/// The full library of Souls-style dialogue fragments for border-checkpoint NPCs.
///
/// COMBINING CONTRACT (see NPCDialogue.BuildMessageFromClue):
///   message = "{A} {B} {C}\n\n{clueText}"
///
///   A - Opening beat.  A complete sentence that sets mood / addresses the traveller.
///   B - Testimony beat. A complete sentence in which the speaker claims (and hedges)
///       knowledge of the border rules. This is what motivates the clue.
///   C - Closing kicker. A short sign-off sentence (or "...") spoken just before the hint.
///
/// Every entry in every bank is INDEPENDENTLY grammatical and tone-matched, so any
/// A x B x C combination reads coherently with no cross-fragment grammar. The clue text
/// itself is produced separately by ClueInterpreter and appended after a blank line;
/// fragments never need to agree with its wording.
///
/// Tone: archaic, weary, cryptic, occasionally dark-funny. Recurring imagery: the queue,
/// the gate, stamps and ink, hollowed clerks, inspectors, the wall, being turned away.
/// </summary>
public static class SoulsDialogueBank
{
    /// <summary>Shown on its own as the greeting (before Continue / Leave). No clue attached.</summary>
    public static readonly string[] StartingLines =
    {
        "Ahh... you still have your face. Hold onto it. They collect them here.",
        "Another traveller for the gate. The queue neither shortens nor forgives.",
        "Stay a while. No one passes quickly through this hall.",
        "You carry papers. Brave. Or foolish. The two wear the same coat.",
        "I have stood in this line so long the stamps have begun to mean things.",
        "Pay the clerk no mind. He stopped blinking some days ago.",
        "You have the look of one who reads the small print. Rare, at this gate.",
        "Welcome to the checkpoint. Leave your hope in the tray with the rest of your effects.",
        "They turned my brother back over a smudge. A smudge. Mind your ink.",
        "The inspector rewrites the rules by the hour. Or my memory unwrites them. Hard to say.",
        "Sit, stand, wear a groove in the flagstones - the gate minds none of it.",
        "You again? No... every face turns the same grey after long enough in here.",
        "Papers, stamps, and patience. Bring all three, or turn back now.",
        "The wall stood here long before the law. The law is merely louder.",
        "Speak softly. Even the hollow clerks lean toward a whisper.",
        "Rest your legs. 'Next' is a season in this hall, not a moment.",
    };

    /// <summary>Opening beat - sets the scene. Complete sentence.</summary>
    public static readonly string[] PartsA =
    {
        "Ahh, another soul bound for the gate.",
        "You reek of the long road, friend.",
        "The queue has not shifted since the lanterns were lit.",
        "I have watched a hundred travellers stamped and sorted since dawn.",
        "Keep your voice beneath the draught; the clerks hear well, even the hollow ones.",
        "There is time to talk here. There is only ever time to talk.",
        "You look like one still counting on the rules to hold their shape.",
        "The gate took my name at the threshold and handed me a number.",
        "Cold hall, colder inspectors. Such is the checkpoint.",
        "Mind the man ahead of you - he has been 'next' for two days.",
        "I stood where you stand once, papers in hand, certain of things.",
        "They say the border crept south last winter. The queue did not follow.",
        "Every stamp in this place costs something. Rarely ink.",
        "You will want an ally in this line. I am the nearest one on offer.",
        "Lean close. The wind carries loose words straight to the guardroom.",
        "The lamp-oil burns low and still the hall will not empty.",
    };

    /// <summary>Testimony beat - the speaker claims and hedges knowledge of the rules. Complete sentence.</summary>
    public static readonly string[] PartsB =
    {
        "A clerk told me the shape of the law, before the hollowing took his tongue.",
        "It is scratched into the wall beneath the window, half-legible, older than the guard.",
        "A man they turned away swore this to me as they dragged him off.",
        "My own papers burned at the last gate, but this much stayed with me.",
        "The night inspector recites it in her sleep; my cot sits beside hers.",
        "I gave up three days' bread for what I am about to tell you.",
        "One learns the rules by marking who gets the stamp and who gets the cane.",
        "The regulars in this hall mutter it the way other men mutter prayers.",
        "A child here knows the law better than the men enforcing it. She told me.",
        "I read it from the inspector's own ledger while he dozed at his post.",
        "The last honest clerk left a note in the belongings tray. This was on it.",
        "Word travels down the line, mouth to ear, and reaches me thinned but whole.",
        "I have been turned back often enough to have learned the pattern.",
        "An old border hand passed this to me before he finally got through.",
        "The rule was posted once, then torn down. Some of us keep it in memory.",
        "A guard let it slip over drink. He will not recall it; you should.",
    };

    /// <summary>Closing kicker - short sign-off spoken just before the hint.</summary>
    public static readonly string[] PartsC =
    {
        "Believe it or not; the gate cares for neither.",
        "Make of it what you will.",
        "Heed it, or join the heap of the turned-away.",
        "It has held true this week. Next week is another country.",
        "That is all I carry. Spend it well.",
        "Do not say you had it from me.",
        "Whether it saves you or damns you, I will not be here to learn.",
        "Take it and move along before the clerk lifts his head.",
        "Truth keeps poorly in this hall, so be quick with it.",
        "I have said more than is wise already.",
        "...",
        "Such is the way of things at the gate.",
        "The rest you must walk into yourself.",
        "Nod as though I told you nothing.",
    };
}
