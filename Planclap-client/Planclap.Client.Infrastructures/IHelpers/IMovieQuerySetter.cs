using Planclap.Client.Domains.core;

namespace Planclap.Client.Infrastructures.IHelpers;

public interface IMovieQuerySetter
{
    ISqlWrapper ExecuteFetchBySlugQuery(ISqlWrapper db, ISet<MovieSlug> slugs);
}
