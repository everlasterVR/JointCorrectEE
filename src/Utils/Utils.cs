using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

static class Utils
{
    public static Regex NewRegex(string regexStr) => new Regex(regexStr, RegexOptions.Compiled);

    public static string EnumerableToString<T>(IEnumerable<T> enumerable, string separator = "\n")
        => string.Join(separator, enumerable.Select(item => item.ToString()).ToArray());
}
