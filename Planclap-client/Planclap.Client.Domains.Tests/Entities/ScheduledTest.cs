using NSubstitute;
using Planclap.Client.Domains.core;
using Planclap.Client.Domains.Entities;
using Planclap.Client.Domains.IEntities;

namespace Planclap.Client.Domains.Tests.Entities;

/// <summary>
///     Réalisé à l'aide de l'IA
/// </summary>
[TestFixture]
public class ScheduledTest
{
    [SetUp]
    public void Setup()
        => _scheduled = new Scheduled(DateTime.Now.AddHours(1), TimeSpan.FromHours(2));

    private Scheduled _scheduled = new(DateTime.Now.AddHours(1), TimeSpan.FromHours(2));

    private static ITicket CreateTicket(ushort row, ushort col)
    {
        var ticket = Substitute.For<ITicket>();
        ticket.Seat.Returns(new Seat(row, col));
        return ticket;
    }

    [Test]
    public void Should_Return_False_When_SeatTaken_Called_On_Empty_Scheduled_Given_Row_And_Col()
    {
        var result = _scheduled.SeatTaken(1, 1);
        Assert.That(result, Is.False);
    }

    [Test]
    public void Should_Return_True_When_SeatTaken_Called_On_Scheduled_With_Ticket_Given_Row_And_Col()
    {
        var ticket = CreateTicket(1, 1);
        _scheduled.AddTicket(ticket);

        var result = _scheduled.SeatTaken(1, 1);

        Assert.That(result, Is.True);
    }

    [Test]
    public void Should_Add_Ticket_To_Scheduled_When_Seat_Is_Not_Taken()
    {
        var ticket = CreateTicket(2, 2);
        _scheduled.AddTicket(ticket);

        Assert.That(_scheduled.SeatTaken(2, 2), Is.True);
    }

    [Test]
    public void Should_Throw_ArgumentException_When_AddTicket_Called_On_Taken_Seat()
    {
        var ticket1 = CreateTicket(1, 1);
        var ticket2 = CreateTicket(1, 1);

        _scheduled.AddTicket(ticket1);

        var ex = Assert.Throws<ArgumentException>(() => _scheduled.AddTicket(ticket2));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(ex!.Message, Is.Not.Null);
            Assert.That(ex.Message, Is.EqualTo("This ticket cannot be added: a ticket already exist for this seat"));
        }
    }

    [Test]
    public void Should_Return_Correct_StartTime_And_Duration()
    {
        var startTime = DateTime.Now.AddHours(3);
        var duration = TimeSpan.FromHours(1.5);
        var scheduled = new Scheduled(startTime, duration);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(startTime, Is.EqualTo(scheduled.StartTime));
            Assert.That(duration, Is.EqualTo(scheduled.Duration));
        }
    }

    [Test]
    public void Should_Return_True_When_Every_Seat_Fit_In_Theater()
    {
        var scheduled = new Scheduled(DateTime.Now.AddHours(1), TimeSpan.FromHours(2));
        scheduled.AddTicket(CreateTicket(0, 0));
        scheduled.AddTicket(CreateTicket(1, 0));
        scheduled.AddTicket(CreateTicket(2, 0));
        scheduled.AddTicket(CreateTicket(1, 1));
        scheduled.AddTicket(CreateTicket(2, 2));

        var theater = new MovieTheater(2, 2);
        var smallTheater = new MovieTheater(1, 1);
        var colSmallTheater = new MovieTheater(2, 1);
        var rowSmallTheater = new MovieTheater(1, 2);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(scheduled.SeatFitInTheater(theater), Is.True);
            Assert.That(scheduled.SeatFitInTheater(smallTheater), Is.False);
            Assert.That(scheduled.SeatFitInTheater(colSmallTheater), Is.False);
            Assert.That(scheduled.SeatFitInTheater(rowSmallTheater), Is.False);
        }
    }
}
