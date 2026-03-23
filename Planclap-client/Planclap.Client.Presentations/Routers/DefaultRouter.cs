namespace Planclap.Client.Presentations.Routers;

public class DefaultRouter : INavigateToPage
{
    private string CurrentPageName { get; set; } = "Home";

    public void GoTo(string pageName)
    {
        var args = new NavigationEventArgs { From = CurrentPageName, To = pageName };

        Navigating?.Invoke(this, args);
        CurrentPageName = pageName;
        Navigated?.Invoke(this, args);
    }

    public event EventHandler<NavigationEventArgs>? Navigating;

    public event EventHandler<NavigationEventArgs>? Navigated;
}
