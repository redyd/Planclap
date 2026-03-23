using Planclap.Client.Domains.core;
using Planclap.Client.Domains.Entities;

namespace Planclap.Client.Infrastructures.Helpers;

public class TicketParser
{
    private static readonly Dictionary<string, TicketType> TypeMapping = new(StringComparer.OrdinalIgnoreCase) { { "n", TicketType.Normal }, { "c", TicketType.Child }, { "s", TicketType.Senior } };

    public IList<Ticket> ParseTickets(string reservations)
    {
        if (string.IsNullOrWhiteSpace(reservations))
        {
            return Array.Empty<Ticket>();
        }

        return reservations
            .Split('|', StringSplitOptions.RemoveEmptyEntries)
            .Select(ParseTicket)
            .ToList();
    }

    private Ticket ParseTicket(string reservation)
    {
        var parts = reservation.Split('=');

        if (parts.Length != 2 || parts.Any(string.IsNullOrEmpty))
        {
            throw new FormatException($"Invalid reservation format: {reservation}. Expected format: 'seat=type'");
        }

        var seat = ParseSeat(parts[0]);
        var isParsed = int.TryParse(parts[1], out var result);

        return isParsed
            ? new Ticket(seat, result)
            : new Ticket(ParseTicketType(parts[1]), seat);
    }

    private static Seat ParseSeat(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Seat value cannot be empty", nameof(value));
        }

        var parts = value.Split('-');

        if (parts.Length != 2)
        {
            throw new FormatException($"Invalid seat format: {value}. Expected format: 'row-column'");
        }

        if (!ushort.TryParse(parts[0], out var row))
        {
            throw new FormatException($"Invalid row number: {parts[0]}");
        }

        if (!ushort.TryParse(parts[1], out var column))
        {
            throw new FormatException($"Invalid column number: {parts[1]}");
        }

        return new Seat(row, column);
    }

    private static TicketType ParseTicketType(string value) =>
        string.IsNullOrWhiteSpace(value)
            ? TicketType.Normal
            : TypeMapping.GetValueOrDefault(value.Trim(), TicketType.Normal);
}
