using Planclap.Client.Domains.core;

namespace Planclap.Client.Domains.Tests.Core;

/// <summary>
///     Réalisé à l'aide de l'IA
/// </summary>
[TestFixture]
public class CineCheckAgeTest
{
    [TestCase("6")]
    [TestCase("9")]
    [TestCase("12")]
    [TestCase("14")]
    [TestCase("16")]
    [TestCase("18")]
    public void Should_Accept_Integer_When_In_CineCheck(string value)
        => Assert.That(() => new CineCheckAge(value), Throws.Nothing);

    [Test]
    public void Should_Accept_Al_String()
        => Assert.That(() => new CineCheckAge("al"), Throws.Nothing);

    [TestCase("5")]
    [TestCase("10")]
    [TestCase("13")]
    [TestCase("15")]
    [TestCase("19")]
    [TestCase("23")]
    [TestCase("-4")]
    public void Should_Not_Accept_Other_Integer_Value(string value)
        => Assert.That(() => new CineCheckAge(value), Throws.ArgumentException);

    [TestCase("6", 6)]
    [TestCase("9", 9)]
    [TestCase("12", 12)]
    [TestCase("14", 14)]
    [TestCase("16", 16)]
    [TestCase("18", 18)]
    [TestCase("AL", 0)]
    public void Should_Correspond_To_Its_String_And_Int_Value(string stringRep, int intRep)
    {
        using (Assert.EnterMultipleScope())
        {
            var check = new CineCheckAge(stringRep);
            Assert.That(check.IntValue, Is.EqualTo(intRep));
            Assert.That(check.StringValue, Is.EqualTo(stringRep.ToLower()));
        }
    }
}
