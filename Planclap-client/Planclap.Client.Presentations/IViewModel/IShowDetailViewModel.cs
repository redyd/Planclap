namespace Planclap.Client.Presentations.IViewModel;

public interface IShowDetailViewModel
{
    IReadOnlyList<string> Tags { get; }

    string Poster { get; }

    string Title { get; }

    TimeSpan Duration { get; }

    string Description { get; }
}
