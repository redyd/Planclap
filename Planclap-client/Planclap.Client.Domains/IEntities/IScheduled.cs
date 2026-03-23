using Planclap.Client.Domains.core;

namespace Planclap.Client.Domains.IEntities;

public interface IScheduled
{
    DateTime StartTime { get; }

    TimeSpan Duration { get; }

    /// <summary>
    ///     Determines if a seat is already taken.
    /// </summary>
    /// <param name="row">The row of the seat.</param>
    /// <param name="col">The column of the seat.</param>
    /// <returns>true if taken, otherwise false.</returns>
    bool SeatTaken(int row, int col);

    /// <summary>
    ///     Adds a ticket to the scheduled session.
    ///     Throws an exception if the seat is already taken.
    /// </summary>
    /// <param name="ticket">The ticket to add.</param>
    void AddTicket(ITicket ticket);

    /// <summary>
    ///     Checks if every seat fit in the theater.
    /// </summary>
    /// <param name="theater">The theater to check.</param>
    /// <returns>true if it fits, otherwise false.</returns>
    bool SeatFitInTheater(MovieTheater theater);
}
