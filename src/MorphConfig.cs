sealed class MorphConfig
{
    public DAZMorph morph { get; }

    // public JSONStorableFloat multiplierJsf { get; }
    public JSONStorableFloat groupMultiplierJsf { private get; set; }

    public MorphConfig(DAZMorph morph)
    {
        this.morph = morph;
    }

    // public MorphConfig(string name, JSONStorableFloat multiplierJsf)
    // {
    //     morph = Utils.GetMorph(name);
    //     this.multiplierJsf = multiplierJsf;
    // }

    public void Update(float value, float min, float max)
    {
        value = Calc.NormalizeFloat(value, min, max);
        // morph.morphValue = groupMultiplierJsf.val * multiplierJsf.val * value;
        morph.morphValue = groupMultiplierJsf.val * value;
    }
}
