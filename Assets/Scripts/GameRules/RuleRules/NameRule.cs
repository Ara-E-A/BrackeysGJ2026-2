using System;

class NameRule : Rule
{
    // Name Rules are rules concerning the player name, things like
    // no people starting with an "L" or names longer than 12 characters.

    private string[] forbiddenNames;
    private string playerName;

    public NameRule(string[] forbiddenNames)
    {
        this.forbiddenNames = forbiddenNames;
    }

    public override bool enforceRule()
    {
        if (forbiddenNames != null && Array.IndexOf(forbiddenNames, this.playerName) >= 0)
        {
            // Player's name is specifically excluded
            // Handle the violation of the rule here
            return false;
        }
        return true;
    }

    public void checkName(string playerName)
    {
        this.playerName = playerName;
    }

}