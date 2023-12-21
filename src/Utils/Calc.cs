using UnityEngine;

static class Calc
{
    /* TODO InverseLerp? */
    public static float NormalizeFloat(float value, float start, float end) =>
        Mathf.Clamp((value - start) / (end - start), 0, 1);
}
