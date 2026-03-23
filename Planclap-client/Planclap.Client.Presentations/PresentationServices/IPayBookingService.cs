using Planclap.Client.Domains.IEntities;

namespace Planclap.Client.Presentations.PresentationServices;

public interface IPayBookingService
{
    void SaveReservationAndNavigateHome(IReservation reservation);

    void NavigateHome();
}
