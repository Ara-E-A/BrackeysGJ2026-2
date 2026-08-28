using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// The generated border-control rule set. Every <c>makeXRule</c> generator now runs an
/// internal consistency pass so a rule can never contradict itself:
///   - Origin: required and forbidden letters are disjoint and fit inside maxLen.
///   - Height: excluded pockets can never swallow the whole allowed band.
///   - Age: the allowed band always leaves at least one un-excluded whole year.
///   - Sex: the allowed list is de-duplicated (SexRule has no forbid list, so no clash).
///   - ID: the divisibility / required-digit / forbidden-digit clauses are always jointly
///     satisfiable by some 0000-9999 value (brute-checked).
/// Field shapes are unchanged, so ClueGenerator / PaperEvaluator keep working as-is.
/// </summary>
public class PaperReq : Paper
{

    public AgeRule AgeRule { get; private set; }
    public HeightRule HeightRule { get; private set; }
    public OriginRule OriginRule { get; private set; }
    public NameRule NameRule { get; private set; }
    public SexRule SexRule { get; private set; }
    public IDRule IDRule { get; private set; }

    public PaperReq()
    {
        makeAgeRule();
        makeHeightRule();
        makeOriginRule();
        makeNameRule();
        makeSexRule();
        makeIDRule();

        VerifyNoContradictions();
    }

    // ---------------- AGE ----------------
    private void makeAgeRule()
    {
        int min = UnityEngine.Random.Range(0, 54);
        int max = UnityEngine.Random.Range(54, 108);

        // Guarantee a usable band (>= 10 whole years) so point exclusions can never block it.
        if (max - min < 10)
        {
            max = Mathf.Min(108, min + 10);
        }

        // Excluded ages: distinct whole years strictly inside (min, max), and never enough
        // of them to block every integer age in the band.
        int freeYears = Mathf.Max(0, max - min - 1);
        int wanted = Mathf.Min(3, Mathf.Max(0, freeYears - 1));

        List<float> excluded = new List<float>();
        int guard = 0;
        while (excluded.Count < wanted && guard++ < 200)
        {
            float year = UnityEngine.Random.Range(min + 1, max); // (min, max) exclusive
            if (!excluded.Contains(year))
            {
                excluded.Add(year);
            }
        }

        // Keep the array non-empty: ClueGenerator reads specificExcludedAges[0].
        if (excluded.Count == 0)
        {
            excluded.Add(Mathf.Clamp((min + max) / 2, min, max));
        }

        AgeRule = new AgeRule(new Tuple<int, int>(min, max), excluded.ToArray());
    }

    // ---------------- HEIGHT ----------------
    private void makeHeightRule()
    {
        float minHeight = UnityEngine.Random.Range(120f, 150f);
        float maxHeight = UnityEngine.Random.Range(160f, 200f);
        if (maxHeight - minHeight < 12f)
        {
            maxHeight = minHeight + 12f;
        }

        int count = UnityEngine.Random.Range(1, 4);
        List<float> starts = new List<float>(count);
        List<float> ends = new List<float>(count);

        for (int i = 0; i < count; i++)
        {
            float width = UnityEngine.Random.Range(5f, 10f);
            float start = UnityEngine.Random.Range(minHeight, Mathf.Max(minHeight, maxHeight - width));
            starts.Add(Mathf.Clamp(start, minHeight, maxHeight));
            ends.Add(Mathf.Clamp(start + width, minHeight, maxHeight));
        }

        // Drop pockets from the end until a >= 2 cm continuous gap survives in [min, max].
        const float minGap = 2f;
        while (starts.Count > 1 && !HasHeightGap(minHeight, maxHeight, starts, ends, minGap))
        {
            starts.RemoveAt(starts.Count - 1);
            ends.RemoveAt(ends.Count - 1);
        }

        // A lone pocket still covering the whole band -> park it in the middle third.
        if (!HasHeightGap(minHeight, maxHeight, starts, ends, minGap))
        {
            float third = (maxHeight - minHeight) / 3f;
            starts[0] = minHeight + third;
            ends[0] = maxHeight - third;
        }

        HeightRule = new HeightRule(minHeight, maxHeight, starts.ToArray(), ends.ToArray());
    }

