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
        for(int i = 0; i < morphConfigs.Count; i++)
        {
            morphConfigs[i].morph.morphValue = 0;
        }
    }

    public void SetGroupMultiplierReferences()
    {
        for(int i = 0; i < morphConfigs.Count; i++)
        {
            morphConfigs[i].groupMultiplierJsf = multiplierJsf;
        }
    }
}
