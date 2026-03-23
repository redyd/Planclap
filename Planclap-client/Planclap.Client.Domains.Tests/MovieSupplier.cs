using Planclap.Client.Domains.core;
using Planclap.Client.Domains.Entities;

namespace Planclap.Client.Domains.Tests;

public class MovieSupplier
{
    public static Movie Supply(string slug = "movie", string title = "Movie", string description = "Description", string age = "al")
        => new(
            new MovieSlug(slug),
            new MovieTitle(title),
            new MovieDescription(description),
            new CineChecksGroup(new CineCheckAge(age), new List<CineCheck>()),
            new Uri("https://www.poster.com"));

    public static MovieSession SupplySession(int duration = 60)
        => new(
            new Scheduled(DateTime.Now, TimeSpan.FromMinutes(duration)),
            Supply());
}
