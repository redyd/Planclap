using Planclap.Client.Presentations.IViewModel;
using Planclap.Client.Presentations.View;

namespace Planclap.Client.Presentations.Tests.View;

/// <summary>
///     Classe générée par IA
/// </summary>
public class DummyPayBookingViewModelTests
{
    [Test]
    public void Should_ReturnFixedTitle_When_AccessingTitle_Given_DefaultInitialization()
    {
        // Arrange
        var sut = new DummyPayBookingViewModel();

        // Act
        var title = sut.Title;

        // Assert
        Assert.That(title, Is.EqualTo("Réservation pour DummyPayBookingViewModel"));
    }

    [Test]
    public void Should_ReturnExpectedReduction_When_AccessingReduction_Given_DefaultInitialization()
    {
        // Arrange
        var sut = new DummyPayBookingViewModel();

        // Act
        var reduction = sut.Reduction;

        // Assert
        Assert.That(reduction, Is.EqualTo(10));
    }

    [Test]
    public void Should_ReturnExpectedPrice_When_AccessingPrice_Given_DefaultInitialization()
    {
        // Arrange
        var sut = new DummyPayBookingViewModel();

        // Act
        var price = sut.Price;

        // Assert
        Assert.That(price, Is.EqualTo(34.5));
    }

    [Test]
    public void Should_ReturnTrueForShowWarning_When_AccessingShowWarning_Given_DefaultInitialization()
    {
        // Arrange
        var sut = new DummyPayBookingViewModel();

        // Act
        var showWarning = sut.ShowWarning;

        // Assert
        Assert.That(showWarning, Is.True);
    }

    [Test]
    public void Should_ReturnNonEmptyWarningMessage_When_AccessingWarningMessage_Given_DefaultInitialization()
    {
        // Arrange
        var sut = new DummyPayBookingViewModel();

        // Act
        var message = sut.WarningMessage;

        // Assert
        Assert.That(message, Is.Not.Null);
        Assert.That(message.Trim(), Is.Not.Empty);
    }

    [Test]
    public void Should_ReturnThreeInputs_When_AccessingInputs_Given_DefaultInitialization()
    {
        // Arrange
        var sut = new DummyPayBookingViewModel();

        // Act
        var inputs = sut.Inputs;

        // Assert
        Assert.That(inputs, Has.Count.EqualTo(3));
    }

    [Test]
    public void Should_ContainOnlyFieldPayBookingViewModels_When_AccessingInputs_Given_DefaultInitialization()
    {
        // Arrange
        var sut = new DummyPayBookingViewModel();

        // Act
        var inputs = sut.Inputs;

        // Assert
        Assert.That(inputs.All(x => x is IFieldPayBookingViewModel), Is.True);
    }

    [Test]
    public void Should_HaveExpectedSeatIds_When_AccessingInputs_Given_DefaultInitialization()
    {
        // Arrange
        var sut = new DummyPayBookingViewModel();

        // Act
        var seatIds = sut.Inputs.Select(i => i.SeatId).ToList();

        // Assert
        Assert.That(seatIds, Is.EqualTo([
            "1 - 4",
            "1 - 5",
            "1 - 6"
        ]));
    }

    [Test]
    public void Should_ReturnNewCancelCommandEachTime_When_AccessingOnCancel_Given_DefaultInitialization()
    {
        // Arrange
        var sut = new DummyPayBookingViewModel();

        // Act
        var cmd1 = sut.OnCancel;
        var cmd2 = sut.OnCancel;

        // Assert
        Assert.That(cmd1, Is.Not.SameAs(cmd2));
    }

    [Test]
    public void Should_ReturnNewPayCommandEachTime_When_AccessingOnPay_Given_DefaultInitialization()
    {
        // Arrange
        var sut = new DummyPayBookingViewModel();

        // Act
        var cmd1 = sut.OnPay;
        var cmd2 = sut.OnPay;

        // Assert
        Assert.That(cmd1, Is.Not.SameAs(cmd2));
    }

    [Test]
    public void Should_ReturnExecutablePayCommand_When_AccessingOnPay_Given_DefaultInitialization()
    {
        // Arrange
        var sut = new DummyPayBookingViewModel();

        // Act
        var canExecute = sut.OnPay.CanExecute(null);

        // Assert
        Assert.That(canExecute, Is.True);
    }
}
