using Planclap.Client.Domains.core;
using Planclap.Client.Domains.IEntities;

namespace Planclap.Client.Domains.Entities;

public class Ticket : ITicket
{
    private int _age;

    public Ticket(Seat seat) : this(TicketType.Empty, seat)
    {
    }

    public Ticket(Seat seat, int age)
    {
        Seat = seat;
        Age = age;
    }

    public Ticket(TicketType ticketType, Seat seat)
    {
        TicketType = ticketType;
        Seat = seat;
    }

    public TicketType TicketType { get; private set; }

    public int Age
    {
        get => _age;
        set
        {
            if (value <= 0)
            {
                _age = 0;
                TicketType = TicketType.Empty;
                return;
            }

            _age = value;
            TicketType =
                value <= 12 ? TicketType.Child
                : value >= 60 ? TicketType.Senior
                : TicketType.Normal;
        }
    }

    public Seat Seat { get; }

    public void Invalidate()
    {
        Age = -1;
        TicketType = TicketType.Empty;
    }

    public double Value => TicketType switch
    {
        TicketType.Child => 7.5,
        TicketType.Senior => 8.0,
        TicketType.Normal => 10.0,
        _ => 0
    };

    public bool ValidTicket => TicketType != TicketType.Empty;

    public override string ToString() => $"Ticket: type={TicketType}, seat={Seat}, valid={ValidTicket}";
}
