using System.Collections.ObjectModel;
using Planclap.Client.Domains.core;
using Planclap.Client.Domains.Entities;
using Planclap.Client.Domains.IEntities;
using Planclap.Client.Presentations.Common;
using Planclap.Client.Presentations.IViewModel;
using Planclap.Client.Presentations.ViewModel;

namespace Planclap.Client.Presentations.PresentationServices;

public class SeatManagement : ISeatManagement
{
    private readonly ObservableCollection<ISeatViewModel> _allSeats = new();
    private readonly ObservableCollection<ISeatViewModel> _selectedSeats = new();

    public IReadOnlyList<ISeatViewModel> AllSeats => _allSeats;

    public IReadOnlyList<ISeatViewModel> SelectedSeats => _selectedSeats;

    public int SelectedCount => _selectedSeats.Count;

    public event Action? SelectionChanged;

    public void SetSeats(
        IReadOnlyList<ISeatViewModel> seats,
        Action<ISeatViewModel> onSeatClick,
        Func<ISeatViewModel, bool> canClick)
    {
        _allSeats.Clear();
        _selectedSeats.Clear();

        var command = new ActionTRelayCommand<ISeatViewModel>(onSeatClick, canClick);
        foreach (var seat in seats)
        {
            seat.ClickCommand = command;
            _allSeats.Add(seat);
        }

        SelectionChanged?.Invoke();
    }

    public void ToggleSelection(ISeatViewModel seat)
    {
        if (!_selectedSeats.Remove(seat))
        {
            _selectedSeats.Add(seat);
            seat.IsSelected = true;
        }
        else
        {
            seat.IsSelected = false;
        }

        SelectionChanged?.Invoke();
    }

    public void Clear()
    {
        _selectedSeats.Clear();
        _allSeats.Clear();
        SelectionChanged?.Invoke();
    }

    public IReadOnlyList<ISeatViewModel> MapToViewModels(IScheduled scheduled, int rows, int cols)
    {
        var seats = new List<SeatViewModel>();

        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < cols; col++)
            {
                seats.Add(new SeatViewModel(
                    CreateSeatId(row, col),
                    !scheduled.SeatTaken(row, col)));
            }
        }

        return seats.AsReadOnly();
    }

    public IList<ITicket> MapToTickets(IEnumerable<ISeatViewModel> selectedSeats) =>
        selectedSeats
            .Select(seat => ParseSeatId(seat.Id))
            .Select(coords => new Ticket(new Seat(coords.row, coords.col)))
            .ToList<ITicket>();

    private static string CreateSeatId(int row, int col)
        => $"{row}-{col}";

    private static (ushort row, ushort col) ParseSeatId(string seatId)
    {
        var parts = seatId.Split('-');
        return (ushort.Parse(parts[0]), ushort.Parse(parts[1]));
    }
}
