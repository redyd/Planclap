namespace Planclap.Client.Infrastructures.Dto;

public record MovieDto(string Slug, string Title, string Poster, string Description, IList<string> CineChecks);
