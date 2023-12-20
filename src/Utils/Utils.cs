using System.Collections.Generic;
using System.Linq;
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

    public static Regex NewRegex(string regexStr) => new Regex(regexStr, RegexOptions.Compiled);

    public static string EnumerableToString<T>(IEnumerable<T> enumerable, string separator = "\n")
        => string.Join(separator, enumerable.Select(item => item.ToString()).ToArray());
}
