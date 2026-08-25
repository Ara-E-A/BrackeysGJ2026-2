using System;

class AgeRule : Rule<float>
{
    // Age Rules are rules concerning the player's age, things like
    // minimum or maximum age requirements.
    float playerAge;
    float[] specificExcludedAges;
    Tuple<int, int> ageRange;

    public AgeRule(Tuple<int, int> ageRange, float[] specificExcludedAges)
    {
        this.ageRange = ageRange;
        this.specificExcludedAges = specificExcludedAges;
    }

    public override bool enforceRule(float playerAge)
    {
        if (this.playerAge < ageRange.Item1 || this.playerAge > ageRange.Item2)
        {
            // Player is outside the allowed age range
            // Handle the violation of the rule here
            return false;
        } else if (Array.IndexOf(specificExcludedAges, this.playerAge) >= 0)
        {
            // Player's age is specifically excluded
            // Handle the violation of the rule here
            return false;
        }
        return true;
    }

    public void checkAge(float age)
    {
        this.playerAge = age;
    }
}