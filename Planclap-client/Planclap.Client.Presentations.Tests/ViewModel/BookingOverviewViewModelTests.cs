using NSubstitute;
using Planclap.Client.Domains.core;
using Planclap.Client.Domains.IEntities;
using Planclap.Client.Presentations.IViewModel;
using Planclap.Client.Presentations.PresentationServices;
using Planclap.Client.Presentations.Routers;
using Planclap.Client.Presentations.ViewModel;

namespace Planclap.Client.Presentations.Tests.ViewModel;

/// <summary>
///     Classe générée par IA
/// </summary>
[TestFixture]
public class BookingOverviewViewModelTests
{
    [SetUp]
    public void SetUp()
    {
        _service = Substitute.For<IBookingOverviewService>();
        _seatManagement = Substitute.For<ISeatManagement>();

        // Default setup
        _service.RoomWidth.Returns(10);
        _service.RoomDepth.Returns(15);
        _seatManagement.AllSeats.Returns(new List<ISeatViewModel>());
        _seatManagement.SelectedSeats.Returns(new List<ISeatViewModel>());
        _seatManagement.SelectedCount.Returns(0);
    }

    private IBookingOverviewService _service = null!;
    private ISeatManagement _seatManagement = null!;
    private BookingOverviewViewModel _viewModel = null!;

    // === CONSTRUCTOR TESTS ===

    [Test]
    public void Should_SubscribeToServiceEvents_When_Constructed_Given_ValidDependencies()
    {
        // Arrange
        Action<OnMovieClickEvent>? capturedAction = null;
        _service.When(x => x.OnMovieClickedEvent(Arg.Any<Action<OnMovieClickEvent>>()))
            .Do(x => capturedAction = x.Arg<Action<OnMovieClickEvent>>());

        // Act
        _viewModel = new BookingOverviewViewModel(_service, _seatManagement);

        // Assert
        _service.Received(1).OnMovieClickedEvent(Arg.Any<Action<OnMovieClickEvent>>());
        Assert.That(capturedAction, Is.Not.Null);
    }

    [Test]
    public void Should_SubscribeToSeatManagementEvents_When_Constructed_Given_ValidDependencies()
    {
        // Act
        _viewModel = new BookingOverviewViewModel(_service, _seatManagement);

        // Assert
        _seatManagement.Received().SelectionChanged += Arg.Any<Action>();
    }

    [Test]
    public void Should_InitializeCommand_When_Constructed_Given_ValidDependencies()
    {
        // Act
        _viewModel = new BookingOverviewViewModel(_service, _seatManagement);

        // Assert
        Assert.That(_viewModel.OnCreateBookingRequested, Is.Not.Null);
    }

    // === PROPERTY TESTS ===

    [Test]
    public void Should_ReturnServiceRoomWidth_When_AccessingRows_Given_ServiceConfigured()
    {
        // Arrange
        _service.RoomWidth.Returns(12);
        _viewModel = new BookingOverviewViewModel(_service, _seatManagement);

        // Act
        var result = _viewModel.Rows;

        // Assert
        Assert.That(result, Is.EqualTo(12));
    }

    [Test]
    public void Should_ReturnServiceRoomDepth_When_AccessingColumns_Given_ServiceConfigured()
    {
        // Arrange
        _service.RoomDepth.Returns(20);
        _viewModel = new BookingOverviewViewModel(_service, _seatManagement);

        // Act
        var result = _viewModel.Columns;

        // Assert
        Assert.That(result, Is.EqualTo(20));
    }

    [Test]
    public void Should_ReturnReservationTitle_When_Accessed_Given_ViewModelInitialized()
    {
        // Arrange
        _viewModel = new BookingOverviewViewModel(_service, _seatManagement);

        // Act
        var result = _viewModel.ReservationTitle;

        // Assert
        Assert.That(result, Is.EqualTo("Réservation"));
    }

    [Test]
    public void Should_ReturnAllSeats_When_AccessingSeats_Given_SeatManagementConfigured()
    {
        // Arrange
        var expectedSeats = new List<ISeatViewModel> { Substitute.For<ISeatViewModel>(), Substitute.For<ISeatViewModel>() };
        _seatManagement.AllSeats.Returns(expectedSeats);
        _viewModel = new BookingOverviewViewModel(_service, _seatManagement);

        // Act
        var result = _viewModel.Seats;

        // Assert
        Assert.That(result, Is.EqualTo(expectedSeats));
    }

