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

        // TRUE: allowed range
        clues.Add(new Clue(
            RuleType.Age,
            ClueTruth.True,
            new { min = rule.ageRange.Item1, max = rule.ageRange.Item2 }
        ));

        // FALSE: forbidden ranges
        clues.Add(new Clue(
            RuleType.Age,
            ClueTruth.False,
            new {
                forbiddenLowerMin = 0,
                forbiddenLowerMax = rule.ageRange.Item1,
                forbiddenUpperMin = rule.ageRange.Item2,
                forbiddenUpperMax = 108,
                excludedAges = rule.specificExcludedAges
            }
        ));

        // HALF TRUE
        clues.Add(new Clue(
            RuleType.Age,
            ClueTruth.HalfTrue,
            new { excluded = rule.specificExcludedAges[0] }
        ));

        // MISLEADING
        clues.Add(new Clue(
            RuleType.Age,
            ClueTruth.Misleading,
            new { ignoreAge = true }
        ));

        return clues;
    }

    // ---------------- HEIGHT ----------------
    private List<Clue> GenerateHeightClues()
    {
        var rule = req.HeightRule;
        var clues = new List<Clue>();

        // TRUE
        clues.Add(new Clue(
            RuleType.Height,
            ClueTruth.True,
            new { rule.minHeight, rule.maxHeight }
        ));

        // FALSE: forbidden ranges + pockets
        clues.Add(new Clue(
            RuleType.Height,
            ClueTruth.False,
            new {
                forbiddenLowerMin = 0f,
                forbiddenLowerMax = rule.minHeight,
                forbiddenUpperMin = rule.maxHeight,
                forbiddenUpperMax = 300f,
                excludedStarts = rule.excludedStarts,
                excludedEnds = rule.excludedEnds
            }
        ));

        // HALF TRUE
        clues.Add(new Clue(
            RuleType.Height,
            ClueTruth.HalfTrue,
            new { start = rule.excludedStarts[0], end = rule.excludedEnds[0] }
        ));

        // MISLEADING
        clues.Add(new Clue(
            RuleType.Height,
            ClueTruth.Misleading,
            new { ignoreHeight = true }
        ));

        return clues;
    }

    // ---------------- ORIGIN ----------------
    private List<Clue> GenerateOriginClues()
    {
        var rule = req.OriginRule;
        var clues = new List<Clue>();

        // TRUE
        clues.Add(new Clue(
            RuleType.Origin,
            ClueTruth.True,
            new { rule.minLen, rule.maxLen, rule.requiredChars, rule.forbiddenChars }
        ));

        // FALSE: forbidden lengths + forbidden chars
        clues.Add(new Clue(
            RuleType.Origin,
            ClueTruth.False,
            new {
                forbiddenLengthMin = 0,
                forbiddenLengthMax = 30,
                exceptMin = rule.minLen,
                exceptMax = rule.maxLen,
                forbiddenChars = rule.requiredChars // opposite
            }
        ));

        // HALF TRUE
        clues.Add(new Clue(
            RuleType.Origin,
            ClueTruth.HalfTrue,
            new { required = rule.requiredChars[0] }
        ));

        // MISLEADING
        clues.Add(new Clue(
            RuleType.Origin,
            ClueTruth.Misleading,
            new { ignoreOrigin = true }
        ));

        return clues;
    }

    // ---------------- NAME ----------------
    private List<Clue> GenerateNameClues()
    {
        var rule = req.NameRule;
        var clues = new List<Clue>();

        // TRUE: real forbidden names
        clues.Add(new Clue(
            RuleType.Name,
            ClueTruth.True,
            new { forbidden = rule.forbiddenNames }
        ));

        // FALSE: all names NOT allowed
        string[] allNames = RulesList.getNames();
        string[] forbidden = allNames
            .Where(n => !rule.forbiddenNames.Contains(n))
            .ToArray();

        clues.Add(new Clue(
            RuleType.Name,
            ClueTruth.False,
            new { forbidden }
        ));

        // HALF TRUE
        clues.Add(new Clue(
            RuleType.Name,
            ClueTruth.HalfTrue,
            new { maybeForbidden = rule.forbiddenNames[0] }
        ));

        // MISLEADING
        clues.Add(new Clue(
            RuleType.Name,
            ClueTruth.Misleading,
            new { ignoreName = true }
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
            new { allowed = rule.allowedSexes }
        ));

        // FALSE: all sexes NOT allowed
        string[] allSexes =
        {
            "Male","Female","Nonbinary","Agender","Fluid",
            "Glorb","Vorb","Zorb","Blorb"
        };

        string[] forbidden = allSexes
            .Where(s => !rule.allowedSexes.Contains(s))
            .ToArray();

        clues.Add(new Clue(
            RuleType.Sex,
            ClueTruth.False,
            new { forbidden }
        ));

        // HALF TRUE
        clues.Add(new Clue(
            RuleType.Sex,
            ClueTruth.HalfTrue,
            new { maybeAllowed = rule.allowedSexes[0] }
        ));

        // MISLEADING
        clues.Add(new Clue(
            RuleType.Sex,
            ClueTruth.Misleading,
            new { ignoreSex = true }
        ));

        return clues;
    }

    // ---------------- ID ----------------
    private List<Clue> GenerateIDClues()
    {
        var rule = req.IDRule;
        var clues = new List<Clue>();

        // TRUE
        clues.Add(new Clue(
            RuleType.ID,
            ClueTruth.True,
            new { rule.mustBeDivisible, rule.divisor, rule.requiredDigit, rule.forbiddenDigit }
        ));

        // FALSE: limited opposite (not full list)
        clues.Add(new Clue(
            RuleType.ID,
            ClueTruth.False,
            new {
                mustBeDivisible = !rule.mustBeDivisible,
                divisor = rule.divisor,
                requiredDigit = rule.forbiddenDigit,
                forbiddenDigit = rule.requiredDigit
            }
        ));

        // HALF TRUE
        clues.Add(new Clue(
            RuleType.ID,
            ClueTruth.HalfTrue,
            new { maybeRequired = rule.requiredDigit }
        ));

        // MISLEADING
        clues.Add(new Clue(
            RuleType.ID,
            ClueTruth.Misleading,
            new { ignoreID = true }
        ));

        return clues;
    }

}