namespace Planclap.Client.Domains.core;

public record CineCheck
{
    private static readonly Dictionary<string, List<string>> Mapping = new()
    {
        { "violence", ["Violence", "VIOLENCE"] },
        { "peur", ["Peur", "FEAR"] },
        { "sex", ["Sexe", "SEX"] },
        { "language", ["Paroles grossieres", "RUDE"] },
        { "discrimination", ["Discrimination", "DISCRIMINATION"] },
        { "drugs-and-alcohol", ["Drogues, alcool et fumer", "DRUGS"] },
    };

    public CineCheck(string value)
    {
        if (!FindKey(value))
        {
            throw new ArgumentException($"CineCheck '{value}' is not valid", nameof(value));
        }
    }

    public string Id { get; private set; } = string.Empty;

    private bool FindKey(string value)
    {
        foreach (var cineCheck in Mapping)
        {
            if (cineCheck.Value.Contains(value))
            {
                Id = cineCheck.Key;
                return true;
            }
        }

        return false;
    }
}
