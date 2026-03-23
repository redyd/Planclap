using Planclap.Client.Presentations.Routers;

namespace Planclap.Client.Presentations.Tests.Routers;

/// <summary>
///     Réalisé à l'aide de l'IA
/// </summary>
public class DefaultRouterTest
{
    private DefaultRouter _router = new();

    [SetUp]
    public void Setup() => _router = new DefaultRouter();

    [Test]
    public void Should_Invoke_Navigating_And_Navigated_Events_When_GoTo_Is_Called()
    {
        // == Arrange ==
        NavigationEventArgs? navigatingArgs = null;
        NavigationEventArgs? navigatedArgs = null;

        _router.Navigating += (s, e) => navigatingArgs = e;
        _router.Navigated += (s, e) => navigatedArgs = e;

        // == Act ==
        _router.GoTo("Movies");

        // == Assert ==
        // Navigating event
        Assert.That(navigatingArgs, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(navigatingArgs!.From, Is.EqualTo("Home"));
            Assert.That(navigatingArgs.To, Is.EqualTo("Movies"));

            // Navigated event
            Assert.That(navigatedArgs, Is.Not.Null);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(navigatedArgs!.From, Is.EqualTo("Home"));
            Assert.That(navigatedArgs.To, Is.EqualTo("Movies"));
        }
    }

    [Test]
    public void Should_Update_CurrentPageName_After_Navigation()
    {
        // == Act ==
        _router.GoTo("Movies");
        _router.GoTo("Bookings");

        // == Assert ==
        string? lastPage = null;
        _router.Navigated += (_, e) => lastPage = e.To;

        _router.GoTo("Home");

        Assert.That(lastPage, Is.EqualTo("Home"));
    }

    [Test]
    public void Should_Trigger_Events_For_Multiple_Navigations()
    {
        // == Arrange ==
        var navigatingCount = 0;
        var navigatedCount = 0;

        _router.Navigating += (_, _) => navigatingCount++;
        _router.Navigated += (_, _) => navigatedCount++;

        // == Act ==
        _router.GoTo("Page1");
        _router.GoTo("Page2");
        _router.GoTo("Page3");

        using (Assert.EnterMultipleScope())
        {
            // == Assert ==
            Assert.That(navigatingCount, Is.EqualTo(3));
            Assert.That(navigatedCount, Is.EqualTo(3));
        }
    }
}
