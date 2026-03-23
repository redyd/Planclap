using NSubstitute;
using Planclap.Client.Domains.core;
using Planclap.Client.Domains.IEntities;
using Planclap.Client.Presentations.Common;
using Planclap.Client.Presentations.IViewModel;
using Planclap.Client.Presentations.PresentationServices;

namespace Planclap.Client.Presentations.Tests.ViewModel;

[TestFixture]
public class SeatManagementTests
{
    [SetUp]
    public void SetUp() => _seatManagement = new SeatManagement();

    private SeatManagement _seatManagement = null!;

    // === CONSTRUCTOR & INITIAL STATE TESTS ===

    [Test]
    public void Should_InitializeWithEmptySeats_When_Constructed_Given_NewInstance() =>
        // Assert
        Assert.That(_seatManagement.AllSeats, Is.Empty);

    [Test]
    public void Should_InitializeWithEmptySelection_When_Constructed_Given_NewInstance() =>
        // Assert
        Assert.That(_seatManagement.SelectedSeats, Is.Empty);

    [Test]
    public void Should_InitializeWithZeroCount_When_Constructed_Given_NewInstance() =>
        // Assert
        Assert.That(_seatManagement.SelectedCount, Is.Zero);

    // === SetSeats TESTS ===

    [Test]
    public void Should_AddAllSeats_When_SetSeatsInvoked_Given_ListOfSeats()
    {
        // Arrange
        var seat1 = CreateSeat("1-1", true);
        var seat2 = CreateSeat("1-2", true);
        var seat3 = CreateSeat("1-3", false);
        var seats = new List<ISeatViewModel> { seat1, seat2, seat3 };

        // Act
        _seatManagement.SetSeats(seats, _ => { }, _ => true);

        // Assert
        Assert.That(_seatManagement.AllSeats, Has.Count.EqualTo(3));
    }

    [Test]
    public void Should_AssignClickCommand_When_SetSeatsInvoked_Given_ListOfSeats()
    {
        // Arrange
        var seat1 = CreateSeat("1-1", true);
        var seat2 = CreateSeat("1-2", true);
        var seats = new List<ISeatViewModel> { seat1, seat2 };

        // Act
        _seatManagement.SetSeats(seats, _ => { }, _ => true);

        // Assert
        seat1.Received().ClickCommand = Arg.Any<ActionTRelayCommand<ISeatViewModel>>();
        seat2.Received().ClickCommand = Arg.Any<ActionTRelayCommand<ISeatViewModel>>();
    }

    [Test]
    public void Should_ClearPreviousSeats_When_SetSeatsInvoked_Given_SeatsAlreadyExist()
    {
        // Arrange
        var initialSeats = new List<ISeatViewModel> { CreateSeat("1-1", true) };
        var newSeats = new List<ISeatViewModel> { CreateSeat("2-1", true), CreateSeat("2-2", true) };

        // Act
        _seatManagement.SetSeats(initialSeats, _ => { }, _ => true);
        _seatManagement.SetSeats(newSeats, _ => { }, _ => true);

        // Assert
        Assert.That(_seatManagement.AllSeats, Has.Count.EqualTo(2));
    }

    [Test]
    public void Should_ClearSelectedSeats_When_SetSeatsInvoked_Given_SeatsWereSelected()
    {
        // Arrange
        var seat1 = CreateSeat("1-1", true);
        var initialSeats = new List<ISeatViewModel> { seat1 };
        _seatManagement.SetSeats(initialSeats, _ => { }, _ => true);
        _seatManagement.ToggleSelection(seat1);

        var newSeats = new List<ISeatViewModel> { CreateSeat("2-1", true) };

        // Act
        _seatManagement.SetSeats(newSeats, _ => { }, _ => true);

        // Assert
        Assert.That(_seatManagement.SelectedCount, Is.Zero);
    }

    [Test]
    public void Should_RaiseSelectionChanged_When_SetSeatsInvoked_Given_ValidSeats()
    {
        // Arrange
        var eventRaised = false;
        _seatManagement.SelectionChanged += () => eventRaised = true;
        var seats = new List<ISeatViewModel> { CreateSeat("1-1", true) };

        // Act
        _seatManagement.SetSeats(seats, _ => { }, _ => true);

        // Assert
        Assert.That(eventRaised, Is.True);
    }

