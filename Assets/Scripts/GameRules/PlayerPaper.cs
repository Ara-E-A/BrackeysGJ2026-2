using UnityEngine;

public class PlayerPaper : Paper
{
    public string name;
    public string origin;
    public string sex;
    public float id;
    public float height;
    public float age;

    //TODO: Find a way to turn this info into a clean string to put on the PaperHUD.
    public PlayerPaper()
    {
        GenerateRandomPaper();
    }

    private void GenerateRandomPaper()
    {
        string[] names = RulesList.getNames();
        name = names[UnityEngine.Random.Range(0, names.Length)];

        origin = GenerateRandomOrigin();

        string[] allSexes =
        {
            "Male", "Female", "Nonbinary", "Agender", "Fluid",
            "Glorb", "Vorb", "Zorb", "Blorb"
        };
        sex = allSexes[UnityEngine.Random.Range(0, allSexes.Length)];

        height = UnityEngine.Random.Range(100f, 220f);

        age = UnityEngine.Random.Range(0f, 108f);

        id = GenerateRandomID();
    }

    private string GenerateRandomOrigin()
    {
        int len = UnityEngine.Random.Range(3, 12);
        string s = "";
        for (int i = 0; i < len; i++)
        {
            s += (char)UnityEngine.Random.Range('A', 'Z' + 1);
        }
        return s;
    }

    private long GenerateRandomID()
    {
        string idStr = "";
        for (int i = 0; i < 10; i++)
        {
            idStr += UnityEngine.Random.Range(0, 10).ToString();
        }
        return long.Parse(idStr);
    }
}