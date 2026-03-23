using Planclap.Client.Domains.core;

namespace Planclap.Client.Domains.Tests.Core;

/// <summary>
///     Réalisé à l'aide de l'IA
/// </summary>
[TestFixture]
public class MovieSlugTests
{
    [TestCase("NormalSlug", "normalslug")]
    [TestCase("Titre Avec Espaces", "titre-avec-espaces")]
    [TestCase("Titre!Avec?Ponctuation.", "titre-avec-ponctuation")]
    [TestCase("Élément-à-test", "element-a-test")]
    [TestCase("  Multiple   Spaces  ", "multiple-spaces")]
    [TestCase("---StartsAndEnds---", "startsandends")]
    public void Should_Slugify_Correctly_When_String_Given(string input, string expected)
    {
        var slug = new MovieSlug(input);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(slug.Value, Is.EqualTo(expected));
            Assert.That(slug.ToString(), Is.EqualTo(expected));
        }
    }

    [TestCase("")]
    [TestCase(null)]
    public void Should_Return_Empty_String_When_Input_NullOrEmpty(string? input)
    {
        var slug = new MovieSlug(input!);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(slug.Value, Is.EqualTo(string.Empty));
            Assert.That(slug.ToString(), Is.EqualTo(string.Empty));
        }
    }
}
