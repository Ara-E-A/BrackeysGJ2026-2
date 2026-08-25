using System;
using UnityEngine;

public class SexRule : Rule<string>
{
    private string[] allowedSexes;

    public SexRule(string[] allowedSexes)
    {
        this.allowedSexes = allowedSexes;
    }

    public override bool enforceRule(string playerSex)
    {
        foreach (string s in allowedSexes)
        {
            if (playerSex.Equals(s, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

}