using Planclap.Client.Domains.Events;
using Planclap.Client.Domains.IEntities;

namespace Planclap.Client.Domains.Services;

public interface IMovieService
{
    /// <summary>
    ///     Fetch every movie.
    /// </summary>
    /// <param name="refresh">If we want to reload from data source.</param>
    /// <returns>A reference to the planning.</returns>
    IPlanning FetchPlanning(bool refresh = true);

    /// <summary>
    ///     Create a new reservation in the planning and in the data source.
    /// </summary>
    /// <param name="reservation">The new reservation.</param>
    void NewReservation(IReservation reservation);

    event EventHandler<RepositoryErrorEventArgs>? RepositoryErrorEvent;
}
