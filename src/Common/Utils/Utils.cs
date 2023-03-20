using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

static class Utils
{
    public static Transform DestroyLayout(Transform transform)
    {
        Object.Destroy(transform.GetComponent<LayoutElement>());
        return transform;
    }

    public static Regex Regex(string regexStr) => new Regex(regexStr, RegexOptions.Compiled);
}
