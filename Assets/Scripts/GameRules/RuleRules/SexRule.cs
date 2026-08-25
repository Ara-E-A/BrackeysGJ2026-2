class SexRule : Rule<string>
{
    // Sex Rules are rules concerning the player's sex, things like
    // requirements based on the stated sex.

    //TODO: come up with a list of rules this can choose from.

    public SexRule()
    {

    }

    public override bool enforceRule(string playerSex)
    {
        // Implementation for enforcing sex rule
        return true;
    }
}