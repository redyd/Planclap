namespace Planclap.Client.Domains.core;

public record CineCheckAge
{
    private static readonly HashSet<int> ValidAges = new()
    {
        6,
        9,
        12,
        14,
        16,
        18,
    };

    public CineCheckAge(string value)
    {
        IntValue = ParseAge(value.ToLowerInvariant());
    }

    public string StringValue => IntValue == 0 ? "al" : IntValue.ToString();
    public int IntValue { get; }

    private static int ParseAge(string value)
    {
        if (value == "al")
        {
            return 0;
        }

        if (int.TryParse(value, out var intVal) && ValidAges.Contains(intVal))
        {
            return intVal;
        }

        throw new ArgumentException($"Valeur d'âge invalide : '{value}'");
    }

    public static bool IsValid(string value)
    {
        try
        {
            ParseAge(value.ToLowerInvariant());
            return true;
        }
        catch
        {
            return false;
        }
    }
}
