using System.Data.Common;
using Planclap.Client.Domains.core;
using Planclap.Client.Domains.Exception;
using Planclap.Client.Domains.IEntities;
using Planclap.Client.Domains.IRepository;
using Planclap.Client.Infrastructures.Helpers;
using Planclap.Client.Infrastructures.IHelpers;
using Planclap.Client.Infrastructures.Mapper;
using Serilog;

namespace Planclap.Client.Infrastructures.Implementations;

public class SqlPlanningRepository(
    DbProviderFactory factory,
    string connectionString,
    IScheduledMapper mapper,
    IPlanningQuerySetter querySetter,
    ILogger? logger = null) : IPlanningRepository
{
    public IDictionary<MovieSlug, IList<IScheduled>> FetchAllForToday()
    {
        try
        {
            logger?.Information("Fetching all scheduled movies for today from database");
            using var db = SqlWrapper.WithAutoCommit(factory, connectionString);

            var scheduledDto = querySetter
                .ExecuteFetchAllForTodayQuery(db)
                .ExecuteQuery(mapper.MapFromReader);

            var mapped = mapper.MapFromDto(scheduledDto);

            logger?.Information("Successfully fetched all schedules");

            return mapped;
        }
        catch (DbException e)
        {
            logger?.Warning("An error occured while fetching all scheduled movies");
            throw new DatabaseException("An error occured while fetching scheduled movies", e);
        }
        catch (Exception ex)
        {
            logger?.Error(ex, "Unexpected error fetching movies");
            throw;
        }
    }

    public void UpdateScheduled(IReservation reservation)
    {
        try
        {
            logger?.Information("Updating reservation for {Reservation}", reservation);
            using var db = SqlWrapper.WithTransaction(factory, connectionString);

            try
            {
                querySetter.ExecuteUpdateReservation(db, reservation).Commit();
                logger?.Information("Schedule updated successfully");
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }
        catch (DbException e)
        {
            logger?.Warning(e, "An error occured while updating reservation");
            throw new DatabaseException("An error occured while updating reservation", e);
        }
        catch (Exception ex)
        {
            logger?.Error(ex, "Unexpected error fetching movies");
            throw;
        }
    }
}
