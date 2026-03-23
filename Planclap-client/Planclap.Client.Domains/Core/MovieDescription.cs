namespace Planclap.Client.Domains.core;

public record MovieDescription
{
    public MovieDescription(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidDataException("Description cannot be empty.");
        }

        if (value.Length > 200)
        {
            throw new InvalidDataException("Description must be less than 200 characters.");
        }

        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
