namespace Planclap.Client.Domains.core;

public record Movie(MovieSlug Slug, MovieTitle Title, MovieDescription Description, CineChecksGroup CineChecks, Uri PosterUrl);
