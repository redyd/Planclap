using NSubstitute;
using Planclap.Client.Domains.Services;
using Planclap.Client.Presentations.Common;
using Planclap.Client.Presentations.Routers;
using Planclap.Client.Presentations.ViewModel;

namespace Planclap.Client.Presentations.Tests.ViewModel;

public class PayBookingPageViewModelTests
{
    [Test]
    public void Should_Init_Controller_With_Non_Null_Values()
    {
        var service = Substitute.For<IMovieService>();
        var router = Substitute.For<INavigateToPage>();
        var pageNotifier = Substitute.For<IPageNotifier>();

        var page = new PayBookingPageViewModel(service, router, pageNotifier);

        Assert.That(page.PayBookingController, Is.Not.Null);
    }
}
