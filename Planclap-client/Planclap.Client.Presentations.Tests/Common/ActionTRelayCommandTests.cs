using Planclap.Client.Presentations.Common;

namespace Planclap.Client.Presentations.Tests.Common;

/// <summary>
///     Classe générée par IA
/// </summary>
public class ActionTRelayCommandTests
{
    [Test]
    public void Should_InvokeExecuteAction_When_ExecuteIsCalled_Given_ParameterOfCorrectType()
    {
        // Arrange
        var result = 0;
        var sut = new ActionTRelayCommand<int>(x => result = x);

        // Act
        sut.Execute(42);

        // Assert
        Assert.That(result, Is.EqualTo(42));
    }

    [Test]
    public void Should_NotInvokeExecuteAction_When_ExecuteIsCalled_Given_ParameterOfIncorrectType()
    {
        // Arrange
        var executed = false;
        var sut = new ActionTRelayCommand<int>(_ => executed = true);

        // Act
        sut.Execute("not an int"); // mauvais type

        // Assert
        Assert.That(executed, Is.False);
    }

    [Test]
    public void Should_ReturnTrue_When_CanExecuteIsNull_Given_CommandCreatedWithoutCanExecute()
    {
        // Arrange
        var sut = new ActionTRelayCommand<int>(_ => { });

        // Act
        var result = sut.CanExecute(123);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void Should_ReturnFalse_When_ParameterCannotBeConverted_Given_CanExecuteExists()
    {
        // Arrange
        var sut = new ActionTRelayCommand<int>(_ => { }, x => x > 0);

        // Act
        var result = sut.CanExecute("wrong");

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void Should_UseCanExecutePredicate_When_CanExecuteIsProvided_Given_ValidParameter()
    {
        // Arrange
        var sut = new ActionTRelayCommand<int>(_ => { }, x => x > 10);

        // Act
        var result = sut.CanExecute(5);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void Should_RaiseCanExecuteChanged_When_RaiseCanExecuteChangedIsCalled_Given_SubscribedHandler()
    {
        // Arrange
        var sut = new ActionTRelayCommand<int>(_ => { });
        var eventRaised = false;

        sut.CanExecuteChanged += (_, _) => eventRaised = true;

        // Act
        sut.RaiseCanExecuteChanged();

        // Assert
        Assert.That(eventRaised, Is.True);
    }
}
