static partial class StringExtensions
{
    public static string Bold(this string str) => $"<b>{str}</b>";

    public static string Size(this string str, int size) => $"<size={size}>{str}</size>";
}
