using System;
using UnityEngine;

public class AgeRule : Rule<float>
{
    public Tuple<int,int> ageRange;
    public float[] specificExcludedAges;

    public AgeRule(Tuple<int,int> ageRange, float[] specificExcludedAges)
    {
        this.ageRange = ageRange;
        this.specificExcludedAges = specificExcludedAges;
    }

    public override bool enforceRule(float playerAge)
    {
        //Check range
        if (playerAge < ageRange.Item1 || playerAge > ageRange.Item2)
            return false;

        //Check excluded ages
        foreach (float excluded in specificExcludedAges)
            if (Mathf.Approximately(playerAge, excluded))
                return false;

        return true;
    }

}