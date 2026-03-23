namespace Planclap.Client.Presentations.Routers;

public class NavigationEventArgs : EventArgs
{
    public string From { get; init; } = string.Empty;

    public string To { get; init; } = string.Empty;
}
