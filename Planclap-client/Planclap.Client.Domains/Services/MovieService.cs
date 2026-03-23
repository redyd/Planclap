using Planclap.Client.Domains.Events;
using Planclap.Client.Domains.Exception;
using Planclap.Client.Domains.IEntities;
using Planclap.Client.Domains.IRepository;

namespace Planclap.Client.Domains.Services;

public class MovieService(
    IMovieRepository movieRepository,
    IPlanningRepository planningRepository,
    IPlanning planning) : IMovieService
{
    public event EventHandler<RepositoryErrorEventArgs>? RepositoryErrorEvent;

    public IPlanning FetchPlanning(bool refresh = true)
    {
        if (!refresh)
        {
            return planning;
        }

        try
        {
            // get planification from repo
            var scheduled = planningRepository.FetchAllForToday();

            // get movies from repo
            var movies = movieRepository.FetchAllBySlug(scheduled.Keys.ToHashSet());

            if (movies.Count == 0)
            {
                RaiseError("Un problème est survenu lors de la récupération des films");
                return planning;
            }

            // merge both
            var movieSessions = MovieServiceHelper.Merge(scheduled, movies);

            if (movieSessions.Count == 0)
            {
                RaiseError("Aucun planning pour aujourd'hui");
                return planning;
            }

            // add into planning
            planning.ResetPlanning(movieSessions);

            return planning;
        }
        catch (RepositoryException)
        {
            RaiseError("Une erreur est survenue lors de la récupération des films");
            return planning;
        }
    }

    public void NewReservation(IReservation reservation)
    {
        try
        {
            planningRepository.UpdateScheduled(reservation);
            planning.UpdateTickets(reservation);
            RaiseSuccess($"Réservation pour {reservation.Title.Value} correctement ajoutée!");
        }
        catch (RepositoryException)
        {
            RaiseError("Une erreur est survenue lors de la réservation");
        }
    }

    private void RaiseError(string error)
        => RepositoryErrorEvent?.Invoke(this, new RepositoryErrorEventArgs(error, 'e'));

    private void RaiseSuccess(string message)
        => RepositoryErrorEvent?.Invoke(this, new RepositoryErrorEventArgs(message, 's'));
}
