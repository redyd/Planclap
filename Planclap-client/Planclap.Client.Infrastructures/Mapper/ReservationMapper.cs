using Planclap.Client.Domains.core;
using Planclap.Client.Domains.Exception;
using Planclap.Client.Domains.IEntities;

namespace Planclap.Client.Infrastructures.Mapper;

public static class ReservationMapper
{
    public static string MapToDateTimeCsv(IReservation reservation)
    {
        var date = reservation.StartTime.ToString("yyyy-MM-dd");
        var startTime = reservation.StartTime.ToString("HH:mm");
        return $"{date},{startTime}";
    }

    public static string MapNewReservation(IReservation reservation, string reservationContent)
        => reservationContent.Length == 0
            ? MapReservation(reservation)
            : $"{reservationContent}|{MapReservation(reservation)}";

    private static string MapReservation(IReservation reservation)
    {
        var line = reservation.Tickets.Aggregate(string.Empty, (current, ticket)
            => current + $"{ticket.Seat.Row}-{ticket.Seat.Column}={GetIdOfTicketType(ticket.TicketType)}|");

        return line[..^1];
    }

    private static char GetIdOfTicketType(TicketType type)
        => type switch
        {
            TicketType.Normal => 'N',
            TicketType.Child => 'C',
            TicketType.Senior => 'S',
            TicketType.Empty => throw new InvalidResourcesException("file should not contain empty ticket type"),
            _ => throw new InvalidResourcesException("csv file is not valid (ticketType not found)")
        };
}
