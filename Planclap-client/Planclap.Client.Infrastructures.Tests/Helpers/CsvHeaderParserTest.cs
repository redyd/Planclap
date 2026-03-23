using Planclap.Client.Domains.Exception;
using Planclap.Client.Infrastructures.Helpers;

namespace Planclap.Client.Infrastructures.Tests.Helpers;

/// <summary>
///     Réalisé à l'aide de l'IA
/// </summary>
[TestFixture]
public class CsvHeaderParserTest
{
    [Test]
    public void Should_Return_Correct_HeaderDictionary_When_Valid_HeaderLine_Given()
    {
        const string headerLine = "Name,Date,Value";

        var result = CsvHeaderParser.Parse(headerLine);

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result["Name"], Is.Zero);
            Assert.That(result["Date"], Is.EqualTo(1));
            Assert.That(result["Value"], Is.EqualTo(2));
        }
    }

    [TestCase("")]
    [TestCase(" ")]
    public void Should_Throw_InvalidResourcesException_When_HeaderLine_EmptyOrNull(string invalidHeaderLine)
    {
        var ex = Assert.Throws<InvalidResourcesException>(() => CsvHeaderParser.Parse(invalidHeaderLine));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ex!.Message, Is.Not.Null);
            Assert.That(ex.Message, Is.EqualTo("CSV header is empty"));
        }
    }
}