    // True when at least one continuous >= minGap stretch of [lo, hi] is outside every pocket.
    private static bool HasHeightGap(float lo, float hi, List<float> starts, List<float> ends, float minGap)
    {
        List<Vector2> pockets = new List<Vector2>(starts.Count);
        for (int i = 0; i < starts.Count; i++)
        {
            pockets.Add(new Vector2(Mathf.Min(starts[i], ends[i]), Mathf.Max(starts[i], ends[i])));
        }
        pockets.Sort((a, b) => a.x.CompareTo(b.x));

        float cursor = lo;
        foreach (Vector2 pocket in pockets)
        {
            if (pocket.x - cursor >= minGap)
            {
                return true;
            }
            cursor = Mathf.Max(cursor, pocket.y);
        }
        return hi - cursor >= minGap;
    }

    // ---------------- NAME ----------------
    private void makeNameRule()
    {
        string[] pool = NameGenerator.GetNames();

        List<string> forbidden = new List<string>();
        int guard = 0;
        while (forbidden.Count < 3 && guard++ < 500)
        {
            string candidate = pool[UnityEngine.Random.Range(0, pool.Length)];

            // Distinct, and never let the forbidden list swallow every name in the pool.
            if (!forbidden.Contains(candidate) && pool.Any(n => n != candidate && !forbidden.Contains(n)))
            {
                forbidden.Add(candidate);
            }
        }

        if (forbidden.Count == 0)
        {
            forbidden.Add(pool[UnityEngine.Random.Range(0, pool.Length)]);
        }

        NameRule = new NameRule(forbidden.ToArray());
    }

    // ---------------- ORIGIN ----------------
    private void makeOriginRule()
    {
        int minLen = UnityEngine.Random.Range(3, 6);
        int maxLen = UnityEngine.Random.Range(6, 12);
        if (minLen > maxLen)
        {
            (minLen, maxLen) = (maxLen, minLen);
        }

        // Distinct required letters, never more than can fit in the longest allowed origin.
        HashSet<char> required = new HashSet<char>();
        int requiredWanted = Mathf.Min(UnityEngine.Random.Range(1, 4), maxLen);
        int guard = 0;
        while (required.Count < requiredWanted && guard++ < 200)
        {
            required.Add((char)UnityEngine.Random.Range('A', 'Z' + 1));
        }

        // Distinct forbidden letters that never overlap the required set.
        HashSet<char> forbidden = new HashSet<char>();
        int forbiddenWanted = UnityEngine.Random.Range(1, 4);
        guard = 0;
        while (forbidden.Count < forbiddenWanted && guard++ < 400)
        {
            char c = (char)UnityEngine.Random.Range('A', 'Z' + 1);
            if (!required.Contains(c))
            {
                forbidden.Add(c);
            }
        }

        // 26 letters minus <=3 required always leaves room, but keep it non-empty defensively.
        if (forbidden.Count == 0)
        {
            for (char c = 'A'; c <= 'Z' && forbidden.Count == 0; c++)
            {
                if (!required.Contains(c))
                {
                    forbidden.Add(c);
                }
            }
        }

        OriginRule = new OriginRule(minLen, maxLen, required.ToArray(), forbidden.ToArray());
    }

