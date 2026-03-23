using System.Text;
using System.Text.RegularExpressions;

namespace Planclap.Client.Domains.core;

public partial record MovieSlug
{
    private static readonly Regex Marks = MyRegex();

    public MovieSlug(string slug)
    {
        Value = Slugify(slug);
    }

    public string Value { get; }

    public static string Slugify(string value)
    {
        var result = StripAccents(value).ToLowerInvariant();
        result = Unwanted().Replace(result, "-");
        result = MultipleMinus().Replace(result, "-");
        result = MinusEndStart().Replace(result, string.Empty);

        return result;
    }

    private static string StripAccents(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        var normalized = input.Normalize(NormalizationForm.FormD);
        var result = Marks.Replace(normalized, string.Empty);

        return result;
    }

    public override string ToString() => Value;

    [GeneratedRegex(@"\p{M}", RegexOptions.Compiled)]
    private static partial Regex MyRegex();

    [GeneratedRegex(@"\s|\p{P}|\W")]
    private static partial Regex Unwanted();

    [GeneratedRegex("-{2,}")]
    private static partial Regex MultipleMinus();

    [GeneratedRegex("^-|-$")]
    private static partial Regex MinusEndStart();
}
