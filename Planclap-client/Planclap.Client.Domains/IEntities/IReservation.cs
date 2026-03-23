using Planclap.Client.Domains.core;

namespace Planclap.Client.Domains.IEntities;

public interface IReservation
{
    /// <summary>
    ///     Gets the % of reduction the reservations offers.
    ///     <ul>
    ///         <li>+=10 -> 25%</li>
    ///         <li>+=5 -> 15%</li>
    ///         <li>+=3 -> 10%</li>
    ///         <li>-3 -> 0%</li>
    ///     </ul>
    /// </summary>
    int Reduction { get; }

    /// <summary>
    ///     Gets the total price for the reservation. Take into consideration the reduction.
    /// </summary>
    double Price { get; }

    /// <summary>
    ///     Gets a value indicating whether if the reservation is valid or not (all tickets must be valid).
    /// </summary>
    bool IsValid { get; }

    /// <summary>
    ///     Gets a read only collection of every ticket in the reservation.
    /// </summary>
    IReadOnlyList<ITicket> Tickets { get; }

    /// <summary>
    ///     Gets the title of the movie.
    /// </summary>
    MovieTitle Title { get; }

    /// <summary>
    ///     Gets the minimum age for the movie.
    /// </summary>
    int MinimumAge { get; }

    /// <summary>
    ///     Gets the start time for the reservation.
    /// </summary>
    DateTime StartTime { get; }
}
