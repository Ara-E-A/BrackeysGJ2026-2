class OriginRule : Rule
{
    // Origin Rules are rules concerning the player's origin, things like
    // no people from a certain country or region.

    private int minLength;
    private int maxLength;

    private char[] requiredChars;
    private char[] forbiddenChars;
    
    public OriginRule(int minLength, int maxLength, char[] requiredChars, char[] forbiddenChars)
    {
        this.minLength = minLength;
        this.maxLength = maxLength;
        this.requiredChars = requiredChars;
        this.forbiddenChars = forbiddenChars;
    }

    public override bool enforceRule(string origin)
    {
       if (origin.Length < minLength || origin.Length > maxLength)
            return false;

        foreach (char c in requiredChars)
            if (!origin.Contains(c))
                return false;

        foreach (char c in forbiddenChars)
            if (origin.Contains(c))
                return false;

        return true;
    }

    // public override string ToString()
    // {
    //     string req = new string(requiredChars);
    //     string forb = new string(forbiddenChars);

    //     return $"OriginRule: length {minLength}-{maxLength}, " + $"requires [{req}], forbids [{forb}]";
    // }

}