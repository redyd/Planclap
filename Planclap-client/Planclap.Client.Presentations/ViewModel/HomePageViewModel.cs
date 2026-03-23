using Planclap.Client.Domains.Services;
using Planclap.Client.Presentations.Common;
using Planclap.Client.Presentations.IViewModel;
using Planclap.Client.Presentations.Mapper;
using Planclap.Client.Presentations.PresentationServices;
using Planclap.Client.Presentations.Routers;

namespace Planclap.Client.Presentations.ViewModel;

public class HomePageViewModel
{
    public HomePageViewModel(IMovieService movieService, ITimeService timeService, INavigateToPage router, IPageNotifier pageNotifier)
    {
        Popup = new PopupViewModel(movieService);

        var planning = movieService.FetchPlanning(refresh: true);
        var homeService = new HomePageService(planning, router, pageNotifier, timeService);
        var seatSelection = new SeatManagement();
        var showDetailDtoMapper = new ShowDetailDtoMapper();

        SelectShowController = new SelectShowViewModel(homeService, planning);
        ShowDetailController = new ShowDetailViewModel(pageNotifier, planning, showDetailDtoMapper);
        BookingOverviewController = new BookingOverviewViewModel(homeService, seatSelection);

        SelectShowController.Initialize();
    }

    public IPopupViewModel Popup { get; }

    public ISelectShowViewModel SelectShowController { get; }

    public IShowDetailViewModel ShowDetailController { get; }

    public IBookingOverviewViewModel BookingOverviewController { get; }
}
