using Planclap.Client.Domains.core;

namespace Planclap.Client.Domains.IEntities;

public interface IPlanning
{
    MovieTheater RoomSize { get; }

    IReadOnlyList<MovieSession> Movies { get; }

    MovieSession this[MovieSlug slug] { get; }

    void ResetPlanning(IList<MovieSession> movies);

    void UpdateTickets(IReservation reservation);
}
