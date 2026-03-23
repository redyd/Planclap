using NSubstitute;
using Planclap.Client.Domains.core;
using Planclap.Client.Domains.Entities;
using Planclap.Client.Domains.IEntities;

namespace Planclap.Client.Domains.Tests.Entities;

/// <summary>
///     Réalisé à l'aide de l'IA
/// </summary>
[TestFixture]
public class ReservationTest
{
    private static ITicket CreateTicket(ushort row, ushort col, double value = 10, bool valid = true)
    {
        var ticket = Substitute.For<ITicket>();
        ticket.Seat.Returns(new Seat(row, col));
        ticket.Value.Returns(value);
        ticket.ValidTicket.Returns(valid);
        ticket.When(x => x.Invalidate()).Do(x => ticket.TicketType.Returns(TicketType.Empty));
        ticket.Age = 20;
        return ticket;
    }

    private static MovieSession CreateMovieSession()
    {
        var movie = MovieSupplier.Supply("slug", "Title", "Desc");
        var scheduled = new Scheduled(DateTime.Now, TimeSpan.FromHours(2));
        return new MovieSession(scheduled, movie);
    }

    [Test]
    public void Should_Calculate_Reduction_When_TicketsCount_Given_VariousThresholds()
    {
        var session = CreateMovieSession();

        var reservation1 = new Reservation(session, new List<ITicket> { CreateTicket(1, 1), CreateTicket(1, 2) });
        Assert.That(reservation1.Reduction, Is.Zero);

        var reservation2 = new Reservation(session, new List<ITicket> { CreateTicket(1, 1), CreateTicket(1, 2), CreateTicket(1, 3) });
        Assert.That(reservation2.Reduction, Is.EqualTo(10));

        var reservation3 = new Reservation(session, new List<ITicket>
        {
            CreateTicket(1, 1),
            CreateTicket(1, 2),
            CreateTicket(1, 3),
            CreateTicket(1, 4),
            CreateTicket(1, 5)
        });
        Assert.That(reservation3.Reduction, Is.EqualTo(15));

        var reservation4 = new Reservation(session, new List<ITicket>
        {
            CreateTicket(1, 1),
            CreateTicket(1, 2),
            CreateTicket(1, 3),
            CreateTicket(1, 4),
            CreateTicket(1, 5),
            CreateTicket(2, 1),
            CreateTicket(2, 2),
            CreateTicket(2, 3),
            CreateTicket(2, 4),
            CreateTicket(2, 5)
        });
        Assert.That(reservation4.Reduction, Is.EqualTo(25));
    }

    [Test]
    public void Should_Calculate_Price_When_ReductionApplied()
    {
        var session = CreateMovieSession();
        var tickets = new List<ITicket> { CreateTicket(1, 1), CreateTicket(1, 2), CreateTicket(1, 3) }; // 3 tickets => 10%
        var reservation = new Reservation(session, tickets);

        var expected = 30 * 0.9;
        Assert.That(reservation.Price, Is.EqualTo(expected));
    }

    [Test]
    public void Should_Calculate_Price_When_ReductionIsZero()
    {
        var session = CreateMovieSession();
        var tickets = new List<ITicket> { CreateTicket(1, 1), CreateTicket(1, 2) }; // 2 tickets => 0%
        var reservation = new Reservation(session, tickets);

        var expected = 20;
        Assert.That(reservation.Price, Is.EqualTo(expected));
    }

    [Test]
    public void Should_Return_IsValid_When_AllTicketsValid()
    {
        var session = CreateMovieSession();
        var tickets = new List<ITicket> { CreateTicket(1, 1, valid: true), CreateTicket(1, 2, valid: true) };
        var reservation = new Reservation(session, tickets);

        Assert.That(reservation.IsValid, Is.True);

        tickets[0].ValidTicket.Returns(false);
        Assert.That(reservation.IsValid, Is.False);
    }

    [Test]
    public void Should_Expose_Tickets_As_ReadOnly()
    {
        var session = CreateMovieSession();
        var tickets = new List<ITicket> { CreateTicket(1, 1), CreateTicket(1, 2) };
        var reservation = new Reservation(session, tickets);

        Assert.That(reservation.Tickets, Is.EquivalentTo(tickets));
        Assert.That(reservation.Tickets, Is.InstanceOf<IReadOnlyList<ITicket>>());
    }

    [Test]
    public void Should_Return_Title_MinimumAge_StartTime()
    {
        var session = CreateMovieSession();
        var reservation = new Reservation(session, new List<ITicket> { CreateTicket(1, 1) });

        using (Assert.EnterMultipleScope())
        {
            Assert.That(reservation.Title, Is.EqualTo(session.Movie.Title));
            Assert.That(reservation.MinimumAge, Is.EqualTo(session.Movie.CineChecks.MinAge));
            Assert.That(reservation.StartTime, Is.EqualTo(session.DateForSession));
        }
    }

    [Test]
    public void Should_Return_Correct_StringRepresentation_When_ToStringCalled()
    {
        var session = CreateMovieSession();
        var ticket1 = CreateTicket(1, 1);
        var ticket2 = CreateTicket(1, 2);
        var reservation = new Reservation(session, new List<ITicket> { ticket1, ticket2 });

        var str = reservation.ToString();
        Assert.That(str, Does.Contain(session.Movie.Title.Value));
        Assert.That(str, Does.Contain("Normal:1-1"));
        Assert.That(str, Does.Contain("Normal:1-2"));
    }
}
