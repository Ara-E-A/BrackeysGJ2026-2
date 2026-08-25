using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

public class PaperReq : Paper
{
    public string name;
    public string origin;
    public string sex;
    public float id;
    public float height;

    private Rule ageRule;
    private Rule heightRule;
    private Rule originRule;
    private Rule nameRule;
    private Rule sexRule;
    private Rule idRule;


    // randomize a requirement paper, then later, enforce the rules over player paper vs reqpaper somehow.
    public PaperReq()
    {
        makeAgeRule();
        makeOriginRule();
    }

    private void makeAgeRule()
    {
        //randomize for range of allowed ages
        System.Tuple<int, int> ageRange = new System.Tuple<int, int>(Random.Range(0, 54), Random.Range(54, 108));
        //randomize three specific ages that are not allowed
        float[] specificExcludedAges = new float[] { Random.Range(0, 108), Random.Range(0, 108), Random.Range(0, 108) };
        
        this.ageRule = new AgeRule(ageRange, specificExcludedAges);
    }

    private void makeNameRule()
    {
        string[] names = RulesList.getNames();

        string[] forbiddenNames = new string[3];
        for (int i = 0; i < 3; i++)
        {
            forbiddenNames[i] = names[Random.Range(0, names.Length)];
        }
        this.nameRule = new NameRule(forbiddenNames);
    }

    private void makeOriginRule()
    {
        //randomize length range
        int minLen = Random.Range(3, 6);
        int maxLen = Random.Range(6, 12);

        //randomize required characters
        int requiredCount = Random.Range(1, 4);
        char[] requiredChars = new char[requiredCount];
        for (int i = 0; i < requiredCount; i++)
        {
            requiredChars[i] = (char)Random.Range('A', 'Z' + 1);
        }

        //randomize forbidden characters
        int forbiddenCount = Random.Range(1, 4);
        char[] forbiddenChars = new char[forbiddenCount];
        for (int i = 0; i < forbiddenCount; i++)
        {
            forbiddenChars[i] = (char)Random.Range('A', 'Z' + 1);
        }

        this.originRule = new OriginRule(minLen, maxLen, requiredChars, forbiddenChars);

        // Debug.Log(originRule.ToString());
    }

}