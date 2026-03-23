using Planclap.Client.Domains.core;

namespace Planclap.Client.Domains.IRepository;

public interface IMovieRepository
{
    /// <summary>
    ///     Récupère tous les films planifiés par slug.
    /// </summary>
    /// <param name="slugs">Les slugs à chercher.</param>
    /// <returns>Une liste des films.</returns>
    IList<Movie> FetchAllBySlug(ISet<MovieSlug> slugs);
}
