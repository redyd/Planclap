namespace Planclap.Client.Presentations.Routers;

public interface INavigateToPage
{
    void GoTo(string pageName);

    event EventHandler<NavigationEventArgs> Navigating;

    event EventHandler<NavigationEventArgs> Navigated;
}