    // ---------------- SEX ----------------
    private void makeSexRule()
    {
        // Draw from the same canonical list PlayerPaper / the Papers UI use.
        string[] pool = PlayerPaper.AllSexes;
        int wanted = Mathf.Clamp(UnityEngine.Random.Range(2, 5), 1, pool.Length);

        List<string> allowed = new List<string>();
        int guard = 0;
        while (allowed.Count < wanted && guard++ < 200)
        {
            string s = pool[UnityEngine.Random.Range(0, pool.Length)];
            if (!allowed.Contains(s)) // de-duplicated: the same value can't be listed twice
            {
                allowed.Add(s);
            }
        }

        if (allowed.Count == 0)
        {
            allowed.Add(pool[UnityEngine.Random.Range(0, pool.Length)]);
        }

        SexRule = new SexRule(allowed.ToArray());
    }

    // ---------------- ID ----------------
    private void makeIDRule()
    {
        bool mustBeDivisible = UnityEngine.Random.Range(0, 2) == 0;
        int divisor = UnityEngine.Random.Range(2, 11);
        int requiredDigit = UnityEngine.Random.Range(0, 10);

        int forbiddenDigit = UnityEngine.Random.Range(0, 10);
        while (forbiddenDigit == requiredDigit)
        {
            forbiddenDigit = UnityEngine.Random.Range(0, 10);
        }

        // Guarantee the clauses are jointly satisfiable. The classic trap: "divisible by 10"
        // forces a trailing 0, so 0 can't also be the forbidden digit.
        int guard = 0;
        while (!IdSatisfiable(mustBeDivisible, divisor, requiredDigit, forbiddenDigit) && guard++ < 10)
        {
            forbiddenDigit = (forbiddenDigit + 1) % 10;
            if (forbiddenDigit == requiredDigit)
            {
                forbiddenDigit = (forbiddenDigit + 1) % 10;
            }
        }

        // Last resort: a non-divisible rule with one required + one forbidden digit always
        // has a solution in 0000-9999.
        if (!IdSatisfiable(mustBeDivisible, divisor, requiredDigit, forbiddenDigit))
        {
            mustBeDivisible = false;
        }

        IDRule = new IDRule(mustBeDivisible, divisor, requiredDigit, forbiddenDigit);
    }

    private static bool IdSatisfiable(bool mustBeDivisible, int divisor, int requiredDigit, int forbiddenDigit)
    {
        char required = (char)('0' + requiredDigit);
        char forbidden = (char)('0' + forbiddenDigit);

        for (int id = 0; id <= 9999; id++)
        {
            bool divisibleOk = mustBeDivisible ? (id % divisor == 0) : (id % divisor != 0);
            if (!divisibleOk)
            {
                continue;
            }

            string s = id.ToString("D4");
            if (s.IndexOf(required) >= 0 && s.IndexOf(forbidden) < 0)
            {
                return true;
            }
        }

        return false;
    }

    // ---------------- backstop ----------------
    private void VerifyNoContradictions()
    {
        bool ageOk = false;
        for (int a = AgeRule.ageRange.Item1; a <= AgeRule.ageRange.Item2 && !ageOk; a++)
        {
            ageOk = AgeRule.enforceRule(a);
        }

        bool heightOk = false;
        for (float h = HeightRule.minHeight; h <= HeightRule.maxHeight && !heightOk; h += 0.5f)
        {
            heightOk = HeightRule.enforceRule(h);
        }

        bool originOk = OriginRule.minLen <= OriginRule.maxLen
                        && OriginRule.requiredChars.Length <= OriginRule.maxLen
                        && !OriginRule.requiredChars.Intersect(OriginRule.forbiddenChars).Any();

        bool idOk = IdSatisfiable(IDRule.mustBeDivisible, IDRule.divisor, IDRule.requiredDigit, IDRule.forbiddenDigit);

        // Sex / Name draw from large pools and only ever list a handful, so they are always
        // satisfiable once de-duplicated.

        if (!ageOk || !heightOk || !originOk || !idOk)
        {
            Debug.LogError($"PaperReq: unresolved contradiction (age:{ageOk} height:{heightOk} origin:{originOk} id:{idOk}).");
        }
    }
}
