using NSubstitute;
using Planclap.Client.Domains.core;
using Planclap.Client.Domains.IEntities;
using Planclap.Client.Presentations.ViewModel;

namespace Planclap.Client.Presentations.Tests.ViewModel;

/// <summary>
///     Classe générée par IA
/// </summary>
[TestFixture]
public class FieldPayBookingViewModelTests
{
    [SetUp]
    public void SetUp()
    {
        _ticket = Substitute.For<ITicket>();
        _ticket.Seat.Returns(new Seat(5, 10));
        _ticket.Value.Returns(12.50);

        _updateCalled = false;
        _onUpdate = () => _updateCalled = true;

        _viewModel = new FieldPayBookingViewModel(_ticket, _onUpdate);
    }

    private ITicket _ticket = null!;
    private Action _onUpdate = null!;
    private FieldPayBookingViewModel _viewModel = null!;
    private bool _updateCalled;

    // === CONSTRUCTOR & INITIAL STATE TESTS ===

    [Test]
    public void Should_InitializeWithEmptyInput_When_Constructed_Given_ValidTicket() =>
        // Assert
        Assert.That(_viewModel.InputValue, Is.EqualTo(string.Empty));

    [Test]
    public void Should_InitializeAsInvalid_When_Constructed_Given_ValidTicket() =>
        // Assert
        Assert.That(_viewModel.IsValid, Is.False);

    [Test]
    public void Should_InitializeWithEmptyWarning_When_Constructed_Given_ValidTicket() =>
        // Assert
        Assert.That(_viewModel.WarningInput, Is.EqualTo(string.Empty));

    [Test]
    public void Should_InitializeWithZeroPrice_When_Constructed_Given_ValidTicket() =>
        // Assert
        Assert.That(_viewModel.Price, Is.Zero);

    [Test]
    public void Should_ReturnFormattedSeatId_When_Accessed_Given_ValidSeat()
    {
        // Act
        var seatId = _viewModel.SeatId;

        // Assert
        Assert.That(seatId, Is.EqualTo("5 - 10"));
    }

    // === INPUT VALIDATION - Empty/Whitespace Tests ===

    [Test]
    public void Should_SetInvalid_When_InputIsEmpty_Given_EmptyString()
    {
        // Act
        _viewModel.InputValue = string.Empty;

        // Assert
        Assert.That(_viewModel.IsValid, Is.False);
    }

    [Test]
    public void Should_SetPriceToZero_When_InputIsEmpty_Given_EmptyString()
    {
        // Act
        _viewModel.InputValue = string.Empty;

        // Assert
        Assert.That(_viewModel.Price, Is.Zero);
    }

    // === INPUT VALIDATION - Non-Numeric Tests ===

    [Test]
    public void Should_SetInvalid_When_InputIsNotNumeric_Given_AlphabeticString()
    {
        // Act
        _viewModel.InputValue = "abc";

        // Assert
        Assert.That(_viewModel.IsValid, Is.False);
    }

    [Test]
    public void Should_SetWarning_When_InputIsNotNumeric_Given_AlphabeticString()
    {
        // Act
        _viewModel.InputValue = "abc";

        // Assert
        Assert.That(_viewModel.WarningInput, Is.EqualTo("Entrée incorrecte"));
    }

    [Test]
    public void Should_SetPriceToZero_When_InputIsNotNumeric_Given_AlphabeticString()
    {
        // Act
        _viewModel.InputValue = "abc";

        // Assert
        Assert.That(_viewModel.Price, Is.Zero);
    }

    [Test]
    public void Should_InvalidateTicket_When_InputIsNotNumeric_Given_AlphabeticString()
    {
        // Act
        _viewModel.InputValue = "abc";

        // Assert
        _ticket.Received(1).Invalidate();
    }

    [Test]
    public void Should_CallOnUpdate_When_InputIsNotNumeric_Given_AlphabeticString()
    {
        // Act
        _viewModel.InputValue = "abc";

        // Assert
        Assert.That(_updateCalled, Is.True);
    }

