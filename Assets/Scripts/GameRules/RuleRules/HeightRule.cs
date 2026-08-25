class HeightRule : Rule<float>
{
    // Height Rules are rules concerning the player's height, things like
    // minimum or maximum height requirements.

    //TODO: come up with a list of rules this can choose from.

    public HeightRule()
    {

    }

    public override bool enforceRule(float playerHeight)
    {
        // Implementation for enforcing height rule
        return true;
    }
}