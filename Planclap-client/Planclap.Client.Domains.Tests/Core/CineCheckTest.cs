using Planclap.Client.Domains.core;

namespace Planclap.Client.Domains.Tests.Core;

/// <summary>
///     Réalisé à l'aide de l'IA
/// </summary>
[TestFixture]
public class CineCheckTest
{
    [TestCase("Violence", "violence")]
    [TestCase("Peur", "peur")]
    [TestCase("Sexe", "sex")]
    [TestCase("Paroles grossieres", "language")]
    [TestCase("Discrimination", "discrimination")]
    [TestCase("Drogues, alcool et fumer", "drugs-and-alcohol")]
    public void Should_Set_Correct_Id_When_Valid_Value_Given(string input, string expectedId)
    {
        var cineCheck = new CineCheck(input);

        Assert.That(cineCheck.Id, Is.EqualTo(expectedId));
    }

    [Test]
    public void Should_Throw_ArgumentException_When_Invalid_Value_Given()
    {
        const string invalidInput = "InvalidValue";

        var ex = Assert.Throws<ArgumentException>(() => _ = new CineCheck(invalidInput));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex!.Message, Is.Not.Null);
            Assert.That(ex.Message, Does.Contain("CineCheck 'InvalidValue' is not valid"));

            Assert.That(ex.ParamName, Is.Not.Null);
            Assert.That(ex.ParamName, Is.EqualTo("value"));
        }
    }
}
