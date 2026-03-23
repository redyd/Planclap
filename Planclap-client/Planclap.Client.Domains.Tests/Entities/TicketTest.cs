using Planclap.Client.Domains.core;
using Planclap.Client.Domains.Entities;

namespace Planclap.Client.Domains.Tests.Entities;

/// <summary>
///     Réalisé à l'aide de l'IA
/// </summary>
[TestFixture]
public class TicketTest
{
    [TestCase(TicketType.Child, 7.5, true)]
    [TestCase(TicketType.Senior, 8.0, true)]
    [TestCase(TicketType.Normal, 10.0, true)]
    [TestCase(TicketType.Empty, 0.0, false)]
    public void Should_Return_Correct_Value_And_Validity_When_TicketType_Given(TicketType type, double expectedValue, bool expectedValid)
    {
        var seat = new Seat(1, 1);
        var ticket = new Ticket(type, seat);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(ticket.Value, Is.EqualTo(expectedValue));
            Assert.That(ticket.ValidTicket, Is.EqualTo(expectedValid));
            Assert.That(ticket.Seat, Is.EqualTo(seat));
            Assert.That(ticket.TicketType, Is.EqualTo(type));
        }
    }

    [Test]
    public void Should_Create_Empty_Ticket_When_Constructor_Given_Seat_Only()
    {
        var seat = new Seat(2, 3);
        var ticket = new Ticket(seat);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(ticket.TicketType, Is.EqualTo(TicketType.Empty));
            Assert.That(ticket.ValidTicket, Is.False);
            Assert.That(ticket.Seat, Is.EqualTo(seat));
            Assert.That(ticket.Value, Is.Zero);
        }
    }

    [Test]
    public void Should_Return_Correct_ToString_When_TicketType_And_Seat_Given()
    {
        var seat = new Seat(1, 2);
        var ticket = new Ticket(TicketType.Normal, seat);

        var expected = $"Ticket: type={TicketType.Normal}, seat={seat}, valid={ticket.ValidTicket}";
        Assert.That(ticket.ToString(), Is.EqualTo(expected));
    }

    [Test]
    public void Should_Allow_Changing_TicketType_When_Setter_Called()
    {
        var seat = new Seat(3, 4);
        var ticket = new Ticket(TicketType.Child, seat) { Age = 65 };

        using (Assert.EnterMultipleScope())
        {
            Assert.That(ticket.TicketType, Is.EqualTo(TicketType.Senior));
            Assert.That(ticket.ValidTicket, Is.True);
            Assert.That(ticket.Value, Is.EqualTo(8.0));
        }
    }
}
