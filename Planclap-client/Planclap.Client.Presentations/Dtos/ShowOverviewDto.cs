namespace Planclap.Client.Presentations.Dtos;

public record ShowOverviewDto(
    Uri Poster,
    string Title = "",
    TimeOnly StartAt = default,
    TimeSpan Duration = default,
    bool CanBuy = true);
