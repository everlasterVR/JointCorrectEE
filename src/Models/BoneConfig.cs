using System;
using System.Collections.Generic;

sealed class BoneConfig
{
    public StorableFloat multiplierFloat;
    public List<MorphConfig> morphConfigs;
    public Action<float> driver;

    public void Update() => driver(multiplierFloat.val);

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
            morphConfigs[i].groupMultiplierFloat = multiplierFloat;
        }
    }
}
