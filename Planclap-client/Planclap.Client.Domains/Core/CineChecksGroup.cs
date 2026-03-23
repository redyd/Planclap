namespace Planclap.Client.Domains.core;

public record CineChecksGroup(CineCheckAge Age, IList<CineCheck> CineChecks)
{
    public int Size => 1 + CineChecks.Count;

    public int MinAge => Age.IntValue;
}
