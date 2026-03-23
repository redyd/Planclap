using System.ComponentModel;
using Planclap.Client.Presentations.Common;

namespace Planclap.Client.Presentations.IViewModel;

public interface ISeatViewModel : INotifyPropertyChanged
{
    string Id { get; }

    bool IsAvailable { get; }

    bool IsSelected { get; set; }

    ActionTRelayCommand<ISeatViewModel>? ClickCommand { get; set; }
}
