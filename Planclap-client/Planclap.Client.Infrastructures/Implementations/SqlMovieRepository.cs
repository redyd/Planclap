using System.Data.Common;
using Planclap.Client.Domains.core;
using Planclap.Client.Domains.Exception;
using Planclap.Client.Domains.IRepository;
using Planclap.Client.Infrastructures.Helpers;
using Planclap.Client.Infrastructures.IHelpers;
using Planclap.Client.Infrastructures.Mapper;
using Serilog;

namespace Planclap.Client.Infrastructures.Implementations;

public class SqlMovieRepository(
    DbProviderFactory factory,
    string connectionString,
    IMovieMapper mapper,
    IMovieQuerySetter movieQuerySetter,
    ILogger? logger = null)
    : IMovieRepository
{
    public IList<Movie> FetchAllBySlug(ISet<MovieSlug> slugs)
    {
        try
        {
            logger?.Information("Starting to fetch movies with {Count} slugs", slugs.Count);
            if (slugs.Count == 0)
            {
                logger?.Warning("No slugs specified");
                return new List<Movie>();
            }

            using var db = SqlWrapper.WithAutoCommit(factory, connectionString);
            var query = movieQuerySetter.ExecuteFetchBySlugQuery(db, slugs);
            var moviesDto = query.ExecuteQuery(mapper.MapFromReader).ToList();
            var movies = mapper.MapFromDto(moviesDto);

            logger?.Information("Fetched {Count} movies successfully", movies.Count);

            return movies;
        }
        catch (DbException e)
        {
            logger?.Warning("Error fetching movies");
            throw new DatabaseException("An error occured while fetching movies", e);
        }
        catch (Exception ex)
        {
            logger?.Error(ex, "Unexpected error fetching movies");
            throw;
        }
    }
}
