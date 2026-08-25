/// <summary>
/// A Clue is a small piece of structured information about one of the game's rules.
/// 
/// IMPORTANT:
/// - A Clue contains NO text.
/// - It only stores raw data (payload) describing something true, false, half‑true,
///   or misleading about a rule.
/// - Different objects in the world (NPCs, signs, notes, posters) will later decide
///   how to turn this raw data into actual displayed text.
/// 
/// Fields:
/// - ruleType: Which rule this clue refers to (Age, Height, Name, Origin, Sex, ID).
/// - truthType: Whether the clue is True, False, HalfTrue, or Misleading.
/// - payload: The raw structured data needed to generate text later.
/// 
/// </summary>

public enum ClueTruth
{
    True,
    False,
    HalfTrue,
    Misleading
}

public enum RuleType
{
    Age,
    Height,
    Origin,
    Name,
    Sex,
    ID
}

[System.Serializable]
public class Clue
{
    public RuleType ruleType;
    public ClueTruth truthType;

    public object payload;

    public Clue(RuleType ruleType, ClueTruth truthType, object payload)
    {
        this.ruleType = ruleType;
        this.truthType = truthType;
        this.payload = payload;
    }
}