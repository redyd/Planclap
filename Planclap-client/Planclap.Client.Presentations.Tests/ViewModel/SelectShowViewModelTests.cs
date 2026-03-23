using System.Collections.ObjectModel;
using NSubstitute;
using Planclap.Client.Domains.core;
using Planclap.Client.Domains.IEntities;
using Planclap.Client.Presentations.Dtos;
using Planclap.Client.Presentations.PresentationServices;
using Planclap.Client.Presentations.ViewModel;

namespace Planclap.Client.Presentations.Tests.ViewModel;

/// <summary>
///     Classe générée par IA
/// </summary>
[TestFixture]
public class SelectShowViewModelTests
{
    // === SETUP ===

    [SetUp]
    public void SetUp()
    {
        _service = Substitute.For<ISelectShowService>();
        _planning = Substitute.For<IPlanning>();

        _movieSessions = new List<MovieSession>();

        _planning.Movies.Returns(_movieSessions);
        _service.Current.Returns(new DateTime(2024, 01, 10, 12, 00, 00));
    }

    private ISelectShowService _service = null!;
    private IPlanning _planning = null!;
    private SelectShowViewModel _viewModel = null!;

    private List<MovieSession> _movieSessions = null!;

    private static Movie CreateMovie(string title)
        => MovieSupplier.Supply(title: title);

    private static IScheduled CreateScheduled(DateTime start, TimeSpan duration)
    {
        var sched = Substitute.For<IScheduled>();
        sched.StartTime.Returns(start);
        sched.Duration.Returns(duration);
        return sched;
    }

    private void BuildViewModel()
        => _viewModel = new SelectShowViewModel(_service, _planning);

    // === CONSTRUCTOR TESTS ===

    [Test]
    public void Should_MapPlanningMovies_When_Constructed_Given_ValidPlanning()
    {
        // Arrange
        var m1 = new MovieSession(
            CreateScheduled(new DateTime(2024, 01, 10, 14, 0, 0), TimeSpan.FromMinutes(120)),
            CreateMovie("Film 1"));

        _movieSessions.Add(m1);

        // Act
        BuildViewModel();

        // Assert
        Assert.That(_viewModel.Shows, Has.Count.EqualTo(1));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(_viewModel.Shows[0].Title, Is.EqualTo("Film 1"));
            Assert.That(_viewModel.Shows[0].CanBuy, Is.True);
        }
    }

    [Test]
    public void Should_SetTitleFromServiceDate_When_Constructed_Given_ServiceCurrentDate()
    {
        // Arrange
        _service.Current.Returns(new DateTime(2024, 01, 10));

        // Act
        BuildViewModel();

        // Assert
        Assert.That(_viewModel.Title, Is.EqualTo("Les séances du 10 janvier 2024"));
    }

    // === PROPERTY SelectedShow ===

    [Test]
    public void Should_CallServiceOnMovieClick_When_SelectedShowChanges_Given_NewNonNullValue()
    {
        // Arrange
        var dto = new ShowOverviewDto(new Uri("http://a"), "Film");
        BuildViewModel();

        // Act
        _viewModel.SelectedShow = dto;

        // Assert
        _service.Received(1).OnMovieClick(dto);
    }

    [Test]
    public void Should_NotCallServiceOnMovieClick_When_SelectedShowChanges_Given_Null()
    {
        // Arrange
        BuildViewModel();

        // Act
        _viewModel.SelectedShow = null;

        // Assert
        _service.DidNotReceive().OnMovieClick(Arg.Any<ShowOverviewDto>());
    }

    [Test]
    public void Should_RaisePropertyChanged_When_SelectedShowChanges_Given_DifferentValue()
    {
        // Arrange
        BuildViewModel();
        var dto = new ShowOverviewDto(new Uri("http://a"));

        string? changed = null;
        _viewModel.PropertyChanged += (_, e) => changed = e.PropertyName;

        // Act
        _viewModel.SelectedShow = dto;

        // Assert
        Assert.That(changed, Is.EqualTo(nameof(SelectShowViewModel.SelectedShow)));
    }

    [Test]
    public void Should_NotRaisePropertyChanged_When_SelectedShowAssignedSameValue_Given_NoChange()
    {
        // Arrange
        BuildViewModel();
        var dto = new ShowOverviewDto(new Uri("http://a"));
        _viewModel.SelectedShow = dto;

        var raised = false;
        _viewModel.PropertyChanged += (_, _) => raised = true;

        // Act
        _viewModel.SelectedShow = dto;

        // Assert
        Assert.That(raised, Is.False);
    }

    // === INITIALIZATION TESTS ===

    [Test]
    public void Should_SelectFirstAvailableShow_When_InitializeCalled_Given_ShowsWithAvailableOne()
    {
        // Arrange: second is buyable
        _movieSessions.Add(new MovieSession(
            CreateScheduled(new DateTime(2024, 01, 10, 10, 0, 0), TimeSpan.FromMinutes(90)),
            CreateMovie("Film 1")));

        _movieSessions.Add(new MovieSession(
            CreateScheduled(new DateTime(2024, 01, 10, 14, 0, 0), TimeSpan.FromMinutes(120)),
            CreateMovie("Film 2")));

        BuildViewModel();

        // Act
        _viewModel.Initialize();

        // Assert
        Assert.That(_viewModel.SelectedShow!.Title, Is.EqualTo("Film 2"));
        _service.Received(1).OnMovieClick(_viewModel.SelectedShow);
    }

    [Test]
    public void Should_SelectLastShow_When_InitializeCalled_Given_NoShowIsBuyable()
    {
        // Arrange: both are in the past
        _service.Current.Returns(new DateTime(2024, 01, 10, 18, 0, 0));

        _movieSessions.Add(new MovieSession(
            CreateScheduled(new DateTime(2024, 01, 10, 09, 0, 0), TimeSpan.FromHours(1)),
            CreateMovie("Film 1")));

        _movieSessions.Add(new MovieSession(
            CreateScheduled(new DateTime(2024, 01, 10, 10, 0, 0), TimeSpan.FromHours(2)),
            CreateMovie("Film 2")));

        BuildViewModel();

        // Act
        _viewModel.Initialize();

        // Assert
        Assert.That(_viewModel.SelectedShow!.Title, Is.EqualTo("Film 2"));
        _service.Received(1).OnMovieClick(_viewModel.SelectedShow);
    }

    [Test]
    public void Should_NotSelectAnyShow_When_InitializeCalled_Given_NoShowExists()
    {
        // Arrange
        BuildViewModel();

        // Act
        _viewModel.Initialize();

        // Assert
        Assert.That(_viewModel.SelectedShow, Is.Null);
        _service.DidNotReceive().OnMovieClick(Arg.Any<ShowOverviewDto>());
    }

    // === FIRST AVAILABLE SHOW TESTS (indirect via Initialize) ===

    [Test]
    public void Should_ReturnNull_When_FirstAvailableShow_Given_EmptyList()
    {
        // Arrange
        BuildViewModel();

        // Act
        _viewModel.Initialize();

        // Assert
        Assert.That(_viewModel.SelectedShow, Is.Null);
    }

    // === SHOWS PROPERTY ===

    [Test]
    public void Should_RaisePropertyChanged_When_ShowsAssigned_Given_NewValue()
    {
        // Arrange
        BuildViewModel();

        string? changed = null;
        _viewModel.PropertyChanged += (_, e) => changed = e.PropertyName;

        // Act
        _viewModel.Shows = new ObservableCollection<ShowOverviewDto>();

        // Assert
        Assert.That(changed, Is.EqualTo(nameof(SelectShowViewModel.Shows)));
    }
}
