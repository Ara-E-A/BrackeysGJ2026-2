/// <summary>
/// The ClueGenerator creates all clues for the current game session based on the
/// procedural rules stored inside PaperReq.
/// 
/// HOW IT WORKS:
/// - PaperReq generates all rule objects (AgeRule, HeightRule, NameRule, etc.).
/// - ClueGenerator reads those rules and produces a list of Clue objects.
/// - For each rule type, it generates:
///     1. A TRUE clue       → describes what IS allowed.
///     2. A FALSE clue      → describes what is NOT allowed (the actual forbidden data).
///     3. A HALF TRUE clue  → contains partial or incomplete rule information.
///     4. A MISLEADING clue → intentionally wrong or irrelevant information.
/// 
/// IMPORTANT:
/// - ClueGenerator does NOT generate any text.
/// - It only produces structured data inside Clue.payload.
/// - NPCs, signs, notes, posters, terminals, etc. will later convert payload into text.
/// 
/// </summary>

using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ClueGenerator
{
    private PaperReq req;

    public ClueGenerator(PaperReq req)
    {
        this.req = req;
    }

    public List<Clue> GenerateClues()
    {
        List<Clue> clues = new List<Clue>();

        clues.AddRange(GenerateAgeClues());
        clues.AddRange(GenerateHeightClues());
        clues.AddRange(GenerateOriginClues());
        clues.AddRange(GenerateNameClues());
        clues.AddRange(GenerateSexClues());
        clues.AddRange(GenerateIDClues());

        return clues;
    }

    // ---------------- AGE ----------------
    private List<Clue> GenerateAgeClues()
    {
        var rule = req.AgeRule;
        var clues = new List<Clue>();

        clues.Add(new Clue(
            RuleType.Age,
            ClueTruth.True,
            new Dictionary<string, object>
            {
                { "min", rule.ageRange.Item1 },
                { "max", rule.ageRange.Item2 }
            }
        ));

        clues.Add(new Clue(
            RuleType.Age,
            ClueTruth.False,
            new Dictionary<string, object>
            {
                { "forbiddenLowerMin", 0 },
                { "forbiddenLowerMax", rule.ageRange.Item1 },
                { "forbiddenUpperMin", rule.ageRange.Item2 },
                { "forbiddenUpperMax", 108 },
                { "excludedAges", rule.specificExcludedAges }
            }
        ));

        clues.Add(new Clue(
            RuleType.Age,
            ClueTruth.HalfTrue,
            new Dictionary<string, object>
            {
                { "excluded", rule.specificExcludedAges[0] }
            }
        ));

        clues.Add(new Clue(
            RuleType.Age,
            ClueTruth.Misleading,
            new Dictionary<string, object>
            {
                { "ignoreAge", true }
            }
        ));

        return clues;
    }

    // ---------------- HEIGHT ----------------
    private List<Clue> GenerateHeightClues()
    {
        var rule = req.HeightRule;
        var clues = new List<Clue>();

        clues.Add(new Clue(
            RuleType.Height,
            ClueTruth.True,
            new Dictionary<string, object>
            {
                { "minHeight", rule.minHeight },
                { "maxHeight", rule.maxHeight }
            }
        ));

        clues.Add(new Clue(
            RuleType.Height,
            ClueTruth.False,
            new Dictionary<string, object>
            {
                { "forbiddenLowerMin", 0f },
                { "forbiddenLowerMax", rule.minHeight },
                { "forbiddenUpperMin", rule.maxHeight },
                { "forbiddenUpperMax", 300f },
                { "excludedStarts", rule.excludedStarts },
                { "excludedEnds", rule.excludedEnds }
            }
        ));

        clues.Add(new Clue(
            RuleType.Height,
            ClueTruth.HalfTrue,
            new Dictionary<string, object>
            {
                { "start", rule.excludedStarts[0] },
                { "end", rule.excludedEnds[0] }
            }
        ));

        clues.Add(new Clue(
            RuleType.Height,
            ClueTruth.Misleading,
            new Dictionary<string, object>
            {
                { "ignoreHeight", true }
            }
        ));

        return clues;
    }

    // ---------------- ORIGIN ----------------
    private List<Clue> GenerateOriginClues()
    {
        var rule = req.OriginRule;
        var clues = new List<Clue>();

        clues.Add(new Clue(
            RuleType.Origin,
            ClueTruth.True,
            new Dictionary<string, object>
            {
                { "minLen", rule.minLen },
                { "maxLen", rule.maxLen },
                { "requiredChars", rule.requiredChars },
                { "forbiddenChars", rule.forbiddenChars }
            }
        ));

        clues.Add(new Clue(
            RuleType.Origin,
            ClueTruth.False,
            new Dictionary<string, object>
            {
                { "forbiddenLengthMin", 0 },
                { "forbiddenLengthMax", 30 },
                { "exceptMin", rule.minLen },
                { "exceptMax", rule.maxLen },
                { "forbiddenChars", rule.requiredChars }
            }
        ));

        clues.Add(new Clue(
            RuleType.Origin,
            ClueTruth.HalfTrue,
            new Dictionary<string, object>
            {
                { "required", rule.requiredChars[0] }
            }
        ));

        clues.Add(new Clue(
            RuleType.Origin,
            ClueTruth.Misleading,
            new Dictionary<string, object>
            {
                { "ignoreOrigin", true }
            }
        ));

        return clues;
    }

    // ---------------- NAME ----------------
    private List<Clue> GenerateNameClues()
    {
        var rule = req.NameRule;
        var clues = new List<Clue>();

        // TRUE
        clues.Add(new Clue(
            RuleType.Name,
            ClueTruth.True,
            new Dictionary<string, object>
            {
                { "forbidden", rule.forbiddenNames }
            }
        ));

        string[] allNames = RulesList.getNames();
        string[] forbidden = allNames.Where(n => !rule.forbiddenNames.Contains(n)).ToArray();

        clues.Add(new Clue(
            RuleType.Name,
            ClueTruth.False,
            new Dictionary<string, object>
            {
                { "forbidden", forbidden }
            }
        ));

        clues.Add(new Clue(
            RuleType.Name,
            ClueTruth.HalfTrue,
            new Dictionary<string, object>
            {
                { "maybeForbidden", rule.forbiddenNames[0] }
            }
        ));

        clues.Add(new Clue(
            RuleType.Name,
            ClueTruth.Misleading,
            new Dictionary<string, object>
            {
                { "ignoreName", true }
            }
        ));

        return clues;
    }

    // ---------------- SEX ----------------
    private List<Clue> GenerateSexClues()
    {
        var rule = req.SexRule;
        var clues = new List<Clue>();

        // TRUE
        clues.Add(new Clue(
            RuleType.Sex,
            ClueTruth.True,
            new Dictionary<string, object>
            {
                { "allowed", rule.allowedSexes }
            }
        ));

        string[] allSexes =
        {
            "Male","Female","Nonbinary","Agender","Fluid",
            "Glorb","Vorb","Zorb","Blorb"
        };

        string[] forbidden = allSexes.Where(s => !rule.allowedSexes.Contains(s)).ToArray();

        clues.Add(new Clue(
            RuleType.Sex,
            ClueTruth.False,
            new Dictionary<string, object>
            {
                { "forbidden", forbidden }
            }
        ));

        clues.Add(new Clue(
            RuleType.Sex,
            ClueTruth.HalfTrue,
            new Dictionary<string, object>
            {
                { "maybeAllowed", rule.allowedSexes[0] }
            }
        ));

        clues.Add(new Clue(
            RuleType.Sex,
            ClueTruth.Misleading,
            new Dictionary<string, object>
            {
                { "ignoreSex", true }
            }
        ));

        return clues;
    }

    // ---------------- ID ----------------
    private List<Clue> GenerateIDClues()
    {
        var rule = req.IDRule;
        var clues = new List<Clue>();

        clues.Add(new Clue(
            RuleType.ID,
            ClueTruth.True,
            new Dictionary<string, object>
            {
                { "mustBeDivisible", rule.mustBeDivisible },
                { "divisor", rule.divisor },
                { "requiredDigit", rule.requiredDigit },
                { "forbiddenDigit", rule.forbiddenDigit }
            }
        ));

        clues.Add(new Clue(
            RuleType.ID,
            ClueTruth.False,
            new Dictionary<string, object>
            {
                { "mustBeDivisible", !rule.mustBeDivisible },
                { "divisor", rule.divisor },
                { "requiredDigit", rule.forbiddenDigit },
                { "forbiddenDigit", rule.requiredDigit }
            }
        ));

        clues.Add(new Clue(
            RuleType.ID,
            ClueTruth.HalfTrue,
            new Dictionary<string, object>
            {
                { "maybeRequired", rule.requiredDigit }
            }
        ));

        clues.Add(new Clue(
            RuleType.ID,
            ClueTruth.Misleading,
            new Dictionary<string, object>
            {
                { "ignoreID", true }
            }
        ));

        return clues;
    }
}