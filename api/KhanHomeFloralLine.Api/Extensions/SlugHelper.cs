using System.Text;
using System.Text.RegularExpressions;

namespace KhanHomeFloralLine.Api.Extensions;

public static partial class SlugHelper
{
    [GeneratedRegex("[^a-z0-9\\s-]")]
    private static partial Regex InvalidCharsRegex();

    [GeneratedRegex("\\s+")]
    private static partial Regex SpaceRegex();

    public static string Generate(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        var normalized = input.Trim().ToLowerInvariant();
        normalized = InvalidCharsRegex().Replace(normalized, string.Empty);
        normalized = SpaceRegex().Replace(normalized, "-");
        normalized = normalized.Replace("--", "-");
        return normalized.Trim('-');
    }
}

