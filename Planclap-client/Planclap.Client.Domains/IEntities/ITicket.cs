using Planclap.Client.Domains.core;

namespace Planclap.Client.Domains.IEntities;

public interface ITicket
{
    /// <summary>
    ///     Gets the ticket type (children | normal | senior | empty).
    /// </summary>
    TicketType TicketType { get; }

    /// <summary>
    ///     Gets or sets and sets age and ticket type.
    /// </summary>
    int Age { get; set; }

    /// <summary>
    ///     Gets the seat associated for the ticket.
    /// </summary>
    Seat Seat { get; }

    /// <summary>
    ///     Gets the ticket value.
    /// </summary>
    double Value { get; }

    /// <summary>
    ///     Gets a value indicating whether the ticket is valid or not.
    /// </summary>
    bool ValidTicket { get; }

    /// <summary>
    ///     Invalidate the current ticket.
    /// </summary>
    void Invalidate();
}
