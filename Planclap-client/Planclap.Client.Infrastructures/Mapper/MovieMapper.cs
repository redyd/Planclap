using System.Data.Common;
using Planclap.Client.Domains.core;
using Planclap.Client.Infrastructures.Dto;
using Planclap.Client.Infrastructures.Helpers;

namespace Planclap.Client.Infrastructures.Mapper;

public class MoviesMapper : IMovieMapper
{
    public MovieDto MapFromReader(DbDataReader reader)
        => new(
            (string)reader["slug"],
            (string)reader["title"],
            (string)reader["poster"],
            (string)reader["description"],
            ((string)reader["cinechecks"]).Split(','));

    public IList<Movie> MapFromDto(IList<MovieDto> movieDtos)
    {
        var movies = new List<Movie>(movieDtos.Count);
        movies
            .AddRange(movieDtos
                .Select(movieDto => new Movie(
                    new MovieSlug(movieDto.Slug),
                    new MovieTitle(movieDto.Title),
                    new MovieDescription(movieDto.Description),
                    MapFromDto(movieDto.CineChecks),
                    new Uri(movieDto.Poster))));

        return movies;
    }

    private static CineChecksGroup MapFromDto(IList<string> cineChecks)
    {
        var (ageStr, others) = CineCheckHelper.SplitAgeAndOther(cineChecks);
        var age = new CineCheckAge(ageStr);
        var list = others.Select(c => new CineCheck(c)).ToList();
        return new CineChecksGroup(age, list);
    }
}
