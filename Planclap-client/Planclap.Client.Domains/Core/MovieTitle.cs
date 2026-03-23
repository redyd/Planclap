namespace Planclap.Client.Domains.core;

public record MovieTitle
{
    public MovieTitle(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentNullException(value);
        }

        Value = value;
    }

    public string Value { get; }
}
