using System.Diagnostics.CodeAnalysis;

sealed class MorphConfig
{
    DAZMorph _dazMorph;
    readonly string _uid;
    readonly DAZMorphBank _morphBank;
    public JSONStorableFloat groupMultiplierFloat;
    bool _enabled = true;

    public MorphConfig(DAZMorph dazMorph)
    {
        _dazMorph = dazMorph;
        _uid = dazMorph.uid;
        _morphBank = dazMorph.morphBank;
    }

    public void Update(float value, float min, float max)
    {
        value = Calc.NormalizeFloat(value, min, max);
        SetValue(groupMultiplierFloat.val * value);
    }

    void SetValue(float value)
    {
        if(!_enabled)
        {
            return;
        }

        if(_dazMorph.isDemandLoaded && !_dazMorph.isDemandActivated)
        {
            _dazMorph = _morphBank.GetMorphByUid(_uid); // Reactivates the morph in case it has been unloaded by e.g. memory optimization
            if(_dazMorph == null)
            {
                new LogBuilder().ErrorNoReport("Morph with uid '{0}' not found!", _uid);
                _enabled = false;
                return;
            }
        }

        _dazMorph.morphValue = value;
    }

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public float GetValue() => _dazMorph.morphValue;

    public void Reset() => _dazMorph.morphValue = 0;
}
