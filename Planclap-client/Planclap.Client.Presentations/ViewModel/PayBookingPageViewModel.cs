using Planclap.Client.Domains.Services;
using Planclap.Client.Presentations.Common;
using Planclap.Client.Presentations.IViewModel;
using Planclap.Client.Presentations.PresentationServices;
using Planclap.Client.Presentations.Routers;

namespace Planclap.Client.Presentations.ViewModel;

public class PayBookingPageViewModel
{
    public PayBookingPageViewModel(IMovieService movieService, INavigateToPage router, IPageNotifier pageNotifier)
    {
        var presentationService = new PayBookingPageService(movieService, router);

        PayBookingController = new PayBookingViewModel(presentationService, pageNotifier);
    }

    public IPayBookingViewModel PayBookingController { get; }
}
