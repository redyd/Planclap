using Planclap.Client.Domains.core;
using Planclap.Client.Domains.IEntities;

namespace Planclap.Client.Domains.Entities;

public class Scheduled(DateTime startTime, TimeSpan duration) : IScheduled
{
    private readonly List<ITicket> _tickets = [];

    public DateTime StartTime { get; } = startTime;

    public TimeSpan Duration { get; } = duration;

    public bool SeatTaken(int row, int col)
        => _tickets.Any(ticket => ticket.Seat.Row == row && ticket.Seat.Column == col);

    public void AddTicket(ITicket ticket)
    {
        if (DoesSeatExists(ticket.Seat))
        {
            throw new ArgumentException("This ticket cannot be added: a ticket already exist for this seat");
        }

        _tickets.Add(ticket);
    }

    public bool SeatFitInTheater(MovieTheater theater)
        => _tickets.All(t => t.Seat.Row <= theater.Depth && t.Seat.Column <= theater.Width);

    private bool DoesSeatExists(Seat seat) =>
        _tickets.Any(x => x.Seat == seat);
}
