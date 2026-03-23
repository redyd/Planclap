using Planclap.Client.Infrastructures.Helpers;

namespace Planclap.Client.Infrastructures.Tests.Helpers;

/// <summary>
/// Classe générée par IA
/// </summary>
[TestFixture]
public class CineCheckHelperTests
{
    [Test]
    public void Should_Split_When_Age_Is_First_Element()
    {
        // Arrange
        var input = new List<string> { "12", "VIOLENCE", "PEUR" };

        // Act
        var (age, others) = CineCheckHelper.SplitAgeAndOther(input);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(age, Is.EqualTo("12"));
            Assert.That(others, Has.Count.EqualTo(2));
            Assert.That(others, Does.Contain("VIOLENCE"));
            Assert.That(others, Does.Contain("PEUR"));
        }
    }

    [Test]
    public void Should_Split_When_Age_Is_In_Middle()
    {
        // Arrange
        var input = new List<string> { "VIOLENCE", "14", "PEUR" };

        // Act
        var (age, others) = CineCheckHelper.SplitAgeAndOther(input);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(age, Is.EqualTo("14"));
            Assert.That(others, Has.Count.EqualTo(2));
            Assert.That(others, Does.Contain("VIOLENCE"));
            Assert.That(others, Does.Contain("PEUR"));
        }
    }

    [Test]
    public void Should_Split_When_Age_Is_Last_Element()
    {
        // Arrange
        var input = new List<string> { "VIOLENCE", "PEUR", "16" };

        // Act
        var (age, others) = CineCheckHelper.SplitAgeAndOther(input);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(age, Is.EqualTo("16"));
            Assert.That(others, Has.Count.EqualTo(2));
            Assert.That(others, Does.Not.Contain("16"));
        }
    }

    [Test]
    public void Should_Throw_When_No_Valid_Age_Found()
    {
        // Arrange
        var input = new List<string> { "VIOLENCE", "PEUR", "LANGAGE" };

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            CineCheckHelper.SplitAgeAndOther(input));

        Assert.That(ex!.Message, Is.EqualTo("Input string was not in a correct format."));
    }
}
