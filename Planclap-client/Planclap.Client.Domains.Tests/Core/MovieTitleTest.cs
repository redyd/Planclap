using Planclap.Client.Domains.core;

namespace Planclap.Client.Domains.Tests.Core;

/// <summary>
///     Réalisé à l'aide de l'IA
/// </summary>
[TestFixture]
public class MovieTitleTest
{
    [Test]
    public void Should_Set_Value_Correctly_When_Valid_String_Given()
    {
        var input = "Inception";
        var title = new MovieTitle(input);

        Assert.That(title.Value, Is.EqualTo(input));
    }

    [TestCase("")]
    [TestCase(null)]
    public void Should_Throw_ArgumentNullException_When_NullOrEmpty_String_Given(string? invalidInput)
    {
        var ex = Assert.Throws<ArgumentNullException>(() => _ = new MovieTitle(invalidInput!));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(ex!.Message, Is.Not.Null);
            Assert.That(ex.ParamName, Is.EqualTo(invalidInput));
        }
    }
}
