using Planclap.Client.Domains.core;
using Planclap.Client.Presentations.Common;
using Planclap.Client.Presentations.IViewModel;
using Planclap.Client.Presentations.PresentationServices;
using Planclap.Client.Presentations.Routers;

namespace Planclap.Client.Presentations.ViewModel;

public class BookingOverviewViewModel : ViewModelBase, IBookingOverviewViewModel
{
    private readonly ISeatManagement _seatManagement;

    // object
    private readonly IBookingOverviewService _service;

    // data
    private MovieSlug? _lastSelectedSlug;

    public BookingOverviewViewModel(IBookingOverviewService service, ISeatManagement seatManagement)
    {
        _service = service;
        _seatManagement = seatManagement;

        _service.OnMovieClickedEvent(RefreshSeats);
        _service.Navigated += OnNavigated;
        _seatManagement.SelectionChanged += OnManagementChanged;

        OnCreateBookingRequested = new ActionRelayCommand(CreateBookingAndNavigate, CanExecuteBooking);
    }

    // == COMMAND ==
    public ActionRelayCommand OnCreateBookingRequested { get; }

    // == PROPERTY ==
    public int Rows => _service.RoomWidth;

    public int Columns => _service.RoomDepth;

    public string ReservationTitle => "Réservation";

    public IReadOnlyList<ISeatViewModel> Seats => _seatManagement.AllSeats;

    // == EVENT ==
    private void RefreshSeats(OnMovieClickEvent obj)
    {
        if (obj.Slug != null)
        {
            RefreshSeats(obj.Slug);
        }
    }

    private void RefreshSeats(MovieSlug slug)
    {
        var seats = _seatManagement.MapToViewModels(_service[slug].Scheduled, Rows, Columns);
        _seatManagement.SetSeats(seats, OnSeatClickAction, AllowClick);

        _lastSelectedSlug = slug;
        OnCreateBookingRequested.RaiseCanExecuteChanged();
    }

    private void OnManagementChanged()
        => OnCreateBookingRequested.RaiseCanExecuteChanged();

    private void CreateBookingAndNavigate()
    {
        if (_lastSelectedSlug == null)
        {
            return;
        }

        var tickets = _seatManagement.MapToTickets(_seatManagement.SelectedSeats);
        _service.GoToPayBookingForm(_lastSelectedSlug, tickets);

        _seatManagement.Clear();
    }

    private bool CanExecuteBooking()
        => _seatManagement.SelectedCount > 0
           && _lastSelectedSlug != null
           && _service.IsMovieAvailable(_lastSelectedSlug);

    private void OnSeatClickAction(ISeatViewModel seat)
        => _seatManagement.ToggleSelection(seat);

    private bool AllowClick(ISeatViewModel seat)
        => seat.IsAvailable;

    // == ROUTING ==
    private void OnNavigated(object? sender, NavigationEventArgs e)
    {
        if (_lastSelectedSlug != null && e.To == "Home")
        {
            RefreshSeats(_lastSelectedSlug);
        }
    }
}