    // === REFRESH SEATS TESTS ===

    [Test]
    public void Should_RefreshSeatsAndSetLastSlug_When_MovieClickedEventReceived_Given_ValidSlug()
    {
        // Arrange
        var slug = new MovieSlug("test-movie");
        var scheduled = Substitute.For<IScheduled>();
        var movieSession = new MovieSession(scheduled, MovieSupplier.Supply("test-movie", "Test movie"));
        var seatViewModels = new List<ISeatViewModel> { Substitute.For<ISeatViewModel>() };

        _service[slug].Returns(movieSession);
        _seatManagement.MapToViewModels(scheduled, 10, 15).Returns(seatViewModels);

        Action<OnMovieClickEvent>? capturedAction = null;
        _service.When(x => x.OnMovieClickedEvent(Arg.Any<Action<OnMovieClickEvent>>()))
            .Do(x => capturedAction = x.Arg<Action<OnMovieClickEvent>>());

        _viewModel = new BookingOverviewViewModel(_service, _seatManagement);

        // Act
        capturedAction?.Invoke(new OnMovieClickEvent { Slug = slug });

        // Assert
        _seatManagement.Received(1).MapToViewModels(scheduled, 10, 15);
        _seatManagement.Received(1).SetSeats(
            seatViewModels,
            Arg.Any<Action<ISeatViewModel>>(),
            Arg.Any<Func<ISeatViewModel, bool>>()
        );
    }

    [Test]
    public void Should_NotRefreshSeats_When_MovieClickedEventReceived_Given_NullSlug()
    {
        // Arrange
        Action<OnMovieClickEvent>? capturedAction = null;
        _service.When(x => x.OnMovieClickedEvent(Arg.Any<Action<OnMovieClickEvent>>()))
            .Do(x => capturedAction = x.Arg<Action<OnMovieClickEvent>>());

        _viewModel = new BookingOverviewViewModel(_service, _seatManagement);

        // Act
        capturedAction?.Invoke(new OnMovieClickEvent { Slug = null });

        // Assert
        _seatManagement.DidNotReceive().MapToViewModels(
            Arg.Any<IScheduled>(),
            Arg.Any<int>(),
            Arg.Any<int>()
        );
        _seatManagement.DidNotReceive().SetSeats(
            Arg.Any<IReadOnlyList<ISeatViewModel>>(),
            Arg.Any<Action<ISeatViewModel>>(),
            Arg.Any<Func<ISeatViewModel, bool>>()
        );
    }

