using UnityEngine;

public class RulesList
{
    public Rule[] nameRules;
    public Rule[] originRules;
    public Rule[] sexRules;
    public Rule[] idRules;
    public Rule[] heightRules;

    public static string[] names;

    public static string[] getNames(){
        names = namesList();
        return names;
    }

    private static string[] namesList()
    {
        string[] nameStarters = {"Ar", "Jo", "Mi", "Ka", "Lu", "Ra", "Sai", "Tar", "Vam", "Zae", "Mir", "Lun", "Eri", "Oli", "Nai", "Syr", "Tae", "Vyn", "Zor", "Pa", "Pel", "Ann", "El"};
        string[] nameMiddles = {"a", "en", "ij", "wo", "mu", "ae", "rai", "ea", "vei", "via", "ie", "soa", "uoi", "lua", "rue"};
        string[] nameEndings = {"", "", "", "", "ria", "rion", "sen", "van", "ton", "von", "rin", "ron", "magnesiumpermanganate", "ka", "kin", "ven", "ite", "bert", "son", "man", "smith"};

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
        int middleCount = Random.Range(1, 4);
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