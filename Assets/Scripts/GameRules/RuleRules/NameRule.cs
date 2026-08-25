using System;

public class NameRule : Rule<string>
{
    public string[] forbiddenNames;

    public NameRule(string[] forbiddenNames)
    {
        this.forbiddenNames = forbiddenNames;
    }

    public override bool enforceRule(string playerName)
    {
        if (forbiddenNames == null)
            return true;

        foreach (string forbidden in forbiddenNames)
        {
            if (playerName.Equals(forbidden, StringComparison.OrdinalIgnoreCase))
                return false;
        }

        return true;
    }

}