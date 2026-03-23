using System.Text.Json;
using Planclap.Client.Domains.core;
using Planclap.Client.Domains.Exception;
using Planclap.Client.Domains.IRepository;
using Planclap.Client.Domains.Services;
using Planclap.Client.Infrastructures.Dto;
using Planclap.Client.Infrastructures.Helpers;
using Planclap.Client.Infrastructures.Mapper;
using Serilog;

namespace Planclap.Client.Infrastructures.Implementations;

public class JsonMovieFileRepository(string directory, IMovieMapper mapper, ITimeService time, ILogger? logger = null)
    : FileRepository(directory, "json"), IMovieRepository
{
    private readonly JsonHelper _json = new();

    public IList<Movie> FetchAllBySlug(ISet<MovieSlug> slugs)
    {
        logger?.Information("Starting to fetch every movie with slugs: {MovieSlug}", string.Join(", ", slugs.Select(slug => $"\"{slug}\"")));
        if (slugs.Count == 0)
        {
            logger?.Warning("No slugs specified");
            return new List<Movie>();
        }

        var path = GetFilePath(time.StartOfTheWeek);
        var rawText = File.ReadAllText(path);

        if (string.IsNullOrEmpty(rawText))
        {
            logger?.Error("The json file is empty");
            throw new InvalidResourcesException("Json file is empty");
        }

        var moviesDto = TryDeserialize<MoviesDto>(rawText);
        var wantedMovies = FilterMoviesBySlugs(slugs, moviesDto);
        var mapped = mapper.MapFromDto(wantedMovies);

        logger?.Information("Successfully fetched {Count} movies", mapped.Count);

        return mapped;
    }

    private T TryDeserialize<T>(string rawText) where T : class
    {
        T? dto;
        try
        {
            dto = _json.Deserialize<T>(rawText);
        }
        catch (JsonException)
        {
            logger?.Error("The json file is not valid");
            throw new InvalidResourcesException("Json file is invalid");
        }

        if (dto != null)
        {
            return dto;
        }

        logger?.Error("The json file is not valid");
        throw new InvalidResourcesException("Json file is invalid");
    }

    private static List<MovieDto> FilterMoviesBySlugs(ISet<MovieSlug> slugs, MoviesDto moviesDto)
    {
        var wanted = new List<MovieDto>(slugs.Count);

        wanted.AddRange(moviesDto.Movies.Where(dto => slugs.Contains(new MovieSlug(dto.Slug))));

        return wanted;
    }
}
