using Planclap.Client.Domains.core;
using Planclap.Client.Domains.IEntities;

namespace Planclap.Client.Domains.Services;

public abstract class MovieServiceHelper
{
    public static IList<MovieSession> Merge(IDictionary<MovieSlug, IList<IScheduled>> scheduled, IList<Movie> movies)
    {
        var result = new List<MovieSession>();

        if (scheduled.Count == 0 || movies.Count == 0)
        {
            return result;
        }

        foreach (var movie in movies)
        {
            var allScheduled = scheduled[movie.Slug];

            // iterate because a slug could be planned at different schedules
            foreach (var singleScheduled in allScheduled)
            {
                result.Add(new MovieSession(singleScheduled, movie));
            }
        }

        return result;
    }
}
