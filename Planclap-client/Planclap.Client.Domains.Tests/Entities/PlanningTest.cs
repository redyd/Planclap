using NSubstitute;
using Planclap.Client.Domains.core;
using Planclap.Client.Domains.Entities;
using Planclap.Client.Domains.Exception;
using Planclap.Client.Domains.IEntities;
using Planclap.Client.Domains.Services;

namespace Planclap.Client.Domains.Tests.Entities;

[TestFixture]
public class PlanningTest
{
    private readonly MovieTheater _room = new(10, 10);
    private readonly ITimeService _today = new TimeService(new DateTime(2025, 1, 1, 10, 0, 0));

    private static MovieSession CreateSession(DateTime start)
    {
        var scheduled = new Scheduled(start, TimeSpan.FromHours(2));
        var movie = MovieSupplier.Supply($"slug-{start:HHmm}", $"Title {start:HH:mm}");
        return new MovieSession(scheduled, movie);
    }

    [Test]
    public void ResetPlanning_Should_Sort_And_Add_Movies()
    {
        var planning = new Planning(_room);
        var later = CreateSession(new DateTime(2025, 1, 2));
        var earlier = CreateSession(new DateTime(2025, 1, 1));
        var movies = new List<MovieSession> { later, earlier };

        planning.ResetPlanning(movies);

        Assert.That(planning.Movies, Has.Count.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(planning.Movies[0], Is.EqualTo(earlier));
            Assert.That(planning.Movies[1], Is.EqualTo(later));
        }
    }

    [Test]
    public void ResetPlanning_Should_Throw_When_Seats_TooBig()
    {
        var planning = new Planning(_room);
        var scheduled = Substitute.For<IScheduled>();
        scheduled.SeatFitInTheater(_room).Returns(false);

        var movie = MovieSupplier.Supply("slug", "title");
        var session = new MovieSession(scheduled, movie);

        Assert.Throws<TheaterTooSmallException>(() => planning.ResetPlanning(new List<MovieSession> { session }));
    }

    [Test]
    public void UpdateTickets_Should_Add_Tickets_To_Correct_Session()
    {
        var planning = new Planning(_room);
        var session1 = CreateSession(_today.Current.AddHours(1));
        var session2 = CreateSession(_today.Current.AddHours(2));
        planning.ResetPlanning(new List<MovieSession> { session1, session2 });

        var ticket1 = Substitute.For<ITicket>();
        var ticket2 = Substitute.For<ITicket>();
        ticket1.Seat.Returns(new Seat(1, 1));
        ticket2.Seat.Returns(new Seat(2, 2));

        var reservation = Substitute.For<IReservation>();
        reservation.StartTime.Returns(session2.DateForSession);
        reservation.Tickets.Returns(new List<ITicket> { ticket1, ticket2 });

        planning.UpdateTickets(reservation);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(session2.Scheduled.SeatTaken(1, 1), Is.True);
            Assert.That(session2.Scheduled.SeatTaken(2, 2), Is.True);
        }
    }

    [Test]
    public void UpdateTickets_Should_Throw_When_Seat_AlreadyTaken()
    {
        var planning = new Planning(_room);
        var session = CreateSession(_today.Current);
        planning.ResetPlanning(new List<MovieSession> { session });

        var ticket = Substitute.For<ITicket>();
        var seat = new Seat(1, 1);
        ticket.Seat.Returns(seat);

        session.Scheduled.AddTicket(ticket); // seat déjà pris

        var reservation = Substitute.For<IReservation>();
        reservation.StartTime.Returns(session.DateForSession);
        reservation.Tickets.Returns(new List<ITicket> { ticket });

        Assert.Throws<ArgumentException>(() => planning.UpdateTickets(reservation));
    }

    [Test]
    public void Indexer_Should_Return_Correct_Session_By_Slug()
    {
        var planning = new Planning(_room);
        var session = CreateSession(_today.Current);
        planning.ResetPlanning(new List<MovieSession> { session });

        var result = planning[session.Movie.Slug];

        Assert.That(result, Is.EqualTo(session));
    }

    [Test]
    public void Movies_Property_Should_Return_ReadOnlyList()
    {
        var planning = new Planning(_room);
        var session = CreateSession(_today.Current);
        planning.ResetPlanning(new List<MovieSession> { session });

        Assert.That(() => ((IList<MovieSession>)planning.Movies).Add(session), Throws.TypeOf<NotSupportedException>());
    }

    [Test]
    public void Planning_Should_Expose_RoomSize()
    {
        var planning = new Planning(_room);
        Assert.That(planning.RoomSize, Is.EqualTo(_room));
    }

    [Test]
    public void ResetPlanning_Should_Clear_Previous_Movies()
    {
        var planning = new Planning(_room);
        var first = CreateSession(_today.Current);
        planning.ResetPlanning(new List<MovieSession> { first });

        var second = CreateSession(_today.Current.AddHours(1));
        planning.ResetPlanning(new List<MovieSession> { second });

        Assert.That(planning.Movies, Has.Count.EqualTo(1));
        Assert.That(planning.Movies[0], Is.EqualTo(second));
    }
}
