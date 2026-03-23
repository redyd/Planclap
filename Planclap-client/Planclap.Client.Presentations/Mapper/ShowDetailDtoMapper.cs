using System.Collections.ObjectModel;
using Planclap.Client.Domains.core;
using Planclap.Client.Presentations.Dtos;

namespace Planclap.Client.Presentations.Mapper;

public class ShowDetailDtoMapper(string? imagesPath = null, string extension = "png") : IShowDetailDtoMapper
{
    private readonly string _imagePath = imagesPath ?? Path.Combine("Resources", "Images");

    public ShowDetailDto MapToDto(MovieSession movie)
        => new(
            movie.Movie.Title.Value,
            movie.Movie.Description.Value,
            MapPoster(movie),
            movie.Scheduled.Duration,
            MapTagsToImagePath(movie.Movie.CineChecks).ToList());

    private static string MapPoster(MovieSession movie)
        => movie.Movie.PosterUrl.AbsoluteUri;

    private ReadOnlyCollection<string> MapTagsToImagePath(CineChecksGroup cineChecks)
    {
        var tags = new List<string>(cineChecks.Size) { Path.Combine(_imagePath, $"{cineChecks.Age.StringValue}.{extension}") };

        tags.AddRange(cineChecks.CineChecks.Select(checks => Path.Combine(_imagePath, $"{checks.Id}.{extension}")));

        return tags.AsReadOnly();
    }
}
