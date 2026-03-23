using System.Collections.ObjectModel;
using Planclap.Client.Presentations.Common;
using Planclap.Client.Presentations.IViewModel;

namespace Planclap.Client.Presentations.View;

public class DummyPayBookingViewModel : IPayBookingViewModel
{
    public string Title => "Réservation pour DummyPayBookingViewModel";

    public int Reduction => 10;

    public double Price => 34.5;

    public bool ShowWarning => true;

    public string WarningMessage => """
                                    Ce film ne convient pas à des enfants.
                                    Le cinéma décline toute responsabilité en cas de traumatisme.
                                    """;

    public ObservableCollection<IFieldPayBookingViewModel> Inputs =>
    [
        new DummyFieldPayBookingViewModel("34", isValid: true, price: 10, seatId: "1 - 4", warningInput: string.Empty),
        new DummyFieldPayBookingViewModel("ezfe", isValid: false, price: 0, seatId: "1 - 5", warningInput: "Entrée invalide"),
        new DummyFieldPayBookingViewModel("3", isValid: true, price: 7, seatId: "1 - 6", warningInput: string.Empty)
    ];

    public ActionRelayCommand OnCancel => new(() => { });

    public ActionRelayCommand OnPay => new(() => { }, () => true);
}
