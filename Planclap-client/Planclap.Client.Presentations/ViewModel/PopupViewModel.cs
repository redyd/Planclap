using Planclap.Client.Domains.Events;
using Planclap.Client.Domains.Services;
using Planclap.Client.Presentations.Common;
using Planclap.Client.Presentations.IViewModel;

namespace Planclap.Client.Presentations.ViewModel;

public class PopupViewModel : ViewModelBase, IPopupViewModel
{
    private bool _isVisible;
    private string _content;
    private string _color;
    private string _iconText;

    public PopupViewModel(IMovieService service)
    {
        service.RepositoryErrorEvent += OnNewMessage;

        _isVisible = false;
        _content = string.Empty;
        _color = string.Empty;
        _iconText = string.Empty;

        Hide = new ActionRelayCommand(CloseError, () => _isVisible);
    }

    public ActionRelayCommand Hide { get; }

    private void CloseError()
    {
        IsVisible = false;
        Content = string.Empty;
    }

    public bool IsVisible
    {
        get => _isVisible;
        set => SetAndRaise(ref _isVisible, value);
    }

    public string Content
    {
        get => _content;
        set => SetAndRaise(ref _content, value);
    }

    public string Color
    {
        get => _color;
        set => SetAndRaise(ref _color, value);
    }

    public string IconText
    {
        get => _iconText;
        set => SetAndRaise(ref _iconText, value);
    }

    private void OnNewMessage(object? sender, RepositoryErrorEventArgs args)
    {
        Content = args.Message;
        Color = args.Type switch
        {
            'e' => "#B91C1C",
            's' => "#228B22",
            _ => "#1E90FF"
        };

        IconText = args.Type switch
        {
            'e' => "!",
            's' => "✓",
            _ => string.Empty
        };

        IsVisible = true;
    }
}
