using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Checks a <see cref="PlayerPaper"/> against the session's <see cref="PaperReq"/> rules and
/// reports which of the six requirements are unmet. Pure logic - no scene dependencies.
/// Reuses each rule's own <c>enforceRule</c>; nothing in the rule system is modified.
/// </summary>
public static class PaperEvaluator
{
    /// <summary>Human-readable lines for every requirement the paper fails. Empty = full pass.</summary>
    public static List<string> FindUnmet(PlayerPaper paper, PaperReq req)
    {
        List<string> unmet = new List<string>();

        if (paper == null || req == null)
        {
            unmet.Add("There is no paper to inspect.");
            return unmet;
        }

        string name = paper.name ?? string.Empty;
        string origin = paper.origin ?? string.Empty;
        string sex = paper.sex ?? string.Empty;

        if (!req.NameRule.enforceRule(name))
            unmet.Add($"Name \"{name}\" is not in order.");

        if (!req.OriginRule.enforceRule(origin))
            unmet.Add($"Origin \"{origin}\" is not in order.");

        if (!req.SexRule.enforceRule(sex))
            unmet.Add($"Sex \"{sex}\" is not permitted.");

        if (!req.AgeRule.enforceRule(paper.age))
            unmet.Add($"Age {Mathf.RoundToInt(paper.age)} does not meet regulations.");

        if (!req.HeightRule.enforceRule(paper.height))
            unmet.Add($"Height {Mathf.RoundToInt(paper.height)} does not meet regulations.");

        if (!req.IDRule.enforceRule(Mathf.RoundToInt(paper.id)))
            unmet.Add($"ID {Mathf.RoundToInt(paper.id):D4} is not valid.");

        return unmet;
    }

    public static bool Passes(PlayerPaper paper, PaperReq req)
    {
        return FindUnmet(paper, req).Count == 0;
    }
}