    [Test]
    public void Should_HandleEmptyList_When_SetSeatsInvoked_Given_EmptyCollection()
    {
        // Arrange
        var seats = new List<ISeatViewModel>();

        // Act
        _seatManagement.SetSeats(seats, _ => { }, _ => true);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(_seatManagement.AllSeats, Is.Empty);
            Assert.That(_seatManagement.SelectedCount, Is.Zero);
        }
    }

    // === ToggleSelection TESTS ===

    [Test]
    public void Should_AddSeatToSelection_When_ToggleSelectionInvoked_Given_UnselectedSeat()
    {
        // Arrange
        var seat = CreateSeat("1-1", true);
        var seats = new List<ISeatViewModel> { seat };
        _seatManagement.SetSeats(seats, _ => { }, _ => true);

        // Act
        _seatManagement.ToggleSelection(seat);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(_seatManagement.SelectedCount, Is.EqualTo(1));
            Assert.That(_seatManagement.SelectedSeats, Contains.Item(seat));
        }
    }

    [Test]
    public void Should_SetIsSelectedTrue_When_ToggleSelectionInvoked_Given_UnselectedSeat()
    {
        // Arrange
        var seat = CreateSeat("1-1", true);
        var seats = new List<ISeatViewModel> { seat };
        _seatManagement.SetSeats(seats, _ => { }, _ => true);

        // Act
        _seatManagement.ToggleSelection(seat);

        // Assert
        seat.Received().IsSelected = true;
    }

    [Test]
    public void Should_RemoveSeatFromSelection_When_ToggleSelectionInvoked_Given_SelectedSeat()
    {
        // Arrange
        var seat = CreateSeat("1-1", true);
        var seats = new List<ISeatViewModel> { seat };
        _seatManagement.SetSeats(seats, _ => { }, _ => true);
        _seatManagement.ToggleSelection(seat); // Select

        // Act
        _seatManagement.ToggleSelection(seat); // Deselect

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(_seatManagement.SelectedCount, Is.Zero);
            Assert.That(_seatManagement.SelectedSeats, Does.Not.Contain(seat));
        }
    }

    [Test]
    public void Should_SetIsSelectedFalse_When_ToggleSelectionInvoked_Given_SelectedSeat()
    {
        // Arrange
        var seat = CreateSeat("1-1", true);
        var seats = new List<ISeatViewModel> { seat };
        _seatManagement.SetSeats(seats, _ => { }, _ => true);
        _seatManagement.ToggleSelection(seat); // Select

        // Act
        _seatManagement.ToggleSelection(seat); // Deselect

        // Assert
        seat.Received().IsSelected = false;
    }

    [Test]
    public void Should_RaiseSelectionChanged_When_ToggleSelectionInvoked_Given_AnySeat()
    {
        // Arrange
        var seat = CreateSeat("1-1", true);
        var seats = new List<ISeatViewModel> { seat };
        _seatManagement.SetSeats(seats, _ => { }, _ => true);

        var eventRaised = false;
        _seatManagement.SelectionChanged += () => eventRaised = true;

        // Act
        _seatManagement.ToggleSelection(seat);

        // Assert
        Assert.That(eventRaised, Is.True);
    }

    [Test]
    public void Should_HandleMultipleSelections_When_ToggleSelectionInvoked_Given_MultipleSeats()
    {
        // Arrange
        var seat1 = CreateSeat("1-1", true);
        var seat2 = CreateSeat("1-2", true);
        var seat3 = CreateSeat("1-3", true);
        var seats = new List<ISeatViewModel> { seat1, seat2, seat3 };
        _seatManagement.SetSeats(seats, _ => { }, _ => true);

        // Act
        _seatManagement.ToggleSelection(seat1);
        _seatManagement.ToggleSelection(seat2);
        _seatManagement.ToggleSelection(seat3);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(_seatManagement.SelectedCount, Is.EqualTo(3));
            Assert.That(_seatManagement.SelectedSeats, Contains.Item(seat1));
        }

        Assert.That(_seatManagement.SelectedSeats, Contains.Item(seat2));
        Assert.That(_seatManagement.SelectedSeats, Contains.Item(seat3));
    }

    [Test]
    public void Should_HandleMixedToggles_When_ToggleSelectionInvoked_Given_SelectAndDeselectActions()
    {
        // Arrange
        var seat1 = CreateSeat("1-1", true);
        var seat2 = CreateSeat("1-2", true);
        var seat3 = CreateSeat("1-3", true);
        var seats = new List<ISeatViewModel> { seat1, seat2, seat3 };
        _seatManagement.SetSeats(seats, _ => { }, _ => true);

        // Act
        _seatManagement.ToggleSelection(seat1); // Select
        _seatManagement.ToggleSelection(seat2); // Select
        _seatManagement.ToggleSelection(seat3); // Select
        _seatManagement.ToggleSelection(seat2); // Deselect

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(_seatManagement.SelectedCount, Is.EqualTo(2));
            Assert.That(_seatManagement.SelectedSeats, Contains.Item(seat1));
        }

        Assert.That(_seatManagement.SelectedSeats, Does.Not.Contain(seat2));
        Assert.That(_seatManagement.SelectedSeats, Contains.Item(seat3));
    }

    // === Clear TESTS ===

    [Test]
    public void Should_ClearAllSeats_When_ClearInvoked_Given_SeatsExist()
    {
        // Arrange
        var seats = new List<ISeatViewModel> { CreateSeat("1-1", true), CreateSeat("1-2", true) };
        _seatManagement.SetSeats(seats, _ => { }, _ => true);

        // Act
        _seatManagement.Clear();

        // Assert
        Assert.That(_seatManagement.AllSeats, Is.Empty);
    }

    [Test]
    public void Should_ClearSelectedSeats_When_ClearInvoked_Given_SelectionsExist()
    {
        // Arrange
        var seat = CreateSeat("1-1", true);
        var seats = new List<ISeatViewModel> { seat };
        _seatManagement.SetSeats(seats, _ => { }, _ => true);
        _seatManagement.ToggleSelection(seat);

        // Act
        _seatManagement.Clear();

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(_seatManagement.SelectedSeats, Is.Empty);
            Assert.That(_seatManagement.SelectedCount, Is.Zero);
        }
    }

    [Test]
    public void Should_RaiseSelectionChanged_When_ClearInvoked_Given_AnyState()
    {
        // Arrange
        var eventRaised = false;
        _seatManagement.SelectionChanged += () => eventRaised = true;

        // Act
        _seatManagement.Clear();

        // Assert
        Assert.That(eventRaised, Is.True);
    }

    [Test]
    public void Should_HandleClearOnEmptyCollection_When_ClearInvoked_Given_NoSeats()
    {
        // Act
        _seatManagement.Clear();

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(_seatManagement.AllSeats, Is.Empty);
            Assert.That(_seatManagement.SelectedSeats, Is.Empty);
        }
    }

    // === MapToViewModels TESTS ===

    [Test]
    public void Should_CreateCorrectNumberOfSeats_When_MapToViewModelsInvoked_Given_RowsAndCols()
    {
        // Arrange
        var scheduled = CreateScheduled();

        // Act
        var result = _seatManagement.MapToViewModels(scheduled, 3, 4);

        // Assert
        Assert.That(result, Has.Count.EqualTo(12)); // 3 rows * 4 cols
    }

    [Test]
    public void Should_CreateSeatsWithCorrectIds_When_MapToViewModelsInvoked_Given_RowsAndCols()
    {
        // Arrange
        var scheduled = CreateScheduled();

        // Act
        var result = _seatManagement.MapToViewModels(scheduled, 2, 2);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result[0].Id, Is.EqualTo("0-0"));
            Assert.That(result[1].Id, Is.EqualTo("0-1"));
            Assert.That(result[2].Id, Is.EqualTo("1-0"));
            Assert.That(result[3].Id, Is.EqualTo("1-1"));
        }
    }

    [Test]
    public void Should_MarkSeatsAsUnavailable_When_MapToViewModelsInvoked_Given_TakenSeats()
    {
        // Arrange
        var scheduled = CreateScheduled();
        scheduled.SeatTaken(0, 0).Returns(true);
        scheduled.SeatTaken(1, 1).Returns(true);

        // Act
        var result = _seatManagement.MapToViewModels(scheduled, 2, 2);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result[0].IsAvailable, Is.False); // 0-0 taken
            Assert.That(result[1].IsAvailable, Is.True); // 0-1 available
            Assert.That(result[2].IsAvailable, Is.True); // 1-0 available
            Assert.That(result[3].IsAvailable, Is.False); // 1-1 taken
        }
    }

    [Test]
    public void Should_MarkAllSeatsAsAvailable_When_MapToViewModelsInvoked_Given_NoTakenSeats()
    {
        // Arrange
        var scheduled = CreateScheduled();
        scheduled.SeatTaken(Arg.Any<int>(), Arg.Any<int>()).Returns(false);

        // Act
        var result = _seatManagement.MapToViewModels(scheduled, 3, 3);

        // Assert
        Assert.That(result.All(s => s.IsAvailable), Is.True);
    }

    [Test]
    public void Should_ReturnReadOnlyList_When_MapToViewModelsInvoked_Given_ValidInput()
    {
        // Arrange
        var scheduled = CreateScheduled();

        // Act
        var result = _seatManagement.MapToViewModels(scheduled, 2, 2);

        // Assert
        Assert.That(result, Is.InstanceOf<IReadOnlyList<ISeatViewModel>>());
    }

    [Test]
    public void Should_HandleZeroRows_When_MapToViewModelsInvoked_Given_ZeroRows()
    {
        // Arrange
        var scheduled = CreateScheduled();

        // Act
        var result = _seatManagement.MapToViewModels(scheduled, 0, 5);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void Should_HandleZeroCols_When_MapToViewModelsInvoked_Given_ZeroCols()
    {
        // Arrange
        var scheduled = CreateScheduled();

        // Act
        var result = _seatManagement.MapToViewModels(scheduled, 5, 0);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void Should_CreateLargeGrid_When_MapToViewModelsInvoked_Given_LargeRowsAndCols()
    {
        // Arrange
        var scheduled = CreateScheduled();

        // Act
        var result = _seatManagement.MapToViewModels(scheduled, 20, 30);

        // Assert
        Assert.That(result, Has.Count.EqualTo(600)); // 20 * 30
    }

    // === MapToTickets TESTS ===

    [Test]
    public void Should_CreateTicketsFromSeats_When_MapToTicketsInvoked_Given_SelectedSeats()
    {
        // Arrange
        var seat1 = CreateSeat("5-10", true);
        var seat2 = CreateSeat("6-11", true);
        var selectedSeats = new List<ISeatViewModel> { seat1, seat2 };

        // Act
        var result = _seatManagement.MapToTickets(selectedSeats);

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
    }

    [Test]
    public void Should_CreateTicketsWithCorrectSeats_When_MapToTicketsInvoked_Given_SelectedSeats()
    {
        // Arrange
        var seat1 = CreateSeat("5-10", true);
        var seat2 = CreateSeat("6-11", true);
        var selectedSeats = new List<ISeatViewModel> { seat1, seat2 };

        // Act
        var result = _seatManagement.MapToTickets(selectedSeats);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result[0].Seat.Row, Is.EqualTo(5));
            Assert.That(result[0].Seat.Column, Is.EqualTo(10));
            Assert.That(result[1].Seat.Row, Is.EqualTo(6));
            Assert.That(result[1].Seat.Column, Is.EqualTo(11));
        }
    }

    [Test]
    public void Should_CreateEmptyTickets_When_MapToTicketsInvoked_Given_NewlyCreatedTickets()
    {
        // Arrange
        var seat = CreateSeat("5-10", true);
        var selectedSeats = new List<ISeatViewModel> { seat };

        // Act
        var result = _seatManagement.MapToTickets(selectedSeats);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result[0].TicketType, Is.EqualTo(TicketType.Empty));
            Assert.That(result[0].ValidTicket, Is.False);
        }
    }

    [Test]
    public void Should_HandleEmptySelection_When_MapToTicketsInvoked_Given_NoSelectedSeats()
    {
        // Arrange
        var selectedSeats = new List<ISeatViewModel>();

        // Act
        var result = _seatManagement.MapToTickets(selectedSeats);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void Should_HandleMultipleTickets_When_MapToTicketsInvoked_Given_ManySelectedSeats()
    {
        // Arrange
        var selectedSeats = new List<ISeatViewModel> { CreateSeat("0-0", true), CreateSeat("1-5", true), CreateSeat("10-20", true), CreateSeat("15-25", true) };

        // Act
        var result = _seatManagement.MapToTickets(selectedSeats);

        // Assert
        Assert.That(result, Has.Count.EqualTo(4));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result[0].Seat.Row, Is.Zero);
            Assert.That(result[0].Seat.Column, Is.Zero);
            Assert.That(result[1].Seat.Row, Is.EqualTo(1));
            Assert.That(result[1].Seat.Column, Is.EqualTo(5));
            Assert.That(result[2].Seat.Row, Is.EqualTo(10));
            Assert.That(result[2].Seat.Column, Is.EqualTo(20));
            Assert.That(result[3].Seat.Row, Is.EqualTo(15));
            Assert.That(result[3].Seat.Column, Is.EqualTo(25));
        }
    }

    [Test]
    public void Should_ParseSeatIdsCorrectly_When_MapToTicketsInvoked_Given_VariousFormats()
    {
        // Arrange
        var selectedSeats = new List<ISeatViewModel> { CreateSeat("0-0", true), CreateSeat("100-200", true) };

        // Act
        var result = _seatManagement.MapToTickets(selectedSeats);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result[0].Seat.Row, Is.Zero);
            Assert.That(result[0].Seat.Column, Is.Zero);
            Assert.That(result[1].Seat.Row, Is.EqualTo(100));
            Assert.That(result[1].Seat.Column, Is.EqualTo(200));
        }
    }

    // === INTEGRATION TESTS ===

    [Test]
    public void Should_RaiseSelectionChangedMultipleTimes_When_MultipleOperations_Given_TypicalFlow()
    {
        // Arrange
        var eventCount = 0;
        _seatManagement.SelectionChanged += () => eventCount++;

        var seat = CreateSeat("1-1", true);
        var seats = new List<ISeatViewModel> { seat };

        // Act
        _seatManagement.SetSeats(seats, _ => { }, _ => true); // +1
        _seatManagement.ToggleSelection(seat); // +1
        _seatManagement.ToggleSelection(seat); // +1
        _seatManagement.Clear(); // +1

        // Assert
        Assert.That(eventCount, Is.EqualTo(4));
    }

    [Test]
    public void Should_HandleComplexScenario_When_MultipleSetSeatsAndToggles_Given_ChangingState()
    {
        // Arrange
        var seat1 = CreateSeat("1-1", true);
        var seat2 = CreateSeat("2-2", true);
        var firstBatch = new List<ISeatViewModel> { seat1 };
        var secondBatch = new List<ISeatViewModel> { seat2 };

        // Act
        _seatManagement.SetSeats(firstBatch, _ => { }, _ => true);
        _seatManagement.ToggleSelection(seat1);
        Assert.That(_seatManagement.SelectedCount, Is.EqualTo(1));

        _seatManagement.SetSeats(secondBatch, _ => { }, _ => true);
        Assert.That(_seatManagement.SelectedCount, Is.Zero); // Cleared by SetSeats

        _seatManagement.ToggleSelection(seat2);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(_seatManagement.SelectedCount, Is.EqualTo(1));

            // Assert
            Assert.That(_seatManagement.AllSeats, Has.Count.EqualTo(1));
        }

        Assert.That(_seatManagement.AllSeats[0], Is.EqualTo(seat2));
    }

    // === HELPER METHODS ===

    private static ISeatViewModel CreateSeat(string id, bool isAvailable)
    {
        var seat = Substitute.For<ISeatViewModel>();
        seat.Id.Returns(id);
        seat.IsAvailable.Returns(isAvailable);
        return seat;
    }

    private static IScheduled CreateScheduled()
    {
        var scheduled = Substitute.For<IScheduled>();
        scheduled.SeatTaken(Arg.Any<int>(), Arg.Any<int>()).Returns(false);
        return scheduled;
    }
}
