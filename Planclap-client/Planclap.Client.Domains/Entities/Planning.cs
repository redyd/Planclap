using Planclap.Client.Domains.core;
using Planclap.Client.Domains.Exception;
using Planclap.Client.Domains.IEntities;

namespace Planclap.Client.Domains.Entities;

public class Planning(MovieTheater room) : IPlanning
{
    private readonly List<MovieSession> _planning = [];

    public MovieTheater RoomSize { get; } = room;

    public MovieSession this[MovieSlug slug] => _planning.Single(m => m.Movie.Slug == slug);

    public IReadOnlyList<MovieSession> Movies => _planning.AsReadOnly();

    /// <summary>
    ///     Update the tickets in the planning based on a new reservation.
    /// </summary>
    /// <param name="reservation">A new reservation.</param>
    public void UpdateTickets(IReservation reservation)
    {
        var start = reservation.StartTime;
        var scheduledTarget = _planning
            .Where(movie => movie.DateForSession == start)
            .Select(movie => movie.Scheduled)
            .First();

        foreach (var reservedTicket in reservation.Tickets)
        {
            scheduledTarget.AddTicket(reservedTicket);
        }
    }

    /// <summary>
    ///     Reset the movies in the planning. Sort the movies by date and time.
    /// </summary>
    /// <param name="movies">The new MovieSession list.</param>
    public void ResetPlanning(IList<MovieSession> movies)
    {
        _planning.Clear();
        movies = movies.OrderBy(x => x.DateForSession).ToList();
        if (movies.Any(movie => !movie.Scheduled.SeatFitInTheater(RoomSize)))
        {
            throw new TheaterTooSmallException("This theater is too small");
        }

        _planning.AddRange(movies);
    }
}
