using System.Collections.ObjectModel;
using Planclap.Client.Presentations.Common;

namespace Planclap.Client.Presentations.IViewModel;

public interface IPayBookingViewModel
{
    string Title { get; }

    int Reduction { get; }

    double Price { get; }

    bool ShowWarning { get; }

    string WarningMessage { get; }

    ObservableCollection<IFieldPayBookingViewModel> Inputs { get; }

    ActionRelayCommand OnCancel { get; }

    ActionRelayCommand OnPay { get; }
}
