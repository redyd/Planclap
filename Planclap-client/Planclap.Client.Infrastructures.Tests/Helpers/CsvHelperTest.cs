using NSubstitute;
using Planclap.Client.Domains.IEntities;
using Planclap.Client.Infrastructures.Dto;
using Planclap.Client.Infrastructures.Helpers;
using Planclap.Client.Infrastructures.IHelpers;

namespace Planclap.Client.Infrastructures.Tests.Helpers;

/// <summary>
///     Réalisé à l'aide de l'IA
/// </summary>
[TestFixture]
public class CsvHelperTest
{
    [SetUp]
    public void SetUp()
    {
        _mockReader = Substitute.For<ICsvReaderHelper>();
        _mockWriter = Substitute.For<ICsvWriterHelper>();

        _csvHelper = new CsvHelper(_mockReader, _mockWriter);
    }

    private ICsvReaderHelper _mockReader;
    private ICsvWriterHelper _mockWriter;
    private CsvHelper _csvHelper;

    public CsvHelperTest()
    {
        _mockReader = Substitute.For<ICsvReaderHelper>();
        _mockWriter = Substitute.For<ICsvWriterHelper>();

        _csvHelper = new CsvHelper(_mockReader, _mockWriter);
    }

    [Test]
    public void Should_Call_ReadSchedules_When_ReadSchedules_Called()
    {
        var filePath = "dummy.csv";
        var expectedSchedules = new List<CsvScheduledDto> { new("", "", "", "", "") };
        _mockReader.ReadSchedules(filePath).Returns(expectedSchedules);

        var result = _csvHelper.ReadSchedules(filePath);

        Assert.That(result, Is.EqualTo(expectedSchedules));
        _mockReader.Received(1).ReadSchedules(filePath);
    }

    [Test]
    public void Should_Call_UpdateReservation_When_UpdateReservation_Called()
    {
        var filePath = "dummy.csv";
        var targetDate = "2025-11-12";
        var reservation = Substitute.For<IReservation>();

        _csvHelper.UpdateReservation(filePath, targetDate, reservation);

        _mockWriter.Received(1).UpdateReservation(filePath, targetDate, reservation);
    }
}
