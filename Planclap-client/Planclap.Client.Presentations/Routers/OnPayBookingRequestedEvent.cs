using Planclap.Client.Domains.IEntities;

namespace Planclap.Client.Presentations.Routers;

public class OnPayBookingRequestedEvent : EventArgs
{
    public IReservation? Reservation { get; init; }
}
