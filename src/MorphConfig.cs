sealed class MorphConfig
{
    readonly DAZMorph _dazMorph;
    public JSONStorableFloat groupMultiplierJsf;

    public MorphConfig(DAZMorph dazMorph)
    {
        _dazMorph = dazMorph;
    }

    public void Update(float value, float min, float max)
    {
        value = Calc.NormalizeFloat(value, min, max);
        _dazMorph.morphValue = groupMultiplierJsf.val * value;
    }

    public void Reset()
    {
        _dazMorph.morphValue = 0;
    }
}
