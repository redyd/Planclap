using Planclap.Client.Domains.IEntities;
using Planclap.Client.Presentations.IViewModel;

namespace Planclap.Client.Presentations.PresentationServices;

public interface ISeatManagement
{
    IReadOnlyList<ISeatViewModel> AllSeats { get; }

    IReadOnlyList<ISeatViewModel> SelectedSeats { get; }

    int SelectedCount { get; }

    void SetSeats(IReadOnlyList<ISeatViewModel> seats, Action<ISeatViewModel> onSeatClick, Func<ISeatViewModel, bool> canClick);

    void ToggleSelection(ISeatViewModel seat);

    void Clear();

    event Action? SelectionChanged;

    IReadOnlyList<ISeatViewModel> MapToViewModels(IScheduled scheduled, int rows, int cols);

    IList<ITicket> MapToTickets(IEnumerable<ISeatViewModel> selectedSeats);
}
