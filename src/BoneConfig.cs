using System;
using System.Collections.Generic;

sealed class BoneConfig
{
    public JSONStorableFloat multiplierJsf { get; set; }
    public List<MorphConfig> morphConfigs { get; set; }
    public Action<float> driver { get; set; }

    public void Update() => driver(multiplierJsf.val);

    public void Reset()
    {
        foreach(var config in morphConfigs)
        {
            config.morph.morphValue = 0;
        }
    }

    public void SetGroupMultiplierReferences()
    {
        foreach(var config in morphConfigs)
        {
            config.groupMultiplierJsf = multiplierJsf;
        }
    }
}
