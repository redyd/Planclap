using Planclap.Client.Presentations.Common;

namespace Planclap.Client.Presentations.Tests.Common;

/// <summary>
///     Classe générée par IA
/// </summary>
public class ActionRelayCommandTests
{
    [Test]
    public void Should_InvokeExecuteAction_When_ExecuteIsCalled_Given_ValidCommand()
    {
        // Arrange
        var executed = false;
        var sut = new ActionRelayCommand(() => executed = true);

        // Act
        sut.Execute(null);

        // Assert
        Assert.That(executed, Is.True);
    }

    [Test]
    public void Should_ReturnTrue_When_CanExecuteIsNull_Given_CommandCreatedWithoutCanExecute()
    {
        // Arrange
        var sut = new ActionRelayCommand(() => { });

        // Act
        var result = sut.CanExecute(null);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void Should_ReturnValueFromCanExecute_When_CanExecuteIsProvided_Given_CommandCreatedWithCanExecute()
    {
        // Arrange
        var sut = new ActionRelayCommand(
            () => { },
            () => false
        );

        // Act
        var result = sut.CanExecute(null);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void Should_RaiseCanExecuteChanged_When_RaiseCanExecuteChangedIsCalled_Given_SubscribedHandler()
    {
        // Arrange
        var sut = new ActionRelayCommand(() => { });
        var eventRaised = false;

        sut.CanExecuteChanged += (_, _) => eventRaised = true;

        // Act
        sut.RaiseCanExecuteChanged();

        // Assert
        Assert.That(eventRaised, Is.True);
    }
}
