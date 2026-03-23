using Planclap.Client.Domains.core;

namespace Planclap.Client.Infrastructures.Tests;

public class MovieSupplier
{
    public static Movie Supply(string slug = "movie", string title = "Movie", string description = "Description", string age = "al")
        => new(
            new MovieSlug(slug),
            new MovieTitle(title),
            new MovieDescription(description),
            new CineChecksGroup(new CineCheckAge(age), new List<CineCheck>()),
            new Uri("https://www.poster.com"));
}