    [Test]
    public void Should_SetInvalid_When_InputIsNotNumeric_Given_MixedAlphanumeric()
    {
        // Act
        _viewModel.InputValue = "25abc";

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(_viewModel.IsValid, Is.False);
            Assert.That(_viewModel.WarningInput, Is.EqualTo("Entrée incorrecte"));
        }
    }

    [Test]
    public void Should_SetInvalid_When_InputIsNotNumeric_Given_SpecialCharacters()
    {
        // Act
        _viewModel.InputValue = "@#$";

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(_viewModel.IsValid, Is.False);
            Assert.That(_viewModel.WarningInput, Is.EqualTo("Entrée incorrecte"));
        }
    }

    [Test]
    public void Should_SetInvalid_When_InputIsNotNumeric_Given_DecimalNumber()
    {
        // Act
        _viewModel.InputValue = "25.5";

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(_viewModel.IsValid, Is.False);
            Assert.That(_viewModel.WarningInput, Is.EqualTo("Entrée incorrecte"));
        }
    }

    // === INPUT VALIDATION - Age Range Tests ===

    [Test]
    public void Should_SetInvalid_When_AgeIsNegative_Given_NegativeNumber()
    {
        // Act
        _viewModel.InputValue = "-5";

        // Assert
        Assert.That(_viewModel.IsValid, Is.False);
    }

    [Test]
    public void Should_SetWarning_When_AgeIsNegative_Given_NegativeNumber()
    {
        // Act
        _viewModel.InputValue = "-5";

        // Assert
        Assert.That(_viewModel.WarningInput, Is.EqualTo("Âge invalide"));
    }

    [Test]
    public void Should_SetPriceToZero_When_AgeIsNegative_Given_NegativeNumber()
    {
        // Act
        _viewModel.InputValue = "-5";

        // Assert
        Assert.That(_viewModel.Price, Is.Zero);
    }

    [Test]
    public void Should_InvalidateTicket_When_AgeIsNegative_Given_NegativeNumber()
    {
        // Act
        _viewModel.InputValue = "-5";

        // Assert
        _ticket.Received(1).Invalidate();
    }

    [Test]
    public void Should_CallOnUpdate_When_AgeIsNegative_Given_NegativeNumber()
    {
        // Act
        _viewModel.InputValue = "-5";

        // Assert
        Assert.That(_updateCalled, Is.True);
    }

    [Test]
    public void Should_SetInvalid_When_AgeIsAbove120_Given_121()
    {
        // Act
        _viewModel.InputValue = "121";

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(_viewModel.IsValid, Is.False);
            Assert.That(_viewModel.WarningInput, Is.EqualTo("Âge invalide"));
        }
    }

    [Test]
    public void Should_SetInvalid_When_AgeIsAbove120_Given_200()
    {
        // Act
        _viewModel.InputValue = "200";

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(_viewModel.IsValid, Is.False);
            Assert.That(_viewModel.WarningInput, Is.EqualTo("Âge invalide"));
        }
    }

    [Test]
    public void Should_InvalidateTicket_When_AgeIsAbove120_Given_150()
    {
        // Act
        _viewModel.InputValue = "150";

        // Assert
        _ticket.Received(1).Invalidate();
    }

    // === INPUT VALIDATION - Valid Age Tests ===

    [Test]
    public void Should_SetValid_When_AgeIsZero_Given_Zero()
    {
        // Act
        _viewModel.InputValue = "0";

        // Assert
        Assert.That(_viewModel.IsValid, Is.True);
    }

    [Test]
    public void Should_SetTicketAge_When_AgeIsZero_Given_Zero()
    {
        // Act
        _viewModel.InputValue = "0";

        // Assert
        _ticket.Received().Age = 0;
    }

    [Test]
    public void Should_ClearWarning_When_AgeIsZero_Given_Zero()
    {
        // Arrange
        _viewModel.InputValue = "abc"; // Met un warning d'abord

        // Act
        _viewModel.InputValue = "0";

        // Assert
        Assert.That(_viewModel.WarningInput, Is.EqualTo(string.Empty));
    }

    [Test]
    public void Should_SetPrice_When_AgeIsZero_Given_Zero()
    {
        // Act
        _viewModel.InputValue = "0";

        // Assert
        Assert.That(_viewModel.Price, Is.EqualTo(12.50));
    }

    [Test]
    public void Should_CallOnUpdate_When_AgeIsZero_Given_Zero()
    {
        // Act
        _viewModel.InputValue = "0";

        // Assert
        Assert.That(_updateCalled, Is.True);
    }

    [Test]
    public void Should_SetValid_When_AgeIs120_Given_120()
    {
        // Act
        _viewModel.InputValue = "120";

        // Assert
        Assert.That(_viewModel.IsValid, Is.True);
    }

    [Test]
    public void Should_SetTicketAge_When_AgeIs120_Given_120()
    {
        // Act
        _viewModel.InputValue = "120";

        // Assert
        _ticket.Received().Age = 120;
    }

    [Test]
    public void Should_SetValid_When_AgeIsTypical_Given_25()
    {
        // Act
        _viewModel.InputValue = "25";

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(_viewModel.IsValid, Is.True);
            Assert.That(_viewModel.WarningInput, Is.EqualTo(string.Empty));
            Assert.That(_viewModel.Price, Is.EqualTo(12.50));
        }
    }

    [Test]
    public void Should_SetTicketAge_When_AgeIsTypical_Given_25()
    {
        // Act
        _viewModel.InputValue = "25";

        // Assert
        _ticket.Received().Age = 25;
    }

    [Test]
    public void Should_CallOnUpdate_When_AgeIsTypical_Given_25()
    {
        // Act
        _viewModel.InputValue = "25";

        // Assert
        Assert.That(_updateCalled, Is.True);
    }

    [Test]
    public void Should_SetValid_When_AgeIsChild_Given_8()
    {
        // Act
        _viewModel.InputValue = "8";

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(_viewModel.IsValid, Is.True);
            Assert.That(_viewModel.WarningInput, Is.EqualTo(string.Empty));
        }
    }

    [Test]
    public void Should_SetTicketAge_When_AgeIsChild_Given_8()
    {
        // Act
        _viewModel.InputValue = "8";

        // Assert
        _ticket.Received().Age = 8;
    }

    [Test]
    public void Should_SetValid_When_AgeIsSenior_Given_75()
    {
        // Act
        _viewModel.InputValue = "75";

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(_viewModel.IsValid, Is.True);
            Assert.That(_viewModel.WarningInput, Is.EqualTo(string.Empty));
        }
    }

    [Test]
    public void Should_SetTicketAge_When_AgeIsSenior_Given_75()
    {
        // Act
        _viewModel.InputValue = "75";

        // Assert
        _ticket.Received().Age = 75;
    }

    // === WHITESPACE HANDLING TESTS ===

    [Test]
    public void Should_TrimWhitespace_When_InputHasLeadingSpaces_Given_SpacesBeforeNumber()
    {
        // Act
        _viewModel.InputValue = "   25";

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(_viewModel.InputValue, Is.EqualTo("25"));
            Assert.That(_viewModel.IsValid, Is.True);
        }
    }

    [Test]
    public void Should_TrimWhitespace_When_InputHasTrailingSpaces_Given_SpacesAfterNumber()
    {
        // Act
        _viewModel.InputValue = "25   ";

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(_viewModel.InputValue, Is.EqualTo("25"));
            Assert.That(_viewModel.IsValid, Is.True);
        }
    }

    [Test]
    public void Should_TrimWhitespace_When_InputHasBothSpaces_Given_SpacesAroundNumber()
    {
        // Act
        _viewModel.InputValue = "  25  ";

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(_viewModel.InputValue, Is.EqualTo("25"));
            Assert.That(_viewModel.IsValid, Is.True);
        }
    }

    // === MULTIPLE UPDATES TESTS ===

    [Test]
    public void Should_UpdateState_When_InputChangesMultipleTimes_Given_SequentialInputs()
    {
        // Act & Assert - Invalid input
        _viewModel.InputValue = "abc";
        using (Assert.EnterMultipleScope())
        {
            Assert.That(_viewModel.IsValid, Is.False);
            Assert.That(_viewModel.WarningInput, Is.EqualTo("Entrée incorrecte"));
        }

        // Act & Assert - Valid input
        _updateCalled = false;
        _viewModel.InputValue = "30";
        using (Assert.EnterMultipleScope())
        {
            Assert.That(_viewModel.IsValid, Is.True);
            Assert.That(_viewModel.WarningInput, Is.EqualTo(string.Empty));
            Assert.That(_updateCalled, Is.True);
        }

        // Act & Assert - Invalid age
        _updateCalled = false;
        _viewModel.InputValue = "150";
        using (Assert.EnterMultipleScope())
        {
            Assert.That(_viewModel.IsValid, Is.False);
            Assert.That(_viewModel.WarningInput, Is.EqualTo("Âge invalide"));
            Assert.That(_updateCalled, Is.True);
        }
    }

    [Test]
    public void Should_CallOnUpdateEachTime_When_InputChanges_Given_MultipleValidInputs()
    {
        // Arrange
        var callCount = 0;
        void CountingAction() => callCount++;
        var vm = new FieldPayBookingViewModel(_ticket, CountingAction);

        // Act
        vm.InputValue = "10";
        vm.InputValue = "20";
        vm.InputValue = "30";

        // Assert
        Assert.That(callCount, Is.EqualTo(3));
    }

    [Test]
    public void Should_NotCallOnUpdate_When_InputDoesNotChange_Given_SameValue()
    {
        // Arrange
        var callCount = 0;
        void CountingAction() => callCount++;
        var vm = new FieldPayBookingViewModel(_ticket, CountingAction);
        vm.InputValue = "25";
        callCount = 0; // Reset après la première assignation

        // Act
        vm.InputValue = "25"; // Même valeur

        // Assert
        Assert.That(callCount, Is.Zero);
    }

    // === PROPERTY CHANGE NOTIFICATION TESTS ===

    [Test]
    public void Should_RaisePropertyChanged_When_InputValueChanges_Given_NewValue()
    {
        // Arrange
        var propertyChanged = false;
        _viewModel.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(_viewModel.InputValue))
            {
                propertyChanged = true;
            }
        };

        // Act
        _viewModel.InputValue = "25";

        // Assert
        Assert.That(propertyChanged, Is.True);
    }

    [Test]
    public void Should_RaisePropertyChanged_When_IsValidChanges_Given_ValidationStateChange()
    {
        // Arrange
        var propertyChanged = false;
        _viewModel.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(_viewModel.IsValid))
            {
                propertyChanged = true;
            }
        };

        // Act
        _viewModel.InputValue = "25";

        // Assert
        Assert.That(propertyChanged, Is.True);
    }

    [Test]
    public void Should_RaisePropertyChanged_When_WarningInputChanges_Given_ValidationStateChange()
    {
        // Arrange
        var propertyChanged = false;
        _viewModel.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(_viewModel.WarningInput))
            {
                propertyChanged = true;
            }
        };

        // Act
        _viewModel.InputValue = "abc";

        // Assert
        Assert.That(propertyChanged, Is.True);
    }

    [Test]
    public void Should_RaisePropertyChanged_When_PriceChanges_Given_ValidationStateChange()
    {
        // Arrange
        var propertyChanged = false;
        _viewModel.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(_viewModel.Price))
            {
                propertyChanged = true;
            }
        };

        // Act
        _viewModel.InputValue = "25";

        // Assert
        Assert.That(propertyChanged, Is.True);
    }

    // === TICKET VALUE TESTS ===

    [Test]
    public void Should_UseTicketValue_When_AgeIsValid_Given_TicketWithCustomPrice()
    {
        // Arrange
        var customTicket = Substitute.For<ITicket>();
        customTicket.Seat.Returns(new Seat(1, 1));
        customTicket.Value.Returns(15.75);
        var vm = new FieldPayBookingViewModel(customTicket, () => { });

        // Act
        vm.InputValue = "30";

        // Assert
        Assert.That(vm.Price, Is.EqualTo(15.75));
    }

    [Test]
    public void Should_UpdatePriceFromTicket_When_ValidAgeEntered_Given_DifferentTicketValues()
    {
        // Arrange
        _ticket.Value.Returns(8.50);

        // Act
        _viewModel.InputValue = "12";

        // Assert
        Assert.That(_viewModel.Price, Is.EqualTo(8.50));
    }

    // === EDGE CASES TESTS ===

    [Test]
    public void Should_HandleLargeValidAge_When_InputIs120_Given_BoundaryValue()
    {
        // Act
        _viewModel.InputValue = "120";

        // Assert
        Assert.That(_viewModel.IsValid, Is.True);
        _ticket.Received().Age = 120;
        Assert.That(_viewModel.Price, Is.EqualTo(12.50));
    }

    [Test]
    public void Should_HandleSmallValidAge_When_InputIs1_Given_BoundaryValue()
    {
        // Act
        _viewModel.InputValue = "1";

        // Assert
        Assert.That(_viewModel.IsValid, Is.True);
        _ticket.Received().Age = 1;
        Assert.That(_viewModel.Price, Is.EqualTo(12.50));
    }

    [Test]
    public void Should_InvalidateTicketMultipleTimes_When_InvalidInputsGiven_Given_MultipleInvalidations()
    {
        // Act
        _viewModel.InputValue = ""; // doesn't count
        _viewModel.InputValue = "abc";
        _viewModel.InputValue = "-5";

        // Assert
        _ticket.Received(2).Invalidate();
    }
}
