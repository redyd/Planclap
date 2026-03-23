using Planclap.Client.Domains.core;
using Planclap.Client.Domains.IEntities;

namespace Planclap.Client.Domains.IRepository;

public interface IPlanningRepository
{
    IDictionary<MovieSlug, IList<IScheduled>> FetchAllForToday();

    void UpdateScheduled(IReservation reservation);
}
