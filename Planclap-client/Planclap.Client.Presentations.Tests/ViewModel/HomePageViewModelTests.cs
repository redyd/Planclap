using NSubstitute;
using Planclap.Client.Domains.Services;
using Planclap.Client.Presentations.Common;
using Planclap.Client.Presentations.Routers;
using Planclap.Client.Presentations.ViewModel;

namespace Planclap.Client.Presentations.Tests.ViewModel;

public class HomePageViewModelTests
{
    [Test]
    public void Should_Init_Controller_With_Non_Null_Values()
    {
        var service = Substitute.For<IMovieService>();
        var time = Substitute.For<ITimeService>();
        var router = Substitute.For<INavigateToPage>();
        var pageNotifier = Substitute.For<IPageNotifier>();

        var homePage = new HomePageViewModel(service, time, router, pageNotifier);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(homePage.BookingOverviewController, Is.Not.Null);
            Assert.That(homePage.SelectShowController, Is.Not.Null);
            Assert.That(homePage.ShowDetailController, Is.Not.Null);
        }
    }
}
