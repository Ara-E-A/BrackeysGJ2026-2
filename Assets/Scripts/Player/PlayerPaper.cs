using UnityEngine;

public class PlayerPaper : Paper
{
    public static readonly string[] AllSexes =
    {
        "Male", "Female", "Nonbinary", "Agender", "Fluid", "Plasma",
        "Glorb", "Vorb", "Zorb", "Blorb"
    };

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
        string[] names = NameGenerator.GetNames();
        name = names[UnityEngine.Random.Range(0, names.Length)];

        origin = GenerateRandomOrigin();

        sex = AllSexes[UnityEngine.Random.Range(0, AllSexes.Length)];

        height = UnityEngine.Random.Range(120f, 230f); // stay within the Papers UI's enforced range

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

    private int GenerateRandomID()
    {
        // 4-digit ID (0000-9999), matching IDRule and the Papers UI field.
        return UnityEngine.Random.Range(0, 10000);
    }
}