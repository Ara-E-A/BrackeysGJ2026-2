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
    }

    private void makeAgeRule()
    {
        //randomize for range of allowed ages
        Tuple<int, int> ageRange = new Tuple<int, int>(random.Range(0, 54), random.Range(54, 108));
        //randomize three specific ages that are not allowed
        float[] specificExcludedAges = new float[] { random.Range(0, 108), random.Range(0, 108), random.Range(0, 108) };
        
        this.ageRule = new AgeRule(ageRange, specificExcludedAges);
    }

}