    [Test]
    public void Should_UpdateLastSelectedSlug_When_MovieClickedEventReceived_Given_DifferentSlug()
    {
        // Arrange
        var firstSlug = new MovieSlug("first-movie");
        var secondSlug = new MovieSlug("second-movie");
        var scheduled1 = Substitute.For<IScheduled>();
        var scheduled2 = Substitute.For<IScheduled>();
        var movieSession1 = new MovieSession(scheduled1, MovieSupplier.Supply("first-movie", "First movie"));
        var movieSession2 = new MovieSession(scheduled2, MovieSupplier.Supply("second-movie", "Second movie"));
        var seatViewModels = new List<ISeatViewModel>();

        _service[firstSlug].Returns(movieSession1);
        _service[secondSlug].Returns(movieSession2);
        _service.IsMovieAvailable(Arg.Any<MovieSlug>()).Returns(true);
        _seatManagement.MapToViewModels(Arg.Any<IScheduled>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns(seatViewModels);
        _seatManagement.SelectedCount.Returns(1);

        Action<OnMovieClickEvent>? capturedAction = null;
        _service.When(x => x.OnMovieClickedEvent(Arg.Any<Action<OnMovieClickEvent>>()))
            .Do(x => capturedAction = x.Arg<Action<OnMovieClickEvent>>());

        _viewModel = new BookingOverviewViewModel(_service, _seatManagement);

        // Act
        capturedAction?.Invoke(new OnMovieClickEvent { Slug = firstSlug });
        capturedAction?.Invoke(new OnMovieClickEvent { Slug = secondSlug });

        // Assert
        var canExecute = _viewModel.OnCreateBookingRequested.CanExecute(null);
        Assert.That(canExecute, Is.True); // Vérifie que le dernier slug est utilisé
        _service.Received(1).IsMovieAvailable(secondSlug);
    }

    // === COMMAND TESTS - CanExecute ===

    [Test]
    public void Should_ReturnFalse_When_CanExecuteBooking_Given_NoSeatsSelected()
    {
        // Arrange
        _seatManagement.SelectedCount.Returns(0);
        _viewModel = new BookingOverviewViewModel(_service, _seatManagement);

        // Act
        var canExecute = _viewModel.OnCreateBookingRequested.CanExecute(null);

        // Assert
        Assert.That(canExecute, Is.False);
    }

    [Test]
    public void Should_ReturnFalse_When_CanExecuteBooking_Given_NoMovieSelected()
    {
        // Arrange
        _seatManagement.SelectedCount.Returns(2);
        _viewModel = new BookingOverviewViewModel(_service, _seatManagement);

        // Act
        var canExecute = _viewModel.OnCreateBookingRequested.CanExecute(null);

        // Assert
        Assert.That(canExecute, Is.False);
    }

    [Test]
    public void Should_ReturnFalse_When_CanExecuteBooking_Given_MovieNotAvailable()
    {
        // Arrange
        var slug = new MovieSlug("test-movie");
        var scheduled = Substitute.For<IScheduled>();
        var movieSession = new MovieSession(scheduled, MovieSupplier.Supply("test-movie", "Test movie"));
        var seatViewModels = new List<ISeatViewModel>();

        _service[slug].Returns(movieSession);
        _service.IsMovieAvailable(slug).Returns(false);
        _seatManagement.MapToViewModels(Arg.Any<IScheduled>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns(seatViewModels);
        _seatManagement.SelectedCount.Returns(2);

        Action<OnMovieClickEvent>? capturedAction = null;
        _service.When(x => x.OnMovieClickedEvent(Arg.Any<Action<OnMovieClickEvent>>()))
            .Do(x => capturedAction = x.Arg<Action<OnMovieClickEvent>>());

        _viewModel = new BookingOverviewViewModel(_service, _seatManagement);
        capturedAction?.Invoke(new OnMovieClickEvent { Slug = slug });

        // Act
        var canExecute = _viewModel.OnCreateBookingRequested.CanExecute(null);

        // Assert
        Assert.That(canExecute, Is.False);
    }

    [Test]
    public void Should_ReturnTrue_When_CanExecuteBooking_Given_ValidSelectionAndAvailableMovie()
    {
        // Arrange
        var slug = new MovieSlug("test-movie");
        var scheduled = Substitute.For<IScheduled>();
        var movieSession = new MovieSession(scheduled, MovieSupplier.Supply("test-movie", "Test movie"));
        var seatViewModels = new List<ISeatViewModel>();

        _service[slug].Returns(movieSession);
        _service.IsMovieAvailable(slug).Returns(true);
        _seatManagement.MapToViewModels(Arg.Any<IScheduled>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns(seatViewModels);
        _seatManagement.SelectedCount.Returns(2);

        Action<OnMovieClickEvent>? capturedAction = null;
        _service.When(x => x.OnMovieClickedEvent(Arg.Any<Action<OnMovieClickEvent>>()))
            .Do(x => capturedAction = x.Arg<Action<OnMovieClickEvent>>());

        _viewModel = new BookingOverviewViewModel(_service, _seatManagement);
        capturedAction?.Invoke(new OnMovieClickEvent { Slug = slug });

        // Act
        var canExecute = _viewModel.OnCreateBookingRequested.CanExecute(null);

        // Assert
        Assert.That(canExecute, Is.True);
    }

    // === COMMAND TESTS - Execute ===

    [Test]
    public void Should_NavigateToPayment_When_ExecutingCommand_Given_ValidSelection()
    {
        // Arrange
        var slug = new MovieSlug("test-movie");
        var scheduled = Substitute.For<IScheduled>();
        var movieSession = new MovieSession(scheduled, MovieSupplier.Supply("test-movie", "Test movie"));
        var seatViewModels = new List<ISeatViewModel> { Substitute.For<ISeatViewModel>() };
        var selectedSeats = new List<ISeatViewModel> { Substitute.For<ISeatViewModel>() };
        var tickets = new List<ITicket> { Substitute.For<ITicket>() };

        _service[slug].Returns(movieSession);
        _service.IsMovieAvailable(slug).Returns(true);
        _seatManagement.MapToViewModels(Arg.Any<IScheduled>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns(seatViewModels);
        _seatManagement.SelectedSeats.Returns(selectedSeats);
        _seatManagement.SelectedCount.Returns(1);
        _seatManagement.MapToTickets(selectedSeats).Returns(tickets);

        Action<OnMovieClickEvent>? capturedAction = null;
        _service.When(x => x.OnMovieClickedEvent(Arg.Any<Action<OnMovieClickEvent>>()))
            .Do(x => capturedAction = x.Arg<Action<OnMovieClickEvent>>());

        _viewModel = new BookingOverviewViewModel(_service, _seatManagement);
        capturedAction?.Invoke(new OnMovieClickEvent { Slug = slug });

        // Act
        _viewModel.OnCreateBookingRequested.Execute(null);

        // Assert
        _service.Received(1).GoToPayBookingForm(slug, tickets);
    }

    [Test]
    public void Should_ClearSeatManagement_When_ExecutingCommand_Given_ValidSelection()
    {
        // Arrange
        var slug = new MovieSlug("test-movie");
        var scheduled = Substitute.For<IScheduled>();
        var movieSession = new MovieSession(scheduled, MovieSupplier.Supply("test-movie", "Test movie"));
        var seatViewModels = new List<ISeatViewModel>();
        var selectedSeats = new List<ISeatViewModel> { Substitute.For<ISeatViewModel>() };
        var tickets = new List<ITicket> { Substitute.For<ITicket>() };

        _service[slug].Returns(movieSession);
        _seatManagement.MapToViewModels(Arg.Any<IScheduled>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns(seatViewModels);
        _seatManagement.SelectedSeats.Returns(selectedSeats);
        _seatManagement.SelectedCount.Returns(1);
        _seatManagement.MapToTickets(selectedSeats).Returns(tickets);

        Action<OnMovieClickEvent>? capturedAction = null;
        _service.When(x => x.OnMovieClickedEvent(Arg.Any<Action<OnMovieClickEvent>>()))
            .Do(x => capturedAction = x.Arg<Action<OnMovieClickEvent>>());

        _viewModel = new BookingOverviewViewModel(_service, _seatManagement);
        capturedAction?.Invoke(new OnMovieClickEvent { Slug = slug });

        // Act
        _viewModel.OnCreateBookingRequested.Execute(null);

        // Assert
        _seatManagement.Received(1).Clear();
    }

    [Test]
    public void Should_DoNothing_When_ExecutingCommand_Given_NoMovieSelected()
    {
        // Arrange
        _viewModel = new BookingOverviewViewModel(_service, _seatManagement);

        // Act
        _viewModel.OnCreateBookingRequested.Execute(null);

        // Assert
        _service.DidNotReceive().GoToPayBookingForm(
            Arg.Any<MovieSlug>(),
            Arg.Any<IList<ITicket>>()
        );
        _seatManagement.DidNotReceive().Clear();
    }

    // === SEAT MANAGEMENT EVENTS ===

    [Test]
    public void Should_RaiseCanExecuteChanged_When_SelectionChanged_Given_SeatManagementEvent()
    {
        // Arrange
        var canExecuteChangedRaised = false;
        _viewModel = new BookingOverviewViewModel(_service, _seatManagement);
        _viewModel.OnCreateBookingRequested.CanExecuteChanged += (_, _) => canExecuteChangedRaised = true;

        // Act
        _seatManagement.SelectionChanged += Raise.Event<Action>();

        // Assert
        Assert.That(canExecuteChangedRaised, Is.True);
    }

    [Test]
    public void Should_CallToggleSelection_When_SeatClicked_Given_SetSeatsConfigured()
    {
        // Arrange
        var slug = new MovieSlug("test-movie");
        var scheduled = Substitute.For<IScheduled>();
        var movieSession = new MovieSession(scheduled, MovieSupplier.Supply("test-movie", "Test movie"));
        var seatViewModels = new List<ISeatViewModel> { Substitute.For<ISeatViewModel>() };
        var clickedSeat = Substitute.For<ISeatViewModel>();

        _service[slug].Returns(movieSession);
        _seatManagement.MapToViewModels(Arg.Any<IScheduled>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns(seatViewModels);

        Action<ISeatViewModel>? capturedClickAction = null;
        _seatManagement.When(x => x.SetSeats(
                Arg.Any<IReadOnlyList<ISeatViewModel>>(),
                Arg.Any<Action<ISeatViewModel>>(),
                Arg.Any<Func<ISeatViewModel, bool>>()))
            .Do(x => capturedClickAction = x.Arg<Action<ISeatViewModel>>());

        Action<OnMovieClickEvent>? capturedMovieAction = null;
        _service.When(x => x.OnMovieClickedEvent(Arg.Any<Action<OnMovieClickEvent>>()))
            .Do(x => capturedMovieAction = x.Arg<Action<OnMovieClickEvent>>());

        _viewModel = new BookingOverviewViewModel(_service, _seatManagement);
        capturedMovieAction?.Invoke(new OnMovieClickEvent { Slug = slug });

        // Act
        capturedClickAction?.Invoke(clickedSeat);

        // Assert
        _seatManagement.Received(1).ToggleSelection(clickedSeat);
    }

    [Test]
    public void Should_AllowClick_When_SeatIsAvailable_Given_SetSeatsConfigured()
    {
        // Arrange
        var slug = new MovieSlug("test-movie");
        var scheduled = Substitute.For<IScheduled>();
        var movieSession = new MovieSession(scheduled, MovieSupplier.Supply("test-movie", "Test movie"));
        var seatViewModels = new List<ISeatViewModel>();
        var availableSeat = Substitute.For<ISeatViewModel>();
        availableSeat.IsAvailable.Returns(true);

        _service[slug].Returns(movieSession);
        _seatManagement.MapToViewModels(Arg.Any<IScheduled>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns(seatViewModels);

        Func<ISeatViewModel, bool>? capturedCanClick = null;
        _seatManagement.When(x => x.SetSeats(
                Arg.Any<IReadOnlyList<ISeatViewModel>>(),
                Arg.Any<Action<ISeatViewModel>>(),
                Arg.Any<Func<ISeatViewModel, bool>>()))
            .Do(x => capturedCanClick = x.Arg<Func<ISeatViewModel, bool>>());

        Action<OnMovieClickEvent>? capturedMovieAction = null;
        _service.When(x => x.OnMovieClickedEvent(Arg.Any<Action<OnMovieClickEvent>>()))
            .Do(x => capturedMovieAction = x.Arg<Action<OnMovieClickEvent>>());

        _viewModel = new BookingOverviewViewModel(_service, _seatManagement);
        capturedMovieAction?.Invoke(new OnMovieClickEvent { Slug = slug });

        // Act
        var canClick = capturedCanClick?.Invoke(availableSeat);

        // Assert
        Assert.That(canClick, Is.True);
    }

    [Test]
    public void Should_DisallowClick_When_SeatIsNotAvailable_Given_SetSeatsConfigured()
    {
        // Arrange
        var slug = new MovieSlug("test-movie");
        var scheduled = Substitute.For<IScheduled>();
        var movieSession = new MovieSession(scheduled, MovieSupplier.Supply("test-movie", "Test movie"));
        var seatViewModels = new List<ISeatViewModel>();
        var unavailableSeat = Substitute.For<ISeatViewModel>();
        unavailableSeat.IsAvailable.Returns(false);

        _service[slug].Returns(movieSession);
        _seatManagement.MapToViewModels(Arg.Any<IScheduled>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns(seatViewModels);

        Func<ISeatViewModel, bool>? capturedCanClick = null;
        _seatManagement.When(x => x.SetSeats(
                Arg.Any<IReadOnlyList<ISeatViewModel>>(),
                Arg.Any<Action<ISeatViewModel>>(),
                Arg.Any<Func<ISeatViewModel, bool>>()))
            .Do(x => capturedCanClick = x.Arg<Func<ISeatViewModel, bool>>());

        Action<OnMovieClickEvent>? capturedMovieAction = null;
        _service.When(x => x.OnMovieClickedEvent(Arg.Any<Action<OnMovieClickEvent>>()))
            .Do(x => capturedMovieAction = x.Arg<Action<OnMovieClickEvent>>());

        _viewModel = new BookingOverviewViewModel(_service, _seatManagement);
        capturedMovieAction?.Invoke(new OnMovieClickEvent { Slug = slug });

        // Act
        var canClick = capturedCanClick?.Invoke(unavailableSeat);

        // Assert
        Assert.That(canClick, Is.False);
    }

    // === NAVIGATION TESTS ===

    [Test]
    public void Should_RefreshSeats_When_NavigatingToHome_Given_MovieWasSelected()
    {
        // Arrange
        var slug = new MovieSlug("test-movie");
        var scheduled = Substitute.For<IScheduled>();
        var movieSession = new MovieSession(scheduled, MovieSupplier.Supply("test-movie", "Test movie"));
        var seatViewModels = new List<ISeatViewModel>();

        _service[slug].Returns(movieSession);
        _seatManagement.MapToViewModels(Arg.Any<IScheduled>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns(seatViewModels);

        Action<OnMovieClickEvent>? capturedMovieAction = null;
        _service.When(x => x.OnMovieClickedEvent(Arg.Any<Action<OnMovieClickEvent>>()))
            .Do(x => capturedMovieAction = x.Arg<Action<OnMovieClickEvent>>());

        _viewModel = new BookingOverviewViewModel(_service, _seatManagement);
        capturedMovieAction?.Invoke(new OnMovieClickEvent { Slug = slug });

        _seatManagement.ClearReceivedCalls();

        // Act
        _service.Navigated += Raise.EventWith(
            _service,
            new NavigationEventArgs { From = "Payment", To = "Home" }
        );

        // Assert
        _seatManagement.Received(1).MapToViewModels(scheduled, 10, 15);
        _seatManagement.Received(1).SetSeats(
            Arg.Any<IReadOnlyList<ISeatViewModel>>(),
            Arg.Any<Action<ISeatViewModel>>(),
            Arg.Any<Func<ISeatViewModel, bool>>()
        );
    }

    [Test]
    public void Should_NotRefreshSeats_When_NavigatingToOtherPage_Given_MovieWasSelected()
    {
        // Arrange
        var slug = new MovieSlug("test-movie");
        var scheduled = Substitute.For<IScheduled>();
        var movieSession = new MovieSession(scheduled, MovieSupplier.Supply("test-movie", "Test movie"));
        var seatViewModels = new List<ISeatViewModel>();

        _service[slug].Returns(movieSession);
        _seatManagement.MapToViewModels(Arg.Any<IScheduled>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns(seatViewModels);

        Action<OnMovieClickEvent>? capturedMovieAction = null;
        _service.When(x => x.OnMovieClickedEvent(Arg.Any<Action<OnMovieClickEvent>>()))
            .Do(x => capturedMovieAction = x.Arg<Action<OnMovieClickEvent>>());

        _viewModel = new BookingOverviewViewModel(_service, _seatManagement);
        capturedMovieAction?.Invoke(new OnMovieClickEvent { Slug = slug });

        _seatManagement.ClearReceivedCalls();

        // Act
        _service.Navigated += Raise.EventWith(
            _service,
            new NavigationEventArgs { From = "Home", To = "Payment" }
        );

        // Assert
        _seatManagement.DidNotReceive().MapToViewModels(
            Arg.Any<IScheduled>(),
            Arg.Any<int>(),
            Arg.Any<int>()
        );
    }

    [Test]
    public void Should_NotRefreshSeats_When_NavigatingToHome_Given_NoMovieWasSelected()
    {
        // Arrange
        _viewModel = new BookingOverviewViewModel(_service, _seatManagement);

        // Act
        _service.Navigated += Raise.EventWith(
            _service,
            new NavigationEventArgs { From = "Payment", To = "Home" }
        );

        // Assert
        _seatManagement.DidNotReceive().MapToViewModels(
            Arg.Any<IScheduled>(),
            Arg.Any<int>(),
            Arg.Any<int>()
        );
    }
}
