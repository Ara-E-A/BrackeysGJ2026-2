using System.Collections.Generic;

public static class NPCClueInterpreter
{
    public static string Interpret(Clue clue)
    {
        switch (clue.ruleType)
        {
            case RuleType.Age: return InterpretAge(clue);
            case RuleType.Height: return InterpretHeight(clue);
            case RuleType.Name: return InterpretName(clue);
            case RuleType.Origin: return InterpretOrigin(clue);
            case RuleType.Sex: return InterpretSex(clue);
            case RuleType.ID: return InterpretID(clue);
            default: return "I’m not sure what that means…";
        }
    }

    private static string InterpretAge(Clue clue)
    {
        var p = clue.payload as IDictionary<string, object>;

        if (clue.truthType == ClueTruth.True)
            return $"I heard the allowed age is between {p["min"]} and {p["max"]}.";

        if (clue.truthType == ClueTruth.False)
            return $"Someone told me certain ages are forbidden…";

        if (clue.truthType == ClueTruth.HalfTrue)
            return $"I think age {p["excluded"]} might be important.";

        return "Age doesn’t matter… or so they say.";
    }

    private static string InterpretHeight(Clue clue)
    {
        var p = clue.payload as IDictionary<string, object>;

        if (clue.truthType == ClueTruth.True)
            return $"They say height must be between {p["minHeight"]} and {p["maxHeight"]}.";

        return "Height rules are strange around here.";
    }

    private static string InterpretName(Clue clue)
    {
        var p = clue.payload as IDictionary<string, object>;

        if (clue.truthType == ClueTruth.True)
            return $"Avoid names like {string.Join(", ", (string[])p["forbidden"])}.";

        return "Names carry meaning… or danger.";
    }

    private static string InterpretOrigin(Clue clue)
    {
        var p = clue.payload as IDictionary<string, object>;

        if (clue.truthType == ClueTruth.True)
            return $"Origins must be between {p["minLen"]} and {p["maxLen"]} letters.";

        return "Origins… they’re complicated.";
    }

    private static string InterpretSex(Clue clue)
    {
        var p = clue.payload as IDictionary<string, object>;

        if (clue.truthType == ClueTruth.True)
            return $"Only certain sexes are allowed: {string.Join(", ", (string[])p["allowed"])}.";

        return "Sex rules? I don’t get them.";
    }

    private static string InterpretID(Clue clue)
    {
        var p = clue.payload as IDictionary<string, object>;

        if (clue.truthType == ClueTruth.True)
            return $"ID must be divisible by {p["divisor"]} and contain {p["requiredDigit"]}.";

        return "ID numbers hide secrets.";
    }
}
