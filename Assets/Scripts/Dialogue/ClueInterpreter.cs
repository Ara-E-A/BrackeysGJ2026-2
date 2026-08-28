using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Turns a data-only <see cref="Clue"/> (produced by <see cref="ClueGenerator"/> from the
/// procedural <see cref="PaperReq"/> rules) into a single line of NPC hearsay.
///
/// One branch per <see cref="RuleType"/> x <see cref="ClueTruth"/>. Voice by truth type:
///   True       - stated plainly and correctly.
///   False      - stated with the same confidence, but wrong ("someone insists...").
///   HalfTrue   - partial / hedged ("something about...").
///   Misleading - dismissive ("they don't even check that").
///
/// Payload keys are whatever <see cref="ClueGenerator"/> put in the dictionary; missing
/// keys and a null/absent payload degrade to a vague line instead of throwing.
/// </summary>
public static class ClueInterpreter
{
    public static string Interpret(Clue clue)
    {
        if (clue == null)
        {
            return "I'm not sure what that was about.";
        }

        switch (clue.ruleType)
        {
            case RuleType.Age: return InterpretAge(clue);
            case RuleType.Height: return InterpretHeight(clue);
            case RuleType.Name: return InterpretName(clue);
            case RuleType.Origin: return InterpretOrigin(clue);
            case RuleType.Sex: return InterpretSex(clue);
            case RuleType.ID: return InterpretID(clue);
            default: return "I'm not sure what that means.";
        }
    }

    // ---------------- AGE ----------------
    private static string InterpretAge(Clue clue)
    {
        var p = Payload(clue);
        if (p == null)
        {
            return "The age rule... I never did get it straight.";
        }

        switch (clue.truthType)
        {
            case ClueTruth.True:
                return $"I heard the gate only takes ages {Num(Get(p, "min"))} through {Num(Get(p, "max"))}.";
            case ClueTruth.False:
                return $"A clerk swears ages {List(Get(p, "excludedAges"))} are struck from the register outright.";
            case ClueTruth.HalfTrue:
                return $"Something about age {Num(Get(p, "excluded"))} sticks with me. Barred, or required? I forget.";
            case ClueTruth.Misleading:
                return "Age? They stopped checking that years ago. Don't waste your breath.";
            default:
                return "The age rule is a blur to me.";
        }
    }

    // ---------------- HEIGHT ----------------
    private static string InterpretHeight(Clue clue)
    {
        var p = Payload(clue);
        if (p == null)
        {
            return "Height rules are strange around here.";
        }

        switch (clue.truthType)
        {
            case ClueTruth.True:
                return $"They say height must sit between {Num(Get(p, "minHeight"))} and {Num(Get(p, "maxHeight"))} centimetres.";
            case ClueTruth.False:
                return $"A drunk at the front swears every height from {List(Get(p, "excludedStarts"))} is refused on sight.";
            case ClueTruth.HalfTrue:
                return $"There's a bad band around {Num(Get(p, "start"))} to {Num(Get(p, "end"))} centimetres, I think. Or near it.";
            case ClueTruth.Misleading:
                return "Height's not on their list. I've seen giants and dwarves both stamped through.";
            default:
                return "The height rule escapes me.";
        }
    }

    // ---------------- NAME ----------------
    private static string InterpretName(Clue clue)
    {
        var p = Payload(clue);
        if (p == null)
        {
            return "Names carry meaning here... or danger.";
        }

        switch (clue.truthType)
        {
            case ClueTruth.True:
                return $"Avoid the names {List(Get(p, "forbidden"))} - those get you turned away.";
            case ClueTruth.False:
                return $"A man swore the barred names are {List(Get(p, "forbidden"))}. Trust that list if you dare.";
            case ClueTruth.HalfTrue:
                return $"One of the forbidden names might be {Str(Get(p, "maybeForbidden"))}. Might.";
            case ClueTruth.Misleading:
                return "Names? Give any name. The register's a mess, nobody checks.";
            default:
                return "The name rule is hearsay to me.";
        }
    }

    // ---------------- ORIGIN ----------------
    private static string InterpretOrigin(Clue clue)
    {
        var p = Payload(clue);
        if (p == null)
        {
            return "Origins... they're complicated.";
        }

        switch (clue.truthType)
        {
            case ClueTruth.True:
                return $"An origin must run {Num(Get(p, "minLen"))} to {Num(Get(p, "maxLen"))} letters, " +
                       $"carry {List(Get(p, "requiredChars"))}, and never {List(Get(p, "forbiddenChars"))}.";
            case ClueTruth.False:
                return $"They told me any origin shorter than {Num(Get(p, "exceptMin"))} or longer than " +
                       $"{Num(Get(p, "exceptMax"))} letters is refused, and the letters {List(Get(p, "forbiddenChars"))} are cursed.";
            case ClueTruth.HalfTrue:
                return $"The origin needs the letter {Str(Get(p, "required"))} in it somewhere. That much I'm sure of.";
            case ClueTruth.Misleading:
                return "Where you're from? They don't care. Half the clerks can't read the maps anyway.";
            default:
                return "The origin rule is a muddle to me.";
        }
    }

    // ---------------- SEX ----------------
    private static string InterpretSex(Clue clue)
    {
        var p = Payload(clue);
        if (p == null)
        {
            return "Sex rules? I don't get them.";
        }

        switch (clue.truthType)
        {
            case ClueTruth.True:
                return $"Only these are let through: {List(Get(p, "allowed"))}.";
            case ClueTruth.False:
                return $"The word going round is that {List(Get(p, "forbidden"))} are all refused at the gate.";
            case ClueTruth.HalfTrue:
                return $"I think {Str(Get(p, "maybeAllowed"))} is on the allowed list. Don't quote me.";
            case ClueTruth.Misleading:
                return "That box on the form? Leave it blank. They never read it.";
            default:
                return "The sex rule is beyond me.";
        }
    }

    // ---------------- ID ----------------
    private static string InterpretID(Clue clue)
    {
        var p = Payload(clue);
        if (p == null)
        {
            return "ID numbers hide their secrets.";
        }

        bool divisible = Get(p, "mustBeDivisible") is bool b && b;
        string divClause = divisible
            ? $"divide cleanly by {Num(Get(p, "divisor"))}"
            : $"not divide by {Num(Get(p, "divisor"))}";

        switch (clue.truthType)
        {
            case ClueTruth.True:
                return $"The ID must {divClause}, show a {Num(Get(p, "requiredDigit"))}, and never a {Num(Get(p, "forbiddenDigit"))}.";
            case ClueTruth.False:
                return $"Someone insists the ID must {divClause}, must carry a {Num(Get(p, "requiredDigit"))}, " +
                       $"and must never carry a {Num(Get(p, "forbiddenDigit"))}.";
            case ClueTruth.HalfTrue:
                return $"There's a digit the ID has to carry - {Num(Get(p, "maybeRequired"))}, I believe.";
            case ClueTruth.Misleading:
                return "The number? Ten digits, any ten. The scanner's been dead for weeks.";
            default:
                return "The ID rule is a cipher to me.";
        }
    }

    // ---------------- helpers ----------------

    private static IDictionary<string, object> Payload(Clue clue)
    {
        return clue.payload as IDictionary<string, object>;
    }

    private static object Get(IDictionary<string, object> p, string key)
    {
        return p != null && p.TryGetValue(key, out object value) ? value : null;
    }

    private static string Num(object value)
    {
        switch (value)
        {
            case null: return "?";
            case float f: return Mathf.RoundToInt(f).ToString();
            case double d: return Mathf.RoundToInt((float)d).ToString();
            default: return value.ToString();
        }
    }

    private static string Str(object value)
    {
        return value == null ? "?" : value.ToString();
    }

    private static string List(object array)
    {
        switch (array)
        {
            case null:
                return "nothing";
            case string[] strings:
                return strings.Length == 0 ? "nothing" : string.Join(", ", strings);
            case char[] chars:
                return chars.Length == 0 ? "nothing" : string.Join(", ", chars);
            case int[] ints:
                return ints.Length == 0 ? "nothing" : string.Join(", ", ints);
            case float[] floats:
            {
                if (floats.Length == 0)
                {
                    return "nothing";
                }

                string[] parts = new string[floats.Length];
                for (int i = 0; i < floats.Length; i++)
                {
                    parts[i] = Mathf.RoundToInt(floats[i]).ToString();
                }

                return string.Join(", ", parts);
            }
            default:
                return array.ToString();
        }
    }
}
