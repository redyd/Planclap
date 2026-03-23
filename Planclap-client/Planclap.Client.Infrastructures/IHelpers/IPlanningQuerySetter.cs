using Planclap.Client.Domains.IEntities;

namespace Planclap.Client.Infrastructures.IHelpers;

public interface IPlanningQuerySetter
{
    ISqlWrapper ExecuteFetchAllForTodayQuery(ISqlWrapper db);

    ISqlWrapper ExecuteUpdateReservation(ISqlWrapper db, IReservation reservation);
}
