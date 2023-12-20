using System;
using System.Collections.Generic;

sealed class BoneConfig
{
    public JSONStorableFloat multiplierJsf;
    public List<MorphConfig> morphConfigs;
    public Action<float> driver;

    public void Update() => driver(multiplierJsf.val);

    public void Reset()
    {
        for(int i = 0; i < morphConfigs.Count; i++)
        {
            morphConfigs[i].Reset();
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
