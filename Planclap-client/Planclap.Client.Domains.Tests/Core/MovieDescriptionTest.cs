using Planclap.Client.Domains.core;

namespace Planclap.Client.Domains.Tests.Core;

/// <summary>
///     Réalisé à l'aide de l'IA
/// </summary>
[TestFixture]
public class MovieDescriptionTest
{
    [Test]
    public void Should_Set_Value_Correctly_When_Valid_String_Given()
    {
        var description = "A valid movie description.";
        var movieDescription = new MovieDescription(description);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(movieDescription.Value, Is.EqualTo(description));
            Assert.That(movieDescription.ToString(), Is.EqualTo(description));
        }
    }

    [TestCase("")]
    [TestCase(" ")]
    public void Should_Throw_InvalidDataException_When_Empty_Or_Whitespace_String_Given(string invalidDescription)
    {
        var ex = Assert.Throws<InvalidDataException>(() => _ = new MovieDescription(invalidDescription));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ex!.Message, Is.Not.Null);
            Assert.That(ex.Message, Is.EqualTo("Description cannot be empty."));
        }
    }

    [Test]
    public void Should_Throw_InvalidDataException_When_String_Exceeds_200_Characters()
    {
        var longDescription = new string('a', 201);

        var ex = Assert.Throws<InvalidDataException>(() => _ = new MovieDescription(longDescription));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(ex!.Message, Is.Not.Null);
            Assert.That(ex.Message, Is.EqualTo("Description must be less than 200 characters."));
        }
    }
}
