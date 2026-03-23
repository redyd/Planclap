using System.Text.RegularExpressions;
using NSubstitute;
using Planclap.Client.Domains.core;
using Planclap.Client.Domains.Entities;
using Planclap.Client.Domains.Exception;
using Planclap.Client.Domains.IEntities;
using Planclap.Client.Domains.Services;
using Planclap.Client.Infrastructures.Exceptions;
using Planclap.Client.Infrastructures.Helpers;
using Planclap.Client.Infrastructures.Implementations;
using Planclap.Client.Infrastructures.Mapper;
using Serilog;

namespace Planclap.Client.Infrastructures.Tests.Implementations;

[TestFixture]
public class CsvPlanningFileRepositoryTest : FileTestHelper
{
    [SetUp]
    public void Setup()
    {
        TempDirectoryBasic = CreateTempDirectory();
        CreateTempFileInDirectory("planning.csv", TempDirectoryBasic, "2025-11-10.csv");

        TempDirectoryWithoutReservation = CreateTempDirectory();
        CreateTempFileInDirectory("planning-without-reservations-col.csv", TempDirectoryWithoutReservation, "2025-11-10.csv");

        TempDirectoryWithoutHeader = CreateTempDirectory();
        CreateTempFileInDirectory("planning-without-header.csv", TempDirectoryWithoutHeader, "2025-11-10.csv");

        TempDirectoryMissingCol = CreateTempDirectory();
        CreateTempFileInDirectory("planning-missing-col.csv", TempDirectoryMissingCol, "2025-11-10.csv");
    }

    private string TempDirectoryBasic { get; set; } = string.Empty;
    private string TempDirectoryWithoutReservation { get; set; } = string.Empty;
    private string TempDirectoryWithoutHeader { get; set; } = string.Empty;
    private string TempDirectoryMissingCol { get; set; } = string.Empty;

    private readonly ITimeService _time = new TimeService(new DateOnly(2025, 11, 10));

    [Test]
    public void Should_Throw_Exception_When_Path_Does_Not_Exist()
        => Assert.Throws(typeof(CouldNotReachDatasourceException), () => _ = new CsvPlanningFileRepository(@"c:\zieuduihz", _time, new CsvHelper(), new ScheduledMapper()));

    [Test]
    public void Should_Get_Every_Scheduled_Movie_From_Date_When_Target_Is_Same_Than_Original_Date()
    {
        var repo = new CsvPlanningFileRepository(TempDirectoryBasic, _time, new CsvHelper(), new ScheduledMapper());
        var result = repo.FetchAllForToday();

        Assert.That(result, Has.Count.EqualTo(5));
    }

    [Test]
    public void Should_Get_Every_Scheduled_Movie_When_Empty_Reservations()
    {
        var repo = new CsvPlanningFileRepository(TempDirectoryBasic, _time, new CsvHelper(), new ScheduledMapper());
        var result = repo.FetchAllForToday();

        Assert.That(result, Has.Count.EqualTo(5));
    }

    [Test]
    public void Should_Get_Every_Scheduled_Movie_When_File_Does_Not_Contains_Reservations()
    {
        var repo = new CsvPlanningFileRepository(TempDirectoryWithoutReservation, _time, new CsvHelper(), new ScheduledMapper());
        var result = repo.FetchAllForToday();

        Assert.That(result, Has.Count.EqualTo(5));
    }

    [Test]
    public void Should_Throws_Exception_When_No_Header()
    {
        var repo = new CsvPlanningFileRepository(TempDirectoryWithoutHeader, _time, new CsvHelper(), new ScheduledMapper());

        Assert.That(() => repo.FetchAllForToday(), Throws.TypeOf<InvalidCsvLineException>());
    }

    [Test]
    public void Should_Throws_Exception_When_Missing_Cols()
    {
        var repo = new CsvPlanningFileRepository(TempDirectoryMissingCol, _time, new CsvHelper(), new ScheduledMapper());

        Assert.That(() => repo.FetchAllForToday(), Throws.TypeOf<InvalidCsvLineException>());
    }

