public class MorphConfig
{
    public float multiplier { get; }
    public DAZMorph morph { get; }

    public MorphConfig(string name, float multiplier)
    {
        this.multiplier = multiplier;
        morph = Utils.GetMorph(name);
    }
}
