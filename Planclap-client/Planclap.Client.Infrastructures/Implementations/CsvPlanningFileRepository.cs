using Planclap.Client.Domains.core;
using Planclap.Client.Domains.Exception;
using Planclap.Client.Domains.IEntities;
using Planclap.Client.Domains.IRepository;
using Planclap.Client.Domains.Services;
using Planclap.Client.Infrastructures.IHelpers;
using Planclap.Client.Infrastructures.Mapper;
using Serilog;

namespace Planclap.Client.Infrastructures.Implementations;

public class CsvPlanningFileRepository(
    string directory,
    ITimeService time,
    ICsvHelper csvHelper,
    IScheduledMapper mapper,
    ILogger? logger = null)
    : FileRepository(directory, "csv"), IPlanningRepository
{
    public IDictionary<MovieSlug, IList<IScheduled>> FetchAllForToday()
    {
        try
        {
            var mondayPath = GetFilePath(time.StartOfTheWeek);
            logger?.Information("Fetching all scheduled movies for {Target} from file \"{Date}\"", time.StartOfTheWeek, Path.GetFullPath(mondayPath));

            var schedules = csvHelper.ReadSchedules(mondayPath);

            var dateInString = time.CurrentDate.ToString("yyyy-MM-dd");
            var filtered = schedules.Where(dto => dto.Date == dateInString).ToList();

            var mapped = mapper.MapFromDto(filtered);

            logger?.Information("Successfully fetched {Count} schedules", mapped.Sum(x => x.Value.Count));
            return mapped;
        }
        catch (PlanningNotFoundException)
        {
            logger?.Error("Planning not found");
            throw;
        }
    }

    public void UpdateScheduled(IReservation reservation)
    {
        try
        {
            logger?.Information("Updating reservation for {Date}: {Reservation}", time.StartOfTheWeek, reservation);

            var filePath = GetFilePath(time.StartOfTheWeek);
            var targetDateTime = ReservationMapper.MapToDateTimeCsv(reservation);

            csvHelper.UpdateReservation(filePath, targetDateTime, reservation);

            logger?.Information("Schedule updated successfully");
        }
        catch (PlanningNotFoundException)
        {
            logger?.Error("Could not update schedule: planning not found");
            throw;
        }
    }
}
