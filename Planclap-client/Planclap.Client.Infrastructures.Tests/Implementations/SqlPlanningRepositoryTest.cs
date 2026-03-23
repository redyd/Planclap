using System.Data.Common;
using Microsoft.Data.Sqlite;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Planclap.Client.Domains.core;
using Planclap.Client.Domains.Entities;
using Planclap.Client.Domains.Exception;
using Planclap.Client.Domains.IEntities;
using Planclap.Client.Domains.Services;
using Planclap.Client.Infrastructures.Helpers;
using Planclap.Client.Infrastructures.IHelpers;
using Planclap.Client.Infrastructures.Implementations;
using Planclap.Client.Infrastructures.Mapper;
using Serilog;

namespace Planclap.Client.Infrastructures.Tests.Implementations;

[TestFixture]
public class SqlPlanningRepositoryTests : FileTestHelper
{
    [SetUp]
    public void Setup()
    {
        var tempDirectory = CreateTempDirectory();
        _tempDbPath = CreateTempFileInDirectory("planclap-test.sqlite", tempDirectory, "test.db");

        _connectionString = $"Data Source={_tempDbPath}";
        _factory = SqliteFactory.Instance;
        _query = new QuerySetter(new TimeService(new DateOnly(2025, 11, 11)));

        _repository = new SqlPlanningRepository(_factory, _connectionString, new ScheduledMapper(), _query);
    }

    private DbProviderFactory _factory = null!;
    private SqlPlanningRepository _repository = null!;
    private IQuerySetter _query = null!;
    private string _tempDbPath = null!;
    private string _connectionString = null!;

