using System.Collections.ObjectModel;
using Planclap.Client.Domains.IEntities;
using Planclap.Client.Presentations.Common;
using Planclap.Client.Presentations.IViewModel;
using Planclap.Client.Presentations.PresentationServices;
using Planclap.Client.Presentations.Routers;

namespace Planclap.Client.Presentations.ViewModel;

public class PayBookingViewModel : ViewModelBase, IPayBookingViewModel
{
    // data
    private readonly ObservableCollection<IFieldPayBookingViewModel> _payBookings = new();

    // object
    private readonly IPayBookingService _service;
    private IReservation? _reservation;

    public PayBookingViewModel(IPayBookingService service, IPageNotifier pageNotifier)
    {
        _service = service;

        pageNotifier.Subscribe<OnPayBookingRequestedEvent>(OnPayBookingRequestedEvent);
        OnCancel = new ActionRelayCommand(ClearAndCancel);
        OnPay = new ActionRelayCommand(OnPayCommand, IsValid);
    }

    // == COMMAND ==
    public ActionRelayCommand OnCancel { get; }

    public ActionRelayCommand OnPay { get; }

    // == PROPERTIES ==
    public bool ShowWarning => _reservation?.Tickets
        .Any(t => t.ValidTicket && t.Age < _reservation.MinimumAge) ?? false;

    public string WarningMessage => """
                                    Ce film ne convient pas à des enfants.
                                    Le cinéma décline toute responsabilité en cas de traumatisme.
                                    """;

    public ObservableCollection<IFieldPayBookingViewModel> Inputs
    {
        get => _payBookings;
        private set
        {
            _payBookings.Clear();
            foreach (var p in value)
            {
                _payBookings.Add(p);
            }
        }
    }

    public string Title => $"Réservation pour {_reservation?.Title.Value}";

    public int Reduction => _reservation?.Reduction ?? 0;

    public double Price => _reservation?.Price ?? 0;

    // == EVENT ==
    private void OnPayBookingRequestedEvent(OnPayBookingRequestedEvent e)
    {
        _reservation = e.Reservation;
        InitializeField();
    }

    private void ClearAndCancel()
    {
        _reservation = null;
        _payBookings.Clear();
        _service.NavigateHome();
    }

    private void OnPayCommand()
    {
        if (_reservation is not { IsValid: true })
        {
            return;
        }

        _service.SaveReservationAndNavigateHome(_reservation);
        _reservation = null;
        _payBookings.Clear();
    }

    private bool IsValid() => _reservation?.IsValid ?? false;

    private void InitializeField()
    {
        if (_reservation == null)
        {
            return;
        }

        var list = _reservation.Tickets
            .Select(ticket => new FieldPayBookingViewModel(ticket, OnInputPropertiesChanged));

        Inputs = new ObservableCollection<IFieldPayBookingViewModel>(list);
        NotifyPropertyChanged(nameof(Reduction));
        NotifyPropertyChanged(nameof(Title));

        OnInputPropertiesChanged();
    }

    /// <summary>
    ///     Méthode appelée par les champs pour mettre à jour le prix et la validité.
    /// </summary>
    private void OnInputPropertiesChanged()
    {
        NotifyPropertyChanged(nameof(Price));
        NotifyPropertyChanged(nameof(ShowWarning));
        OnPay.RaiseCanExecuteChanged();
    }
}
