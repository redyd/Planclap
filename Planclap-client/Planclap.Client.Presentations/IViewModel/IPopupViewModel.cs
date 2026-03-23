using Planclap.Client.Presentations.Common;

namespace Planclap.Client.Presentations.IViewModel;

public interface IPopupViewModel
{
    ActionRelayCommand Hide { get; }

    bool IsVisible { get; set; }

    string Content { get; set; }

    string Color { get; set; }

    string IconText { get; set; }
}
