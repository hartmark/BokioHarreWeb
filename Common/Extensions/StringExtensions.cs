namespace Common.Extensions;

public static partial class StringExtensions
{
    public static string Unescape(this string input)
    {
        // Replace the Unicode escape sequences with their corresponding characters
        return EscapedTextRegex().Replace(input, 
            match => char.ConvertFromUtf32(Convert.ToInt32(match.Groups[1].Value, 16)));
    }

    [System.Text.RegularExpressions.GeneratedRegex(@"\\u([0-9A-Fa-f]{4})")]
    private static partial System.Text.RegularExpressions.Regex EscapedTextRegex();
}