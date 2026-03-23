using Planclap.Client.Presentations.Common;
using Planclap.Client.Presentations.IViewModel;
using Planclap.Client.Presentations.ViewModel;

namespace Planclap.Client.Presentations.Tests.ViewModel;

/// <summary>
///     Classe générée par IA
/// </summary>
[TestFixture]
public class SeatViewModelTests
{
    private SeatViewModel _viewModel = null!;

    // === CONSTRUCTOR TESTS ===

    [Test]
    public void Should_SetProperties_When_Constructed_Given_IdAndAvailability()
    {
        // Act
        _viewModel = new SeatViewModel("A1", true);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(_viewModel.Id, Is.EqualTo("A1"));
            Assert.That(_viewModel.IsAvailable, Is.True);
            Assert.That(_viewModel.IsSelected, Is.False);
            Assert.That(_viewModel.ClickCommand, Is.Null);
        }
    }

    // === PROPERTY: IsAvailable ===

    [Test]
    public void Should_RaisePropertyChanged_When_IsAvailableChanges_Given_NewValue()
    {
        // Arrange
        _viewModel = new SeatViewModel("A1", true);

        string? changedProperty = null;
        _viewModel.PropertyChanged += (_, e) => changedProperty = e.PropertyName;

        // Act
        _viewModel.IsAvailable = false;

        // Assert
        Assert.That(changedProperty, Is.EqualTo(nameof(SeatViewModel.IsAvailable)));
    }

    [Test]
    public void Should_NotRaisePropertyChanged_When_IsAvailableAssignedSameValue_Given_NoChange()
    {
        // Arrange
        _viewModel = new SeatViewModel("A1", true);

        var raised = false;
        _viewModel.PropertyChanged += (_, _) => raised = true;

        // Act
        _viewModel.IsAvailable = true;

        // Assert
        Assert.That(raised, Is.False);
    }

    [Test]
    public void Should_CallRaiseCanExecuteChanged_When_IsAvailableChanges_Given_ClickCommandAssigned()
    {
        // Arrange
        _viewModel = new SeatViewModel("A1", true);

        var eventRaised = false;

        var command = new ActionTRelayCommand<ISeatViewModel>(
            _ => { },
            _ => true
        );

        command.CanExecuteChanged += (_, _) => eventRaised = true;

        _viewModel.ClickCommand = command;

        // Act
        _viewModel.IsAvailable = false;

        // Assert
        Assert.That(eventRaised, Is.True);
    }


    [Test]
    public void Should_NotThrow_When_IsAvailableChanges_Given_ClickCommandIsNull()
    {
        // Arrange
        _viewModel = new SeatViewModel("A1", true) { ClickCommand = null };

        // Act / Assert: should not throw
        Assert.DoesNotThrow(() => _viewModel.IsAvailable = false);
    }

    // === PROPERTY: IsSelected ===

    [Test]
    public void Should_RaisePropertyChanged_When_IsSelectedChanges_Given_NewValue()
    {
        // Arrange
        _viewModel = new SeatViewModel("A1", true);

        string? changedProperty = null;
        _viewModel.PropertyChanged += (_, e) => changedProperty = e.PropertyName;

        // Act
        _viewModel.IsSelected = true;

        // Assert
        Assert.That(changedProperty, Is.EqualTo(nameof(SeatViewModel.IsSelected)));
    }

    [Test]
    public void Should_NotRaisePropertyChanged_When_IsSelectedAssignedSameValue_Given_NoChange()
    {
        // Arrange
        _viewModel = new SeatViewModel("A1", true);

        var raised = false;
        _viewModel.PropertyChanged += (_, _) => raised = true;

        // Act
        _viewModel.IsSelected = false;

        // Assert
        Assert.That(raised, Is.False);
    }

    // === PROPERTY: ClickCommand ===

    [Test]
    public void Should_RaisePropertyChanged_When_ClickCommandChanges_Given_NewCommand()
    {
        // Arrange
        _viewModel = new SeatViewModel("A1", true);

        string? changedProperty = null;
        _viewModel.PropertyChanged += (_, e) => changedProperty = e.PropertyName;

        var command = new ActionTRelayCommand<ISeatViewModel>(_ => { }, _ => true);

        // Act
        _viewModel.ClickCommand = command;

        // Assert
        Assert.That(changedProperty, Is.EqualTo(nameof(SeatViewModel.ClickCommand)));
    }

    [Test]
    public void Should_NotRaisePropertyChanged_When_ClickCommandAssignedSameCommand_Given_NoChange()
    {
        // Arrange
        _viewModel = new SeatViewModel("A1", true);

        var command = new ActionTRelayCommand<ISeatViewModel>(_ => { }, _ => true);
        _viewModel.ClickCommand = command;

        var raised = false;
        _viewModel.PropertyChanged += (_, _) => raised = true;

        // Act
        _viewModel.ClickCommand = command;

        // Assert
        Assert.That(raised, Is.False);
    }

    // === METHOD: ToString ===

    [Test]
    public void Should_ReturnFormattedString_When_ToStringCalled_Given_IdAndAvailability()
    {
        // Arrange
        _viewModel = new SeatViewModel("A1", false);

        // Act
        var result = _viewModel.ToString();

        // Assert
        Assert.That(result, Is.EqualTo("A1 - False"));
    }
}
