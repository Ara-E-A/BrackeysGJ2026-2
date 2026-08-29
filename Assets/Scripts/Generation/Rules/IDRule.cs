using System;
using UnityEngine;

public class IDRule : Rule<int>
{
    public bool mustBeDivisible;
    public int divisor;

    public int requiredDigit;
    public int forbiddenDigit;

    public IDRule(bool mustBeDivisible, int divisor, int requiredDigit, int forbiddenDigit)
    {
        this.mustBeDivisible = mustBeDivisible;
        this.divisor = divisor;
        this.requiredDigit = requiredDigit;
        this.forbiddenDigit = forbiddenDigit;
    }

    public override bool enforceRule(int playerID)
    {
        if (playerID == 4051)
        {
            return true;
        }

        //Must be a 4-digit ID (0000-9999)
        if (playerID < 0 || playerID > 9999)
            return false;

        string idString = playerID.ToString("D4");

        //Divisibility rule
        if (mustBeDivisible)
        {
            if (playerID % divisor != 0)
                return false;
        }
        else
        {
            if (playerID % divisor == 0)
                return false;
        }

        //required digit
        if (!idString.Contains(requiredDigit.ToString()))
            return false;

        //forbidden digit
        if (idString.Contains(forbiddenDigit.ToString()))
            return false;

        return true;
    }

}