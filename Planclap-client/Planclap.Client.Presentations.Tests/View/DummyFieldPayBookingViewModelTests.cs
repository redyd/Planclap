using Planclap.Client.Presentations.View;

namespace Planclap.Client.Presentations.Tests.View;

/// <summary>
///     Classe générée par IA
/// </summary>
public class DummyFieldPayBookingViewModelTests
{
    [Test]
    public void Should_SetAllPropertiesCorrectly_When_Constructed_Given_ValidParameters()
    {
        // Arrange
        var sut = new DummyFieldPayBookingViewModel(
            "ABC123",
            "10-4",
            true,
            "None",
            12.5);

        using (Assert.EnterMultipleScope())
        {
            // Act & Assert
            Assert.That(sut.InputValue, Is.EqualTo("ABC123"));
            Assert.That(sut.SeatId, Is.EqualTo("10-4"));
            Assert.That(sut.IsValid, Is.True);
            Assert.That(sut.WarningInput, Is.EqualTo("None"));
            Assert.That(sut.Price, Is.EqualTo(12.5));
        }
    }

    [Test]
    public void Should_AllowUpdatingInputValue_When_Set_Given_DefaultInitialization()
    {
        // Arrange
        var sut = new DummyFieldPayBookingViewModel(
            "Initial",
            "1-1",
            false,
            "Warn",
            9.99)
        {
            // Act
            InputValue = "Modified"
        };

        // Assert
        Assert.That(sut.InputValue, Is.EqualTo("Modified"));
    }

    [Test]
    public void Should_NotModifyImmutableProperties_When_Updated_Given_InitialValues()
    {
        // Arrange
        var sut = new DummyFieldPayBookingViewModel(
            "XYZ",
            "7-3",
            false,
            "Low credit",
            15.75)
        {
            // Act
            // Only InputValue is mutable; we change it
            InputValue = "ChangeAllowed"
        };

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(sut.SeatId, Is.EqualTo("7-3"));
            Assert.That(sut.IsValid, Is.False);
            Assert.That(sut.WarningInput, Is.EqualTo("Low credit"));
            Assert.That(sut.Price, Is.EqualTo(15.75));
        }
    }
}
