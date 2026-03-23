using Planclap.Client.Domains.core;
using Planclap.Client.Domains.IEntities;
using Planclap.Client.Presentations.Routers;

namespace Planclap.Client.Presentations.PresentationServices;

public interface IBookingOverviewService
{
    int RoomWidth { get; }

    int RoomDepth { get; }

    MovieSession this[MovieSlug slug] { get; }

    void OnMovieClickedEvent(Action<OnMovieClickEvent> action);

    void GoToPayBookingForm(MovieSlug slug, IList<ITicket> tickets);

    bool IsMovieAvailable(MovieSlug slug);

    event EventHandler<NavigationEventArgs>? Navigated;
}
