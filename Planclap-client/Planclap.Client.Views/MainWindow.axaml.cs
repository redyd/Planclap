using Avalonia.Controls;
using Planclap.Client.Presentations.Routers;
using Serilog;

namespace Planclap.Client.Views;

public partial class MainWindow : Window
{
    private readonly Dictionary<string, UserControl> _pages = new();

    public MainWindow()
    {
        InitializeComponent();
    }

    public ILogger? Logger { get; set; }

    public void AddPage(string pageName, UserControl page)
    {
        _pages[pageName] = page;
        if (_pages.Count == 1)
        {
            Content = _pages[pageName];
        }
    }

    private void NavigateToPage(string pageName)
    {
        if (!_pages.TryGetValue(pageName, out var page))
        {
            Logger?.Error("Page {pageName} does not exist", pageName);
            throw new ArgumentException($"Page {pageName} does not exist");
        }

        Content = page;
    }

    public void OnNavigating(object? sender, NavigationEventArgs e)
    {
        Logger?.Information("Navigating from {source} to {destination}", e.From, e.To);
        NavigateToPage(e.To);
        Logger?.Information("Current page is {current}", e.To);
    }
}
