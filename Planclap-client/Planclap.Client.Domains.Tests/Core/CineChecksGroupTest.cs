using Planclap.Client.Domains.core;

namespace Planclap.Client.Domains.Tests.Core;

/// <summary>
///     Généré en partie à l'aide d'IA
/// </summary>
[TestFixture]
public class CineChecksGroupTest
{
    [Test]
    public void Should_Return_Correct_Size_When_Age_And_CineChecks_Given()
    {
        var age = new CineCheckAge("AL"); // Exemple de CineCheckAge
        var cineChecks = new List<CineCheck> { new("Violence"), new("Sexe") };

        var group = new CineChecksGroup(age, cineChecks);

        Assert.That(group.Size, Is.EqualTo(1 + cineChecks.Count));
    }

    [Test]
    public void Should_Return_MinAge_When_Age_Given()
    {
        var age = new CineCheckAge("AL");
        var cineChecks = new List<CineCheck>();

        var group = new CineChecksGroup(age, cineChecks);

        Assert.That(group.MinAge, Is.EqualTo(age.IntValue));
    }
}
