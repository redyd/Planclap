using Planclap.Client.Presentations.Common;

namespace Planclap.Client.Presentations.IViewModel;

public interface IBookingOverviewViewModel
{
    string ReservationTitle { get; }

    IReadOnlyList<ISeatViewModel> Seats { get; }

    ActionRelayCommand OnCreateBookingRequested { get; }

    int Rows { get; }

    int Columns { get; }
}
