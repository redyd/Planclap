using Planclap.Client.Domains.core;
using Planclap.Client.Domains.Exception;
using Planclap.Client.Domains.IEntities;
using Planclap.Client.Infrastructures.Mapper;
using NSubstitute;
using Planclap.Client.Domains.Entities;

namespace Planclap.Client.Infrastructures.Tests.Mapper;

[TestFixture]
public class ReservationMapperTests
{
    private IReservation _reservation = null!;

    [SetUp]
    public void SetUp() => _reservation = Substitute.For<IReservation>();

    [Test]
    public void Should_MapToDateTimeCsv_ReturnCorrectFormat_When_ValidReservation()
    {
        // Arrange
        var startTime = new DateTime(2025, 11, 10, 14, 30, 0);
        _reservation.StartTime.Returns(startTime);

        // Act
        var result = ReservationMapper.MapToDateTimeCsv(_reservation);

        // Assert
        Assert.That(result, Is.EqualTo("2025-11-10,14:30"));
    }

    [Test]
    public void Should_MapNewReservation_AppendReservation_When_ContentNotEmpty()
    {
        // Arrange
        var tickets = new List<Ticket>
        {
            new Ticket(TicketType.Normal, new Seat(0,0)),
            new Ticket(TicketType.Child, new Seat(1,1))
        };
        _reservation.Tickets.Returns(tickets);
        var reservationContent = "existing";

        // Act
        var result = ReservationMapper.MapNewReservation(_reservation, reservationContent);

        // Assert
        Assert.That(result, Is.EqualTo("existing|0-0=N|1-1=C"));
    }

    [Test]
    public void Should_MapNewReservation_ReturnReservation_When_ContentEmpty()
    {
        // Arrange
        var tickets = new List<Ticket>
        {
            new Ticket(TicketType.Normal, new Seat(0,0)),
            new Ticket(TicketType.Child, new Seat(1,1))
        };
        _reservation.Tickets.Returns(tickets);
        var reservationContent = "";

        // Act
        var result = ReservationMapper.MapNewReservation(_reservation, reservationContent);

        // Assert
        Assert.That(result, Is.EqualTo("0-0=N|1-1=C"));
    }

    [Test]
    public void Should_MapReservation_ReturnCorrectLine_ForAllTicketTypes()
    {
        // Arrange
        var tickets = new List<Ticket>
        {
            new Ticket(TicketType.Normal, new Seat(0,0)),
            new Ticket(TicketType.Child, new Seat(1,1)),
            new Ticket(TicketType.Senior, new Seat(2,2))
        };
        _reservation.Tickets.Returns(tickets);

        // Act
        var result = ReservationMapper.MapNewReservation(_reservation, "");

        // Assert
        Assert.That(result, Is.EqualTo("0-0=N|1-1=C|2-2=S"));
    }

    [Test]
    public void Should_Throw_When_TicketTypeEmpty()
    {
        // Arrange
        var tickets = new List<Ticket> { new Ticket(TicketType.Empty, new Seat(0,0)) };
        _reservation.Tickets.Returns(tickets);

        // Act & Assert
        var ex = Assert.Throws<InvalidResourcesException>(() => ReservationMapper.MapNewReservation(_reservation, ""));
        Assert.That(ex?.Message, Is.EqualTo("file should not contain empty ticket type"));
    }

    [Test]
    public void Should_Throw_When_TicketTypeUnknown()
    {
        // Arrange
        // Création d'un TicketType invalide (cast)
        var tickets = new List<Ticket> { new((TicketType)999, new Seat(0,0)) };
        _reservation.Tickets.Returns(tickets);

        // Act & Assert
        var ex = Assert.Throws<InvalidResourcesException>(() => ReservationMapper.MapNewReservation(_reservation, ""));
        Assert.That(ex?.Message, Is.EqualTo("csv file is not valid (ticketType not found)"));
    }
}
