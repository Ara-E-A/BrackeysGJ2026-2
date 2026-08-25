class AgeRule : Rule
{
    // Age Rules are rules concerning the player's age, things like
    // minimum or maximum age requirements.
    float playerAge;

    public AgeRule()
    {
        Tuple<int, int> ageRange;
        float[] specificExcludedAges;
    }

    public override void enforceRule()
    {
        if (playerAge < ageRange.Item1 || playerAge > ageRange.Item2)
        {
            // Player is outside the allowed age range
            // Handle the violation of the rule here
        } else if (specificExcludedAges.Contains(playerAge))
        {
            // Player's age is specifically excluded
            // Handle the violation of the rule here
        }
    }

    public checkAge(float age)
    {
        this.playerAge = age;
    }
}