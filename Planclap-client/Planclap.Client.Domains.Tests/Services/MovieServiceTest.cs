using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Planclap.Client.Domains.core;
using Planclap.Client.Domains.Entities;
using Planclap.Client.Domains.Events;
using Planclap.Client.Domains.Exception;
using Planclap.Client.Domains.IEntities;
using Planclap.Client.Domains.IRepository;
using Planclap.Client.Domains.Services;

namespace Planclap.Client.Domains.Tests.Services;

/// <summary>
/// Classe refractoré par IA
/// </summary>
[TestFixture]
public class MovieServiceTest
{
    [SetUp]
    public void Setup()
    {
        _movieRepository = Substitute.For<IMovieRepository>();
        _planningRepository = Substitute.For<IPlanningRepository>();
        _planning = Substitute.For<IPlanning>();
    }

    private readonly DateTime _date = new(2025, 11, 13, 12, 0, 0);
    private IMovieRepository _movieRepository = Substitute.For<IMovieRepository>();
    private IPlanningRepository _planningRepository = Substitute.For<IPlanningRepository>();
    private IPlanning _planning = Substitute.For<IPlanning>();

    [Test]
    public void Should_Fetch_Into_Repository_On_Refresh()
    {
        // == Arrange ==
        var movieSlug = new MovieSlug("test-movie");
        var scheduled = new Scheduled(_date, TimeSpan.FromMinutes(75));
        var movie = new Movie(movieSlug,
            new MovieTitle("Test movie"),
            new MovieDescription("Description"),
            new CineChecksGroup(new CineCheckAge("al"), new List<CineCheck>()),
            new Uri("https://www.poster.com"));

        // return values for repo
        var planningReturn = new Dictionary<MovieSlug, IList<IScheduled>> { { movieSlug, new List<IScheduled> { scheduled } } };

        var movieReturn = new List<Movie> { movie };

        // repositories
        _planningRepository
            .FetchAllForToday()
            .Returns(planningReturn);

        _movieRepository
            .FetchAllBySlug(Arg.Is<HashSet<MovieSlug>>(s => s.Contains(movieSlug)))
            .Returns(movieReturn);

        // creates service
        var service = new MovieService(_movieRepository, _planningRepository, _planning);

        // == Act ==
        var result = service.FetchPlanning();

        // == Assert ==
        _planning.Received(1).ResetPlanning(Arg.Is<IList<MovieSession>>(list =>
            list.Count == 1 &&
            list[0].Movie == movie &&
            list[0].Scheduled == scheduled
        ));
        Assert.That(result, Is.EqualTo(_planning));
    }

    [Test]
    public void Should_Not_Fetch_Into_Repository_On_Refresh()
    {
        // creates service
        var service = new MovieService(_movieRepository, _planningRepository, _planning);

        // == Act ==
        var result = service.FetchPlanning(false);

        // == Assert ==
        _planning.Received(0).ResetPlanning(Arg.Any<IList<MovieSession>>());
        _planningRepository.Received(0).FetchAllForToday();
        _movieRepository.Received(0).FetchAllBySlug(Arg.Any<HashSet<MovieSlug>>());
        Assert.That(result, Is.EqualTo(_planning));
    }

    [Test]
    public void Should_Update_Planning_And_Repo_On_New_Reservation()
    {
        // == Arrange ==
        var movieSlug = new MovieSlug("test-movie");
        var scheduled = new Scheduled(_date, TimeSpan.FromMinutes(75));
        var fakeReservation = Substitute.For<IReservation>();
        fakeReservation.Title.Returns(new MovieTitle("Test movie"));

        // return values for repo
        var planningReturn = new Dictionary<MovieSlug, IList<IScheduled>> { { movieSlug, new List<IScheduled> { scheduled } } };

        // repositories
        _planningRepository
            .FetchAllForToday()
            .Returns(planningReturn);

        // creates service
        var service = new MovieService(_movieRepository, _planningRepository, _planning);

        // == Act ==
        service.NewReservation(fakeReservation);

        // == Assert ==
        _planning.Received(1).UpdateTickets(fakeReservation);
        _planningRepository.Received(1).UpdateScheduled(fakeReservation);
    }

    [Test]
    public void Should_Raise_Error_Event_When_FetchPlanning_Throws_RepositoryException()
    {
        // == Arrange ==
        _planningRepository
            .FetchAllForToday()
            .Throws(new RepositoryException("Test error"));

        var service = new MovieService(_movieRepository, _planningRepository, _planning);

        RepositoryErrorEventArgs? eventArgs = null;
        service.RepositoryErrorEvent += (_, args) => eventArgs = args;

        // == Act ==
        var result = service.FetchPlanning();

        // == Assert ==
        Assert.That(eventArgs, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(eventArgs!.Message, Is.EqualTo("Une erreur est survenue lors de la récupération des films"));
            Assert.That(result, Is.EqualTo(_planning));
        }
        _planning.Received(0).ResetPlanning(Arg.Any<IList<MovieSession>>());
    }

    [Test]
    public void Should_Raise_Error_Event_When_NewReservation_Throws_RepositoryException()
    {
        // == Arrange ==
        var fakeReservation = Substitute.For<IReservation>();

        _planningRepository
            .When(x => x.UpdateScheduled(fakeReservation))
            .Do(_ => throw new RepositoryException("Test error"));

        var service = new MovieService(_movieRepository, _planningRepository, _planning);

        RepositoryErrorEventArgs? eventArgs = null;
        service.RepositoryErrorEvent += (_, args) => eventArgs = args;

        // == Act ==
        service.NewReservation(fakeReservation);

        // == Assert ==
        Assert.That(eventArgs, Is.Not.Null);
        Assert.That(eventArgs!.Message, Is.EqualTo("Une erreur est survenue lors de la réservation"));
        _planning.Received(0).UpdateTickets(Arg.Any<IReservation>());
    }

    [Test]
    public void Should_Not_Raise_Error_Event_When_No_Exception_Occurs()
    {
        // == Arrange ==
        var movieSlug = new MovieSlug("test-movie");
        var scheduled = new Scheduled(_date, TimeSpan.FromMinutes(75));
        var movie = new Movie(movieSlug,
            new MovieTitle("Test movie"),
            new MovieDescription("Description"),
            new CineChecksGroup(new CineCheckAge("al"), new List<CineCheck>()),
            new Uri("https://www.poster.com"));

        var planningReturn = new Dictionary<MovieSlug, IList<IScheduled>> { { movieSlug, new List<IScheduled> { scheduled } } };
        var movieReturn = new List<Movie> { movie };

        _planningRepository.FetchAllForToday().Returns(planningReturn);
        _movieRepository.FetchAllBySlug(Arg.Any<HashSet<MovieSlug>>()).Returns(movieReturn);

        var service = new MovieService(_movieRepository, _planningRepository, _planning);

        var eventRaised = false;
        service.RepositoryErrorEvent += (_, _) => eventRaised = true;

        // == Act ==
        service.FetchPlanning();

        // == Assert ==
        Assert.That(eventRaised, Is.False);
    }
}
