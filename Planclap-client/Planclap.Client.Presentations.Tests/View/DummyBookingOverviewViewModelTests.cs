using Planclap.Client.Presentations.IViewModel;
using Planclap.Client.Presentations.View;

namespace Planclap.Client.Presentations.Tests.View;

/// <summary>
///     Classe générée par IA
/// </summary>
public class DummyBookingOverviewViewModelTests
{
    [Test]
    public void Should_ReturnFixedReservationTitle_When_AccessingReservationTitle_Given_DefaultInitialization()
    {
        // Arrange
        var sut = new DummyBookingOverviewViewModel();

        // Act
        var result = sut.ReservationTitle;

        // Assert
        Assert.That(result, Is.EqualTo("Film dummy"));
    }

    [Test]
    public void Should_ReturnExpectedRowsAndColumns_When_Called_Given_DefaultInitialization()
    {
        // Arrange
        var sut = new DummyBookingOverviewViewModel();

        using (Assert.EnterMultipleScope())
        {
            // Act / Assert
            Assert.That(sut.Rows, Is.EqualTo(10));
            Assert.That(sut.Columns, Is.EqualTo(8));
        }
    }

    [Test]
    public void Should_GenerateSeatsOnlyOnce_When_AccessingSeatsMultipleTimes_Given_RandomInitialization()
    {
        // Arrange
        var sut = new DummyBookingOverviewViewModel();

        // Act
        var seats1 = sut.Seats;
        var seats2 = sut.Seats;

        // Assert
        Assert.That(seats1, Is.SameAs(seats2));
    }

    [Test]
    public void Should_GenerateCorrectNumberOfSeats_When_SeatsAreAccessed_Given_DefaultRowsAndColumns()
    {
        // Arrange
        var sut = new DummyBookingOverviewViewModel();

        // Act
        var seats = sut.Seats;

        // Assert
        var expectedCount = sut.Rows * sut.Columns;
        Assert.That(seats, Has.Count.EqualTo(expectedCount));
    }

    [Test]
    public void Should_CreateSeatViewModels_When_SeatsAreGenerated_Given_DefaultInitialization()
    {
        // Arrange
        var sut = new DummyBookingOverviewViewModel();

        // Act
        var seats = sut.Seats;

        // Assert
        Assert.That(seats.All(s => s is ISeatViewModel), Is.True);
    }

    [Test]
    public void Should_AlwaysReturnNewCommandInstance_When_OnCreateBookingRequestedIsAccessed_Given_DefaultInitialization()
    {
        // Arrange
        var sut = new DummyBookingOverviewViewModel();

        // Act
        var cmd1 = sut.OnCreateBookingRequested;
        var cmd2 = sut.OnCreateBookingRequested;

        // Assert
        Assert.That(cmd1, Is.Not.SameAs(cmd2));
    }

    [Test]
    public void Should_ReturnExecutableCommand_When_OnCreateBookingRequestedIsAccessed_Given_DefaultInitialization()
    {
        // Arrange
        var sut = new DummyBookingOverviewViewModel();
        var cmd = sut.OnCreateBookingRequested;

        // Act
        var canExecute = cmd.CanExecute(null);

        // Assert
        Assert.That(canExecute, Is.True);
    }
}
