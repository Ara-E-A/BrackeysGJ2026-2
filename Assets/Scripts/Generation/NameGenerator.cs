using UnityEngine;

public class NameGenerator
{
    public Rule<string>[] nameRules;
    public Rule<string>[] originRules;
    public Rule<string>[] sexRules;
    public Rule<int>[] idRules;
    public Rule<float>[] heightRules;

    public static string[] names;

    public static string[] GetNames(){
        names = namesList();
        return names;
    }

    private static string[] namesList()
    {
        string[] nameStarters = {
            "Ar", "Bri", "Cok", "Dra", "Esh", "Fen", "Gri", "Hav", "Iri", "Jor", "Karn",
            "Lut", "Mek", "Nox", "Orr", "Pax", "Quar", "Riv", "Saar", "Tarn", "Urr",
            "Vek", "Wren", "Xor", "Yar", "Zek", "Eld", "Mire", "Lorn", "Pell", "Ash", "Syr",
            "Tae", "Vyn", "Zor"
        };
        string[] nameMiddles = {
            "a", "ae", "ba", "be", "ci", "de", "ea", "en", "fa", "fi", "ga", "he",
            "ik", "in", "je", "ka", "ki", "lo", "lu", "me", "mu", "na", "o", "pi",
            "qu", "ra", "rai", "rue", "sa", "so", "ti", "uoi", "va", "vei", "via",
            "we", "wo", "xi", "xo", "ya", "yo", "za", "ze", "dr", "gr", "sk", "th"
        };
        string[] nameEndings = {
            "", "a", "ax", "berg", "cair", "dun", "e", "fen", "gash", "hyn", "ite",
            "ka", "kin", "lorn", "mire", "na", "nex", "or", "qin", "rath", "rion", "ron",
            "sen", "son", "steel", "ton", "van", "ven", "wren", "xel", "yne", "za", "zin"
        };

        string[] fullNames = new string[25];
        for (int i = 0; i < 25; i++)
        {
            fullNames[i] = assembleName(nameStarters, nameMiddles, nameEndings);
            Debug.Log(fullNames[i]);
        }

        return fullNames;
    }

    private static string assembleName(string[] nameStarters, string[] nameMiddles, string[] nameEndings)
    {
        // Randomly select a starter and ending from the respective arrays
        string starter = nameStarters[Random.Range(0, nameStarters.Length)];
        string ending = nameEndings[Random.Range(0, nameEndings.Length)];
        int middleCount = Random.Range(1, 2);
        string middle = "";

        for (int index = 0; index < middleCount; index++)
        {
            middle += nameMiddles[Random.Range(0, nameMiddles.Length)];
        }

        // Concatenate the selected parts to form a full name
        string fullName = starter + middle + ending;
        return fullName;
    }
}