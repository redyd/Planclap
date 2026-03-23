namespace Planclap.Client.Presentations.Dtos;

public record ShowDetailDto(
    string Title,
    string Description,
    string Poster,
    TimeSpan Duration,
    IReadOnlyList<string> Tags);
