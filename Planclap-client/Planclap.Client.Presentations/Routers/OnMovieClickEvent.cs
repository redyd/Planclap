using Planclap.Client.Domains.core;

namespace Planclap.Client.Presentations.Routers;

public class OnMovieClickEvent : EventArgs
{
    public MovieSlug? Slug { get; init; }
}
