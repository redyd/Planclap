using NSubstitute;
using Planclap.Client.Domains.core;
using Planclap.Client.Domains.IEntities;
using Planclap.Client.Presentations.Common;
using Planclap.Client.Presentations.PresentationServices;
using Planclap.Client.Presentations.Routers;
using Planclap.Client.Presentations.ViewModel;

namespace Planclap.Client.Presentations.Tests.ViewModel;

/// <summary>
///     Classe générée par IA
/// </summary>
[TestFixture]
public class PayBookingViewModelTests
{
    [SetUp]
    public void SetUp()
    {
        _service = Substitute.For<IPayBookingService>();
        _pageNotifier = Substitute.For<IPageNotifier>();
        _viewModel = new PayBookingViewModel(_service, _pageNotifier);
    }

    private IPayBookingService _service = null!;
    private IPageNotifier _pageNotifier = null!;
    private PayBookingViewModel _viewModel = null!;

    // === CONSTRUCTOR TESTS ===

    [Test]
    public void Should_SubscribeToPageNotifier_When_Constructed_Given_ValidDependencies() =>
        // Assert
        _pageNotifier.Received(1).Subscribe(
            Arg.Any<Action<OnPayBookingRequestedEvent>>()
        );

    [Test]
    public void Should_InitializeCommands_When_Constructed_Given_ValidDependencies()
    {
        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(_viewModel.OnCancel, Is.Not.Null);
            Assert.That(_viewModel.OnPay, Is.Not.Null);
        }
    }

    // === PROPERTY TESTS - Initial State ===

    [Test]
    public void Should_ReturnFalse_When_AccessingShowWarning_Given_NoReservation()
    {
        // Act
        var result = _viewModel.ShowWarning;

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void Should_ReturnZero_When_AccessingReduction_Given_NoReservation()
    {
        // Act
        var result = _viewModel.Reduction;

        // Assert
        Assert.That(result, Is.Zero);
    }

    [Test]
    public void Should_ReturnZero_When_AccessingPrice_Given_NoReservation()
    {
        // Act
        var result = _viewModel.Price;

        // Assert
        Assert.That(result, Is.Zero);
    }

    [Test]
    public void Should_ReturnEmptyCollection_When_AccessingInputs_Given_NoReservation()
    {
        // Act
        var result = _viewModel.Inputs;

        // Assert
        Assert.That(result, Is.Empty);
    }

    // === EVENT HANDLING - OnPayBookingRequestedEvent ===

    [Test]
    public void Should_InitializeInputs_When_EventReceived_Given_ValidReservation()
    {
        // Arrange
        var ticket1 = CreateTicket(5, 10);
        var ticket2 = CreateTicket(6, 11);
        var reservation = CreateReservation([ticket1, ticket2]);

        Action<OnPayBookingRequestedEvent>? capturedAction = null;
        _pageNotifier.When(x => x.Subscribe(
                Arg.Any<Action<OnPayBookingRequestedEvent>>()))
            .Do(x => capturedAction = x.Arg<Action<OnPayBookingRequestedEvent>>());

        _viewModel = new PayBookingViewModel(_service, _pageNotifier);

        // Act
        capturedAction?.Invoke(new OnPayBookingRequestedEvent { Reservation = reservation });

        // Assert
        Assert.That(_viewModel.Inputs, Has.Count.EqualTo(2));
    }

    [Test]
    public void Should_UpdateTitle_When_EventReceived_Given_ValidReservation()
    {
        // Arrange
        var ticket = CreateTicket(5, 10);
        var reservation = CreateReservation([ticket]);

        Action<OnPayBookingRequestedEvent>? capturedAction = null;
        _pageNotifier.When(x => x.Subscribe(
                Arg.Any<Action<OnPayBookingRequestedEvent>>()))
            .Do(x => capturedAction = x.Arg<Action<OnPayBookingRequestedEvent>>());

        _viewModel = new PayBookingViewModel(_service, _pageNotifier);

        // Act
        capturedAction?.Invoke(new OnPayBookingRequestedEvent { Reservation = reservation });

        // Assert
        Assert.That(_viewModel.Title, Is.EqualTo("Réservation pour Test Movie"));
    }

    [Test]
    public void Should_UpdateReduction_When_EventReceived_Given_ReservationWithReduction()
    {
        // Arrange
        var ticket = CreateTicket(5, 10);
        var reservation = CreateReservation([ticket]);
        reservation.Reduction.Returns(15);

        Action<OnPayBookingRequestedEvent>? capturedAction = null;
        _pageNotifier.When(x => x.Subscribe(
                Arg.Any<Action<OnPayBookingRequestedEvent>>()))
            .Do(x => capturedAction = x.Arg<Action<OnPayBookingRequestedEvent>>());

        _viewModel = new PayBookingViewModel(_service, _pageNotifier);

        // Act
        capturedAction?.Invoke(new OnPayBookingRequestedEvent { Reservation = reservation });

        // Assert
        Assert.That(_viewModel.Reduction, Is.EqualTo(15));
    }

    // === PROPERTY TESTS - With Reservation ===

    [Test]
    public void Should_ReturnTrue_When_AccessingShowWarning_Given_TicketBelowMinimumAge()
    {
        // Arrange
        var ticket = CreateTicket(5, 10, 12);
        ticket.ValidTicket.Returns(true);
        ticket.Age.Returns(12);

        var reservation = CreateReservation([ticket]);
        reservation.MinimumAge.Returns(16);

        Action<OnPayBookingRequestedEvent>? capturedAction = null;
        _pageNotifier.When(x => x.Subscribe(
                Arg.Any<Action<OnPayBookingRequestedEvent>>()))
            .Do(x => capturedAction = x.Arg<Action<OnPayBookingRequestedEvent>>());

        _viewModel = new PayBookingViewModel(_service, _pageNotifier);
        capturedAction?.Invoke(new OnPayBookingRequestedEvent { Reservation = reservation });

        // Act
        var result = _viewModel.ShowWarning;

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void Should_ReturnFalse_When_AccessingShowWarning_Given_TicketAboveMinimumAge()
    {
        // Arrange
        var ticket = CreateTicket(5, 10, 18);
        ticket.ValidTicket.Returns(true);
        ticket.Age.Returns(18);

        var reservation = CreateReservation([ticket]);
        reservation.MinimumAge.Returns(16);

        Action<OnPayBookingRequestedEvent>? capturedAction = null;
        _pageNotifier.When(x => x.Subscribe(
                Arg.Any<Action<OnPayBookingRequestedEvent>>()))
            .Do(x => capturedAction = x.Arg<Action<OnPayBookingRequestedEvent>>());

        _viewModel = new PayBookingViewModel(_service, _pageNotifier);
        capturedAction?.Invoke(new OnPayBookingRequestedEvent { Reservation = reservation });

        // Act
        var result = _viewModel.ShowWarning;

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void Should_ReturnFalse_When_AccessingShowWarning_Given_InvalidTicket()
    {
        // Arrange
        var ticket = CreateTicket(5, 10, 12);
        ticket.ValidTicket.Returns(false);
        ticket.Age.Returns(12);

        var reservation = CreateReservation([ticket]);
        reservation.MinimumAge.Returns(16);

        Action<OnPayBookingRequestedEvent>? capturedAction = null;
        _pageNotifier.When(x => x.Subscribe(
                Arg.Any<Action<OnPayBookingRequestedEvent>>()))
            .Do(x => capturedAction = x.Arg<Action<OnPayBookingRequestedEvent>>());

        _viewModel = new PayBookingViewModel(_service, _pageNotifier);
        capturedAction?.Invoke(new OnPayBookingRequestedEvent { Reservation = reservation });

        // Act
        var result = _viewModel.ShowWarning;

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void Should_ReturnWarningMessage_When_Accessed_Given_ViewModelInitialized()
    {
        // Act
        var result = _viewModel.WarningMessage;

        // Assert
        Assert.That(result, Does.Contain("Ce film ne convient pas à des enfants"));
        Assert.That(result, Does.Contain("Le cinéma décline toute responsabilité"));
    }

    [Test]
    public void Should_ReturnPrice_When_Accessed_Given_ReservationWithPrice()
    {
        // Arrange
        var ticket = CreateTicket(5, 10);
        var reservation = CreateReservation([ticket]);
        reservation.Price.Returns(25.50);

        Action<OnPayBookingRequestedEvent>? capturedAction = null;
        _pageNotifier.When(x => x.Subscribe(
                Arg.Any<Action<OnPayBookingRequestedEvent>>()))
            .Do(x => capturedAction = x.Arg<Action<OnPayBookingRequestedEvent>>());

        _viewModel = new PayBookingViewModel(_service, _pageNotifier);
        capturedAction?.Invoke(new OnPayBookingRequestedEvent { Reservation = reservation });

        // Act
        var result = _viewModel.Price;

        // Assert
        Assert.That(result, Is.EqualTo(25.50));
    }

    // === COMMAND TESTS - OnCancel ===

    [Test]
    public void Should_ClearInputs_When_CancelExecuted_Given_ReservationExists()
    {
        // Arrange
        var ticket = CreateTicket(5, 10);
        var reservation = CreateReservation([ticket]);

        Action<OnPayBookingRequestedEvent>? capturedAction = null;
        _pageNotifier.When(x => x.Subscribe(
                Arg.Any<Action<OnPayBookingRequestedEvent>>()))
            .Do(x => capturedAction = x.Arg<Action<OnPayBookingRequestedEvent>>());

        _viewModel = new PayBookingViewModel(_service, _pageNotifier);
        capturedAction?.Invoke(new OnPayBookingRequestedEvent { Reservation = reservation });

        // Act
        _viewModel.OnCancel.Execute(null);

        // Assert
        Assert.That(_viewModel.Inputs, Is.Empty);
    }

    [Test]
    public void Should_NavigateHome_When_CancelExecuted_Given_ViewModelInitialized()
    {
        // Act
        _viewModel.OnCancel.Execute(null);

        // Assert
        _service.Received(1).NavigateHome();
    }

    [Test]
    public void Should_ClearReservation_When_CancelExecuted_Given_ReservationExists()
    {
        // Arrange
        var ticket = CreateTicket(5, 10);
        var reservation = CreateReservation([ticket]);

        Action<OnPayBookingRequestedEvent>? capturedAction = null;
        _pageNotifier.When(x => x.Subscribe(
                Arg.Any<Action<OnPayBookingRequestedEvent>>()))
            .Do(x => capturedAction = x.Arg<Action<OnPayBookingRequestedEvent>>());

        _viewModel = new PayBookingViewModel(_service, _pageNotifier);
        capturedAction?.Invoke(new OnPayBookingRequestedEvent { Reservation = reservation });

        // Act
        _viewModel.OnCancel.Execute(null);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(_viewModel.Price, Is.Zero);
            Assert.That(_viewModel.Reduction, Is.Zero);
        }
    }

    // === COMMAND TESTS - OnPay CanExecute ===

    [Test]
    public void Should_ReturnFalse_When_CanExecutePay_Given_NoReservation()
    {
        // Act
        var canExecute = _viewModel.OnPay.CanExecute(null);

        // Assert
        Assert.That(canExecute, Is.False);
    }

    [Test]
    public void Should_ReturnFalse_When_CanExecutePay_Given_InvalidReservation()
    {
        // Arrange
        var ticket = CreateTicket(5, 10);
        var reservation = CreateReservation([ticket]);
        reservation.IsValid.Returns(false);

        Action<OnPayBookingRequestedEvent>? capturedAction = null;
        _pageNotifier.When(x => x.Subscribe(
                Arg.Any<Action<OnPayBookingRequestedEvent>>()))
            .Do(x => capturedAction = x.Arg<Action<OnPayBookingRequestedEvent>>());

        _viewModel = new PayBookingViewModel(_service, _pageNotifier);
        capturedAction?.Invoke(new OnPayBookingRequestedEvent { Reservation = reservation });

        // Act
        var canExecute = _viewModel.OnPay.CanExecute(null);

        // Assert
        Assert.That(canExecute, Is.False);
    }

    [Test]
    public void Should_ReturnTrue_When_CanExecutePay_Given_ValidReservation()
    {
        // Arrange
        var ticket = CreateTicket(5, 10);
        var reservation = CreateReservation([ticket]);
        reservation.IsValid.Returns(true);

        Action<OnPayBookingRequestedEvent>? capturedAction = null;
        _pageNotifier.When(x => x.Subscribe(
                Arg.Any<Action<OnPayBookingRequestedEvent>>()))
            .Do(x => capturedAction = x.Arg<Action<OnPayBookingRequestedEvent>>());

        _viewModel = new PayBookingViewModel(_service, _pageNotifier);
        capturedAction?.Invoke(new OnPayBookingRequestedEvent { Reservation = reservation });

        // Act
        var canExecute = _viewModel.OnPay.CanExecute(null);

        // Assert
        Assert.That(canExecute, Is.True);
    }

    // === COMMAND TESTS - OnPay Execute ===

    [Test]
    public void Should_SaveReservation_When_PayExecuted_Given_ValidReservation()
    {
        // Arrange
        var ticket = CreateTicket(5, 10);
        var reservation = CreateReservation([ticket]);
        reservation.IsValid.Returns(true);

        Action<OnPayBookingRequestedEvent>? capturedAction = null;
        _pageNotifier.When(x => x.Subscribe(
                Arg.Any<Action<OnPayBookingRequestedEvent>>()))
            .Do(x => capturedAction = x.Arg<Action<OnPayBookingRequestedEvent>>());

        _viewModel = new PayBookingViewModel(_service, _pageNotifier);
        capturedAction?.Invoke(new OnPayBookingRequestedEvent { Reservation = reservation });

        // Act
        _viewModel.OnPay.Execute(null);

        // Assert
        _service.Received(1).SaveReservationAndNavigateHome(reservation);
    }

    [Test]
    public void Should_ClearInputs_When_PayExecuted_Given_ValidReservation()
    {
        // Arrange
        var ticket = CreateTicket(5, 10);
        var reservation = CreateReservation([ticket]);
        reservation.IsValid.Returns(true);

        Action<OnPayBookingRequestedEvent>? capturedAction = null;
        _pageNotifier.When(x => x.Subscribe(
                Arg.Any<Action<OnPayBookingRequestedEvent>>()))
            .Do(x => capturedAction = x.Arg<Action<OnPayBookingRequestedEvent>>());

        _viewModel = new PayBookingViewModel(_service, _pageNotifier);
        capturedAction?.Invoke(new OnPayBookingRequestedEvent { Reservation = reservation });

        // Act
        _viewModel.OnPay.Execute(null);

        // Assert
        Assert.That(_viewModel.Inputs, Is.Empty);
    }

    [Test]
    public void Should_ClearReservation_When_PayExecuted_Given_ValidReservation()
    {
        // Arrange
        var ticket = CreateTicket(5, 10);
        var reservation = CreateReservation([ticket]);
        reservation.IsValid.Returns(true);

        Action<OnPayBookingRequestedEvent>? capturedAction = null;
        _pageNotifier.When(x => x.Subscribe(
                Arg.Any<Action<OnPayBookingRequestedEvent>>()))
            .Do(x => capturedAction = x.Arg<Action<OnPayBookingRequestedEvent>>());

        _viewModel = new PayBookingViewModel(_service, _pageNotifier);
        capturedAction?.Invoke(new OnPayBookingRequestedEvent { Reservation = reservation });

        // Act
        _viewModel.OnPay.Execute(null);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(_viewModel.Price, Is.Zero);
            Assert.That(_viewModel.Reduction, Is.Zero);
        }
    }

    [Test]
    public void Should_DoNothing_When_PayExecuted_Given_InvalidReservation()
    {
        // Arrange
        var ticket = CreateTicket(5, 10);
        var reservation = CreateReservation([ticket]);
        reservation.IsValid.Returns(false);

        Action<OnPayBookingRequestedEvent>? capturedAction = null;
        _pageNotifier.When(x => x.Subscribe(
                Arg.Any<Action<OnPayBookingRequestedEvent>>()))
            .Do(x => capturedAction = x.Arg<Action<OnPayBookingRequestedEvent>>());

        _viewModel = new PayBookingViewModel(_service, _pageNotifier);
        capturedAction?.Invoke(new OnPayBookingRequestedEvent { Reservation = reservation });

        // Act
        _viewModel.OnPay.Execute(null);

        // Assert
        _service.DidNotReceive().SaveReservationAndNavigateHome(Arg.Any<IReservation>());
        Assert.That(_viewModel.Inputs, Has.Count.EqualTo(1)); // Pas nettoyé
    }

    [Test]
    public void Should_DoNothing_When_PayExecuted_Given_NoReservation()
    {
        // Act
        _viewModel.OnPay.Execute(null);

        // Assert
        _service.DidNotReceive().SaveReservationAndNavigateHome(Arg.Any<IReservation>());
    }

    // === FIELD INITIALIZATION TESTS ===

    [Test]
    public void Should_CreateFieldForEachTicket_When_EventReceived_Given_MultipleTickets()
    {
        // Arrange
        var ticket1 = CreateTicket(1, 5);
        var ticket2 = CreateTicket(2, 6);
        var ticket3 = CreateTicket(3, 7);
        var reservation = CreateReservation([ticket1, ticket2, ticket3]);

        Action<OnPayBookingRequestedEvent>? capturedAction = null;
        _pageNotifier.When(x => x.Subscribe(
                Arg.Any<Action<OnPayBookingRequestedEvent>>()))
            .Do(x => capturedAction = x.Arg<Action<OnPayBookingRequestedEvent>>());

        _viewModel = new PayBookingViewModel(_service, _pageNotifier);

        // Act
        capturedAction?.Invoke(new OnPayBookingRequestedEvent { Reservation = reservation });

        // Assert
        Assert.That(_viewModel.Inputs, Has.Count.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(_viewModel.Inputs[0].SeatId, Is.EqualTo("1 - 5"));
            Assert.That(_viewModel.Inputs[1].SeatId, Is.EqualTo("2 - 6"));
            Assert.That(_viewModel.Inputs[2].SeatId, Is.EqualTo("3 - 7"));
        }
    }

    [Test]
    public void Should_ReplaceOldInputs_When_EventReceivedTwice_Given_DifferentReservations()
    {
        // Arrange
        var ticket1 = CreateTicket(1, 5);
        var reservation1 = CreateReservation([ticket1]);

        var ticket2 = CreateTicket(2, 6);
        var ticket3 = CreateTicket(3, 7);
        var reservation2 = CreateReservation([ticket2, ticket3]);

        Action<OnPayBookingRequestedEvent>? capturedAction = null;
        _pageNotifier.When(x => x.Subscribe(
                Arg.Any<Action<OnPayBookingRequestedEvent>>()))
            .Do(x => capturedAction = x.Arg<Action<OnPayBookingRequestedEvent>>());

        _viewModel = new PayBookingViewModel(_service, _pageNotifier);

        // Act
        capturedAction?.Invoke(new OnPayBookingRequestedEvent { Reservation = reservation1 });
        capturedAction?.Invoke(new OnPayBookingRequestedEvent { Reservation = reservation2 });

        // Assert
        Assert.That(_viewModel.Inputs, Has.Count.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(_viewModel.Inputs[0].SeatId, Is.EqualTo("2 - 6"));
            Assert.That(_viewModel.Inputs[1].SeatId, Is.EqualTo("3 - 7"));
        }
    }

    // === HELPER METHODS ===

    private static ITicket CreateTicket(ushort row, ushort col, int age = 0)
    {
        var ticket = Substitute.For<ITicket>();
        var seat = new Seat(row, col);
        ticket.Seat.Returns(seat);
        ticket.Age.Returns(age);
        ticket.ValidTicket.Returns(false);
        ticket.Value.Returns(10.0);
        return ticket;
    }

    private static IReservation CreateReservation(ITicket[] tickets, string movieTitle = "Test Movie")
    {
        var reservation = Substitute.For<IReservation>();
        var title = new MovieTitle(movieTitle);

        reservation.Title.Returns(title);
        reservation.Tickets.Returns(tickets);
        reservation.IsValid.Returns(false);
        reservation.Price.Returns(0.0);
        reservation.Reduction.Returns(0);
        reservation.MinimumAge.Returns(0);

        return reservation;
    }
}
