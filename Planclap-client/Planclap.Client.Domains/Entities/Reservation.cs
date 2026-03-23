using Planclap.Client.Domains.core;
using Planclap.Client.Domains.IEntities;

namespace Planclap.Client.Domains.Entities;

public class Reservation(MovieSession movieSession, IList<ITicket> tickets) : IReservation
{
    private readonly IList<ITicket> _tickets = new List<ITicket>(tickets);

    public int Reduction
    {
        get
        {
            var count = _tickets.Count;
            return count >= 10 ? 25 : count >= 5 ? 15 : count >= 3 ? 10 : 0;
        }
    }

    public double Price
    {
        get
        {
            var totalWithoutReduction = _tickets.Select(t => t.Value).Sum();
            if (Reduction == 0)
            {
                return totalWithoutReduction;
            }

            return totalWithoutReduction * (1 - (Reduction / 100.0));
        }
    }

    public bool IsValid => _tickets.All(ticket => ticket.ValidTicket) && tickets.Count > 0;

    public IReadOnlyList<ITicket> Tickets => _tickets.AsReadOnly();

    public MovieTitle Title => movieSession.Movie.Title;

    public int MinimumAge => movieSession.Movie.CineChecks.MinAge;

    public DateTime StartTime => movieSession.DateForSession;

    public override string ToString() =>
        $"Reservation for \"{movieSession.Movie.Title.Value}\" [{string.Join(", ", _tickets.Select(t => $"{t.TicketType}:{t.Seat.Row}-{t.Seat.Column}"))}]";
}
