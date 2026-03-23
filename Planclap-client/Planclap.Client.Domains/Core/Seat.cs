namespace Planclap.Client.Domains.core;

public readonly record struct Seat(ushort Row, ushort Column)
{
    public string ToId => $"{Row}-{Column}";
}