    [Test]
    public void Should_Add_Reservation_When_Columns_Does_Not_Exist()
    {
        var repo = new CsvPlanningFileRepository(TempDirectoryWithoutReservation, _time, new CsvHelper(), new ScheduledMapper());
        var fakeMovie = MovieSupplier.Supply("un-p-tit-truc-en-plus");

        // target the first scheduled movie
        var fakeScheduled = new Scheduled(
            new DateTime(2025, 11, 10, 12, 0, 0),
            TimeSpan.FromMinutes(60 + 39));

        var movieSession = new MovieSession(fakeScheduled, fakeMovie);
        var tickets = new List<ITicket> { new Ticket(TicketType.Normal, new Seat(0, 0)), new Ticket(TicketType.Child, new Seat(0, 1)), new Ticket(TicketType.Senior, new Seat(0, 2)) };

        var scheduled = new Reservation(movieSession, tickets);

        repo.UpdateScheduled(scheduled);

        var content = File.ReadAllText(Path.Combine(TempDirectoryWithoutReservation, "2025-11-10.csv"));

        var pattern = Regex.Escape("0-0=N|0-1=C|0-2=S");
        var occurrences = Regex.Matches(content, pattern).Count;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(occurrences, Is.EqualTo(1));
            Assert.That(content, Does.Contain("2025-11-10,12:00,13:39,un-p-tit-truc-en-plus,0-0=N|0-1=C|0-2=S"));
        }
    }

    [Test]
    public void Should_Add_Reservation_When_Columns_Exist_But_Targe_Is_Empty()
    {
        var repo = new CsvPlanningFileRepository(TempDirectoryBasic, _time, new CsvHelper(), new ScheduledMapper());
        var fakeMovie = MovieSupplier.Supply("vaiana-2");

        // target the scheduled movie
        var fakeScheduled = new Scheduled(
            new DateTime(2025, 11, 11, 12, 0, 0),
            TimeSpan.FromMinutes(60 + 40));

        var movieSession = new MovieSession(fakeScheduled, fakeMovie);
        var tickets = new List<ITicket> { new Ticket(TicketType.Child, new Seat(0, 4)), new Ticket(TicketType.Child, new Seat(1, 1)), new Ticket(TicketType.Senior, new Seat(3, 2)) };

        var scheduled = new Reservation(movieSession, tickets);

        repo.UpdateScheduled(scheduled);

        var content = File.ReadAllText(Path.Combine(TempDirectoryBasic, "2025-11-10.csv"));

        var pattern = Regex.Escape("0-4=C|1-1=C|3-2=S");
        var occurrences = Regex.Matches(content, pattern).Count;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(occurrences, Is.EqualTo(1));
            Assert.That(content, Does.Contain("2025-11-11,12:00,13:40,vaiana-2,0-4=C|1-1=C|3-2=S"));
        }
    }

    [Test]
    public void Should_Add_Reservation_When_Columns_Exist_And_Target_Contains_Reservations()
    {
        var repo = new CsvPlanningFileRepository(TempDirectoryBasic, _time, new CsvHelper(), new ScheduledMapper());
        var fakeMovie = MovieSupplier.Supply("le-comte-de-monte-cristo");

        // target the scheduled movie
        var fakeScheduled = new Scheduled(
            new DateTime(2025, 11, 10, 16, 30, 0),
            TimeSpan.FromMinutes(60 * 2 + 58));

        var movieSession = new MovieSession(fakeScheduled, fakeMovie);
        var tickets = new List<ITicket>
        {
            new Ticket(TicketType.Child, new Seat(0, 4)), new Ticket(TicketType.Child, new Seat(1, 1)), new Ticket(TicketType.Senior, new Seat(3, 2)), new Ticket(TicketType.Normal, new Seat(5, 2))
        };

        var scheduled = new Reservation(movieSession, tickets);

        repo.UpdateScheduled(scheduled);

        var content = File.ReadAllText(Path.Combine(TempDirectoryBasic, "2025-11-10.csv"));

        var pattern = Regex.Escape("2-0=N|2-1=C|2-2=S|2-3=S|0-4=C|1-1=C|3-2=S|5-2=N");
        var occurrences = Regex.Matches(content, pattern).Count;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(occurrences, Is.EqualTo(1));
            Assert.That(content, Does.Contain("2025-11-10,16:30,19:28,le-comte-de-monte-cristo,2-0=N|2-1=C|2-2=S|2-3=S|0-4=C|1-1=C|3-2=S|5-2=N"));
        }
    }

    [Test]
    public void Should_CallLogger_When_FetchAllForToday()
    {
        // Arrange
        var logger = Substitute.For<ILogger>();
        var repo = new CsvPlanningFileRepository(TempDirectoryBasic, _time, new CsvHelper(), new ScheduledMapper(), logger);

        // Act
        _ = repo.FetchAllForToday();

        // Assert
        // Vérifie qu'au moins un appel à Information a été fait
        Assert.That(logger.ReceivedCalls().Any(c => c.GetMethodInfo().Name == "Information"), Is.True);
    }

    [Test]
    public void Should_CallLogger_When_UpdateScheduled()
    {
        // Arrange
        var logger = Substitute.For<ILogger>();
        var repo = new CsvPlanningFileRepository(TempDirectoryBasic, _time, new CsvHelper(), new ScheduledMapper(), logger);

        var fakeMovie = MovieSupplier.Supply("vaiana-2");
        var fakeScheduled = new Scheduled(
            new DateTime(2025, 11, 11, 12, 0, 0),
            TimeSpan.FromMinutes(60 + 40));
        var movieSession = new MovieSession(fakeScheduled, fakeMovie);
        var tickets = new List<ITicket> { new Ticket(TicketType.Child, new Seat(0, 4)) };
        var scheduled = new Reservation(movieSession, tickets);

        // Act
        repo.UpdateScheduled(scheduled);

        // Assert
        Assert.That(logger.ReceivedCalls().Any(c => c.GetMethodInfo().Name == "Information"), Is.True);
    }


}
