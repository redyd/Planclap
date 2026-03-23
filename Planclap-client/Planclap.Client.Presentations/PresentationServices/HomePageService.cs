using Planclap.Client.Domains.core;
using Planclap.Client.Domains.Entities;
using Planclap.Client.Domains.IEntities;
using Planclap.Client.Domains.Services;
using Planclap.Client.Presentations.Common;
using Planclap.Client.Presentations.Dtos;
using Planclap.Client.Presentations.Routers;

namespace Planclap.Client.Presentations.PresentationServices;

public class HomePageService : IBookingOverviewService, ISelectShowService
{
    private readonly IPageNotifier _pageNotifier;
    private readonly IPlanning _planning;
    private readonly INavigateToPage _router;
    private readonly ITimeService _timeService;

    public HomePageService(IPlanning planning, INavigateToPage router, IPageNotifier pageNotifier, ITimeService timeService)
    {
        _planning = planning;
        _router = router;
        _pageNotifier = pageNotifier;
        _timeService = timeService;

        _router.Navigated += (_, e) => Navigated?.Invoke(this, e);
    }

    public void OnMovieClickedEvent(Action<OnMovieClickEvent> action)
        => _pageNotifier.Subscribe(action);

    public void GoToPayBookingForm(MovieSlug slug, IList<ITicket> tickets)
    {
        var reservation = new Reservation(_planning[slug], tickets);
        _pageNotifier.Publish(new OnPayBookingRequestedEvent { Reservation = reservation });
        _router.GoTo("PayBooking");
    }

    public bool IsMovieAvailable(MovieSlug slug)
        => _planning[slug].IsAvailable(_timeService.Current);

    public int RoomWidth => _planning.RoomSize.Depth;

    public int RoomDepth => _planning.RoomSize.Width;

    public MovieSession this[MovieSlug slug] => _planning[slug];

    public event EventHandler<NavigationEventArgs>? Navigated;

    public void OnMovieClick(ShowOverviewDto showDetailViewModel)
        => _pageNotifier.Publish(new OnMovieClickEvent { Slug = new MovieSlug(showDetailViewModel.Title) });

    public DateTime Current => _timeService.Current;
}
