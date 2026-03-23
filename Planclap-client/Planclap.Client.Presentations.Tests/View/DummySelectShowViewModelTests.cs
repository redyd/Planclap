using Planclap.Client.Presentations.View;

namespace Planclap.Client.Presentations.Tests.View;

/// <summary>
///     Classe générée par IA
/// </summary>
public class DummySelectShowViewModelTests
{
    [Test]
    public void Should_ReturnFixedTitle_When_AccessingTitle_Given_DefaultInitialization()
    {
        // Arrange
        var sut = new DummySelectShowViewModel();

        // Act
        var title = sut.Title;

        // Assert
        Assert.That(title, Is.EqualTo("Les films d'aujourd'hui"));
    }

    [Test]
    public void Should_ReturnNonEmptyShowList_When_AccessingShows_Given_DefaultInitialization()
    {
        // Arrange
        var sut = new DummySelectShowViewModel();

        // Act
        var shows = sut.Shows;

        // Assert
        Assert.That(shows, Is.Not.Null);
        Assert.That(shows, Is.Not.Empty);
    }

    [Test]
    public void Should_ContainExpectedNumberOfShows_When_AccessingShows_Given_DefaultInitialization()
    {
        // Arrange
        var sut = new DummySelectShowViewModel();

        // Act
        var shows = sut.Shows;

        // Assert
        Assert.That(shows, Has.Count.EqualTo(5));
    }

    [Test]
    public void Should_ContainShowsWithExpectedPosterUri_When_AccessingShows_Given_DefaultInitialization()
    {
        // Arrange
        var sut = new DummySelectShowViewModel();
        var expectedUri = new Uri("https://theposterdb.com/api/assets/468365/view");

        // Act
        var posters = sut.Shows.Select(s => s.Poster);

        // Assert
        foreach (var poster in posters)
        {
            Assert.That(poster, Is.EqualTo(expectedUri));
        }
    }

    [Test]
    public void Should_HaveCorrectTitles_When_AccessingShows_Given_DefaultInitialization()
    {
        // Arrange
        var sut = new DummySelectShowViewModel();

        // Act
        var titles = sut.Shows.Select(s => s.Title).ToList();

        // Assert
        Assert.That(titles, Is.EqualTo([
            "La Nuit des Étoiles",
            "Les Gardiens du Temps",
            "Horizons Perdus",
            "Mission Ultime",
            "Rêves d’Aurore"
        ]));
    }

    [Test]
    public void Should_RespectCanBuyValues_When_AccessingShows_Given_DefaultInitialization()
    {
        // Arrange
        var sut = new DummySelectShowViewModel();

        // Act
        var flags = sut.Shows.Select(s => s.CanBuy).ToList();

        // Assert
        Assert.That(flags, Is.EqualTo([
            false,
            false,
            true,
            true,
            true
        ]));
    }

    [Test]
    public void Should_SetSelectedShowToNull_When_InitializeIsCalled_Given_AnyPreviousSelection()
    {
        // Arrange
        var sut = new DummySelectShowViewModel();
        sut.SelectedShow = sut.Shows[3];

        // Act
        sut.Initialize();

        // Assert
        Assert.That(sut.SelectedShow, Is.Null);
    }

    [Test]
    public void Should_AllowSelectingAShow_When_SelectedShowIsSet_Given_ValidShow()
    {
        // Arrange
        var sut = new DummySelectShowViewModel();
        var show = sut.Shows[2];

        // Act
        sut.SelectedShow = show;

        // Assert
        Assert.That(sut.SelectedShow, Is.EqualTo(show));
    }
}
