using System;
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

    private Rule<float> ageRule;
    private Rule<float> heightRule;
    private Rule<string> originRule;
    private Rule<string> nameRule;
    private Rule<string> sexRule;
    private Rule<int> idRule;

    public PaperReq()
    {
        makeAgeRule();
        makeHeightRule();
        makeOriginRule();
        makeNameRule();
        makeSexRule();
        makeIDRule();
    }

    private void makeAgeRule()
    {
        var ageRange = new Tuple<int, int>(
            UnityEngine.Random.Range(0, 54),
            UnityEngine.Random.Range(54, 108)
        );

        float[] specificExcludedAges = new float[]
        {
            UnityEngine.Random.Range(0f, 108f),
            UnityEngine.Random.Range(0f, 108f),
            UnityEngine.Random.Range(0f, 108f)
        };

        this.ageRule = new AgeRule(ageRange, specificExcludedAges);
    }

    private void makeHeightRule()
    {
        float minHeight = UnityEngine.Random.Range(120f, 150f);
        float maxHeight = UnityEngine.Random.Range(160f, 200f);

        int excludedCount = UnityEngine.Random.Range(1, 4);

        float[] excludedStarts = new float[excludedCount];
        float[] excludedEnds = new float[excludedCount];

        for (int i = 0; i < excludedCount; i++)
        {
            float pocketWidth = UnityEngine.Random.Range(5f, 10f);

            float start = UnityEngine.Random.Range(minHeight, maxHeight - pocketWidth);
            float end = start + pocketWidth;

            excludedStarts[i] = start;
            excludedEnds[i] = end;
        }

        this.heightRule = new HeightRule(minHeight, maxHeight, excludedStarts, excludedEnds);
    }

    private void makeNameRule()
    {
        string[] names = RulesList.getNames();
        string[] forbiddenNames = new string[3];

        for (int i = 0; i < 3; i++)
        {
            string candidate = names[UnityEngine.Random.Range(0, names.Length)];

            while (forbiddenNames.Contains(candidate))
                candidate = names[UnityEngine.Random.Range(0, names.Length)];

            forbiddenNames[i] = candidate;
        }

        this.nameRule = new NameRule(forbiddenNames);
    }

    private void makeOriginRule()
    {
        int minLen = UnityEngine.Random.Range(3, 6);
        int maxLen = UnityEngine.Random.Range(6, 12);

        int requiredCount = UnityEngine.Random.Range(1, 4);
        char[] requiredChars = new char[requiredCount];

        for (int i = 0; i < requiredCount; i++)
            requiredChars[i] = (char)UnityEngine.Random.Range('A', 'Z' + 1);

        int forbiddenCount = UnityEngine.Random.Range(1, 4);
        char[] forbiddenChars = new char[forbiddenCount];

        for (int i = 0; i < forbiddenCount; i++)
            forbiddenChars[i] = (char)UnityEngine.Random.Range('A', 'Z' + 1);

        this.originRule = new OriginRule(minLen, maxLen, requiredChars, forbiddenChars);
    }

    private void makeSexRule()
    {
        string[] allSexes =
        {
            "Male", "Female", "Nonbinary", "Agender", "Fluid", "Glorb", "Vorb", "Zorb", "Blorb"
        };

        int count = UnityEngine.Random.Range(2, 5);
        string[] allowed = new string[count];

        for (int i = 0; i < count; i++)
            allowed[i] = allSexes[UnityEngine.Random.Range(0, allSexes.Length)];

        this.sexRule = new SexRule(allowed);
    }

    private void makeIDRule()
    {
        bool mustBeDivisible = UnityEngine.Random.Range(0, 2) == 0;

        int divisor = UnityEngine.Random.Range(2, 11);

        int requiredDigit = UnityEngine.Random.Range(0, 10);

        int forbiddenDigit = UnityEngine.Random.Range(0, 10);
        while (forbiddenDigit == requiredDigit)
            forbiddenDigit = UnityEngine.Random.Range(0, 10);

        this.idRule = new IDRule(mustBeDivisible, divisor, requiredDigit, forbiddenDigit);
    }
}
