using Planclap.Client.Presentations.View;

namespace Planclap.Client.Presentations.Tests.View;

/// <summary>
///     Classe générée par IA
/// </summary>
[TestFixture]
public class DummySeatViewModelTests
{
    [Test]
    public void Constructor_ShouldInitializeProperties()
    {
        // Arrange & Act
        var vm = new DummySeatViewModel("2 - 3", true, false);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(vm.Id, Is.EqualTo("2 - 3"));
            Assert.That(vm.IsAvailable, Is.True);
            Assert.That(vm.IsSelected, Is.False);
            Assert.That(vm.ClickCommand, Is.Not.Null);
        }
    }

    [Test]
    public void ClickCommand_WhenSeatIsAvailable_ShouldToggleIsSelected()
    {
        // Arrange
        var vm = new DummySeatViewModel(isAvailable: true, isSelected: false);

        // Act
        vm.ClickCommand!.Execute(vm);

        // Assert
        Assert.That(vm.IsSelected, Is.True);

        // Act (toggle again)
        vm.ClickCommand!.Execute(vm);

        // Assert
        Assert.That(vm.IsSelected, Is.False);
    }

    [Test]
    public void ClickCommand_WhenSeatIsNotAvailable_ShouldNotChangeSelection()
    {
        // Arrange
        var vm = new DummySeatViewModel(isAvailable: false, isSelected: false);

        // Act
        vm.ClickCommand!.Execute(vm);

        // Assert
        Assert.That(vm.IsSelected, Is.False);
    }

    [Test]
    public void IsAvailable_SetValue_ShouldUpdateProperty()
    {
        var vm = new DummySeatViewModel
        {
            // Act
            IsAvailable = false
        };

        // Assert
        Assert.That(vm.IsAvailable, Is.False);

        // Act
        vm.IsAvailable = true;

        // Assert
        Assert.That(vm.IsAvailable, Is.True);
    }

    [Test]
    public void IsSelected_SetValue_ShouldUpdateProperty()
    {
        var vm = new DummySeatViewModel
        {
            // Act
            IsSelected = true
        };

        // Assert
        Assert.That(vm.IsSelected, Is.True);

        // Act
        vm.IsSelected = false;

        // Assert
        Assert.That(vm.IsSelected, Is.False);
    }

    [Test]
    public void ClickCommand_ShouldNotThrow_WhenExecutedWithNullParameter()
    {
        // Arrange
        var vm = new DummySeatViewModel();

        // Act & Assert
        Assert.That(() => vm.ClickCommand!.Execute(null), Throws.Nothing);
    }
}