    [Test]
    public void Should_Get_Every_Reservation_Thursday_11()
    {
        // im expecting 4 distincts movies and a total of 5
        /*
         * 12:00	Vaiana 2		        2 réservations	4 + 2 = 6 places
         * 13:40	Dune : Deuxième Partie	3 réservations	2 + 2 + 3 = 7 places -> 6-18, 6-19, 6-20, 7-15, 7-16, 8-10, 8-11
         * 16:31	Vaiana 2		        2 réservations	6 + 3 = 9 places
         * 18:25	Kung Fu Panda 4		    2 réservations	7 + 3 = 10 places
         * 20:04	Vice-Versa 2		    0 réservation
         */
        var result = _repository.FetchAllForToday();

        // checking for dune tickets
        var dune = result[new MovieSlug("Dune : Deuxième Partie")];
        var expectedSeats = new (int row, int col)[] { (6, 18), (6, 19), (6, 20), (7, 15), (7, 16), (8, 10), (8, 11) };

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Keys, Has.Count.EqualTo(4));
            Assert.That(result[new MovieSlug("Vaiana 2")], Has.Count.EqualTo(2)); // 2 differents scheduled
            Assert.That(result[new MovieSlug("Dune : Deuxième Partie")], Has.Count.EqualTo(1));
            Assert.That(result[new MovieSlug("Kung Fu Panda 4")], Has.Count.EqualTo(1));
            Assert.That(result[new MovieSlug("Vice-Versa 2")], Has.Count.EqualTo(1));
        }

        using (Assert.EnterMultipleScope())
        {
            foreach (var (row, col) in expectedSeats)
            {
                Assert.That(
                    dune.Any(sch => sch.SeatTaken(row, col)),
                    Is.True,
                    $"Seat {row}-{col} should be taken but was not found."
                );
            }
        }
    }

    [Test]
    public void Should_Update_The_Reservation_When_Empty_Reservation()
    {
        // im expecting 4 distincts movies and a total of 5
        /*
         * 12:00	Vaiana 2		        2 réservations	4 + 2 = 6 places
         * 13:40	Dune : Deuxième Partie	3 réservations	2 + 2 + 3 = 7 places -> 6-18, 6-19, 6-20, 7-15, 7-16, 8-10, 8-11
         * 16:31	Vaiana 2		        2 réservations	6 + 3 = 9 places
         * 18:25	Kung Fu Panda 4		    2 réservations	7 + 3 = 10 places
         * 20:04	Vice-Versa 2		    0 réservation
         */
        var dateExpected = new DateOnly(2025, 11, 11);

        IReadOnlyList<Ticket> ticketsList =
        [
            new(new Seat(0, 0), 13),
            new(new Seat(0, 1), 25),
            new(new Seat(0, 2), 65)
        ];

        var reservation = Substitute.For<IReservation>();
        reservation.StartTime.Returns(dateExpected.ToDateTime(new TimeOnly(20, 04)));
        reservation.Title.Returns(new MovieTitle("Vice-Versa 2"));
        reservation.Tickets.Returns(ticketsList);

        var before = _repository.FetchAllForToday();

        _repository.UpdateScheduled(reservation);

        var after = _repository.FetchAllForToday();
        var viceAfter = after[new MovieSlug("Vice-Versa 2")].Single();

        Assert.That(before, Is.Not.EqualTo(after));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(viceAfter.SeatTaken(0, 0), Is.True);
            Assert.That(viceAfter.SeatTaken(0, 1), Is.True);
            Assert.That(viceAfter.SeatTaken(0, 2), Is.True);
        }
    }

    [Test]
    public void Should_Update_The_Reservation_When_Existing_Reservation()
    {
        // im expecting 4 distincts movies and a total of 5
        /*
         * 12:00	Vaiana 2		        2 réservations	4 + 2 = 6 places
         * 13:40	Dune : Deuxième Partie	3 réservations	2 + 2 + 3 = 7 places -> 6-18, 6-19, 6-20, 7-15, 7-16, 8-10, 8-11
         * 16:31	Vaiana 2		        2 réservations	6 + 3 = 9 places
         * 18:25	Kung Fu Panda 4		    2 réservations	7 + 3 = 10 places
         * 20:04	Vice-Versa 2		    0 réservation
         */
        var dateExpected = new DateOnly(2025, 11, 11);

        IReadOnlyList<Ticket> ticketsList =
        [
            new(new Seat(0, 0), 13),
            new(new Seat(0, 1), 25),
            new(new Seat(0, 2), 65)
        ];

        var reservation = Substitute.For<IReservation>();
        var expectedSeats = new (int row, int col)[] { (6, 18), (6, 19), (6, 20), (7, 15), (7, 16), (8, 10), (8, 11), (0, 0), (0, 1), (0, 2) };
        reservation.StartTime.Returns(dateExpected.ToDateTime(new TimeOnly(13, 40)));
        reservation.Title.Returns(new MovieTitle("Dune : Deuxième Partie"));
        reservation.Tickets.Returns(ticketsList);

        var before = _repository.FetchAllForToday();

        _repository.UpdateScheduled(reservation);

        var after = _repository.FetchAllForToday();
        var duneAfter = after[new MovieSlug("Dune : Deuxième Partie")].Single();

        Assert.That(before, Is.Not.EqualTo(after));

        using (Assert.EnterMultipleScope())
        {
            foreach (var (row, col) in expectedSeats)
            {
                Assert.That(
                    duneAfter.SeatTaken(row, col),
                    Is.True,
                    $"Seat {row}-{col} should be taken but was not found."
                );
            }
        }
    }

    [Test]
    public void Should_Throw_Database_Exception_When_FetchAllForToday()
    {
        _query = Substitute.For<IQuerySetter>();
        _query.ExecuteFetchAllForTodayQuery(Arg.Any<ISqlWrapper>()).Throws(new SqliteException("Something went wrong", -1));
        _repository = new SqlPlanningRepository(_factory, _connectionString, new ScheduledMapper(), _query);

        Assert.Throws<DatabaseException>(() => _repository.FetchAllForToday());
    }

    [Test]
    public void Should_Throw_Database_Exception_When_Update()
    {
        _query = Substitute.For<IQuerySetter>();
        _query.ExecuteUpdateReservation(Arg.Any<ISqlWrapper>(), Arg.Any<IReservation>()).Throws(new SqliteException("Something went wrong", -1));
        _repository = new SqlPlanningRepository(_factory, _connectionString, new ScheduledMapper(), _query);

        Assert.Throws<DatabaseException>(() => _repository.UpdateScheduled(null!));
    }

    [Test]
    public void Should_Call_Logger_On_FetchAllForToday_Success()
    {
        var logger = Substitute.For<ILogger>();
        _repository = new SqlPlanningRepository(_factory, _connectionString, new ScheduledMapper(), _query, logger);

        _repository.FetchAllForToday();

        logger.Received().Information("Fetching all scheduled movies for today from database");
        logger.Received().Information("Successfully fetched all schedules");
    }

    [Test]
    public void Should_Call_Logger_On_FetchAllForToday_DbException()
    {
        var logger = Substitute.For<ILogger>();
        var query = Substitute.For<IPlanningQuerySetter>();
        query.ExecuteFetchAllForTodayQuery(Arg.Any<ISqlWrapper>()).Throws(new SqliteException("oops", -1));
        _repository = new SqlPlanningRepository(_factory, _connectionString, new ScheduledMapper(), query, logger);

        Assert.Throws<DatabaseException>(() => _repository.FetchAllForToday());

        logger.Received().Warning("An error occured while fetching all scheduled movies");
    }

    [Test]
    public void Should_Call_Logger_On_FetchAllForToday_GenericException()
    {
        var logger = Substitute.For<ILogger>();
        var query = Substitute.For<IPlanningQuerySetter>();
        query.ExecuteFetchAllForTodayQuery(Arg.Any<ISqlWrapper>()).Throws(new Exception("oops"));
        _repository = new SqlPlanningRepository(_factory, _connectionString, new ScheduledMapper(), query, logger);

        Assert.Throws<Exception>(() => _repository.FetchAllForToday());

        logger.Received().Error(Arg.Any<Exception>(), "Unexpected error fetching movies");
    }

    [Test]
    public void Should_Call_Logger_On_UpdateScheduled_Success()
    {
        var logger = Substitute.For<ILogger>();
        var reservation = Substitute.For<IReservation>();
        var query = Substitute.For<IPlanningQuerySetter>();
        var tx = Substitute.For<ISqlWrapper>();

        query.ExecuteUpdateReservation(Arg.Any<ISqlWrapper>(), reservation).Returns(tx);

        _repository = new SqlPlanningRepository(_factory, _connectionString, new ScheduledMapper(), query, logger);
        _repository.UpdateScheduled(reservation);

        logger.Received().Information("Updating reservation for {Reservation}", reservation);
        logger.Received().Information("Schedule updated successfully");
    }

    [Test]
    public void Should_Call_Logger_On_UpdateScheduled_DbException()
    {
        var logger = Substitute.For<ILogger>();
        var reservation = Substitute.For<IReservation>();
        var query = Substitute.For<IPlanningQuerySetter>();

        query.ExecuteUpdateReservation(Arg.Any<ISqlWrapper>(), reservation)
            .Throws(new SqliteException("oops", -1));

        _repository = new SqlPlanningRepository(_factory, _connectionString, new ScheduledMapper(), query, logger);

        Assert.Throws<DatabaseException>(() => _repository.UpdateScheduled(reservation));
        logger.Received().Warning(Arg.Any<DbException>(), "An error occured while updating reservation");
    }

    [Test]
    public void Should_Call_Logger_On_UpdateScheduled_GenericException()
    {
        var logger = Substitute.For<ILogger>();
        var reservation = Substitute.For<IReservation>();
        var query = Substitute.For<IPlanningQuerySetter>();
        query.ExecuteUpdateReservation(Arg.Any<ISqlWrapper>(), reservation)
            .Throws(new Exception("oops"));

        _repository = new SqlPlanningRepository(_factory, _connectionString, new ScheduledMapper(), query, logger);

        Assert.Throws<Exception>(() => _repository.UpdateScheduled(reservation));
        logger.Received().Error(Arg.Any<Exception>(), "Unexpected error fetching movies");
    }
}
