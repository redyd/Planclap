using Planclap.Client.Domains.IEntities;
using Planclap.Client.Domains.Services;
using Planclap.Client.Presentations.Routers;

namespace Planclap.Client.Presentations.PresentationServices;

public class PayBookingPageService : IPayBookingService
{
    private readonly IMovieService _domainMovieService;
    private readonly INavigateToPage _router;

    public PayBookingPageService(IMovieService domainMovieService, INavigateToPage router)
    {
        _domainMovieService = domainMovieService;
        _router = router;
    }

    public void SaveReservationAndNavigateHome(IReservation reservation)
    {
        _domainMovieService.NewReservation(reservation);
        _router.GoTo("Home");
    }

    public void NavigateHome() => _router.GoTo("Home");
}
