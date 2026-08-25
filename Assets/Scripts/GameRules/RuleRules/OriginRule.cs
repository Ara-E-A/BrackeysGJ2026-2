public class OriginRule : Rule<string>
{
    public int minLen;
    public int maxLen;

    public char[] requiredChars;
    public char[] forbiddenChars;

    public OriginRule(int minLen, int maxLen, char[] requiredChars, char[] forbiddenChars)
    {
        this.minLen = minLen;
        this.maxLen = maxLen;
        this.requiredChars = requiredChars;
        this.forbiddenChars = forbiddenChars;
    }

    public override bool enforceRule(string origin)
    {
       if (origin.Length < minLen || origin.Length > maxLen)
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