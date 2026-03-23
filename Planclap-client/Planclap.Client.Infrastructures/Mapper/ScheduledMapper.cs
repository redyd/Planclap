using System.Data.Common;
using Planclap.Client.Domains.core;
using Planclap.Client.Domains.Entities;
using Planclap.Client.Domains.IEntities;
using Planclap.Client.Infrastructures.Dto;
using Planclap.Client.Infrastructures.Extentions;
using Planclap.Client.Infrastructures.Helpers;

namespace Planclap.Client.Infrastructures.Mapper;

public class ScheduledMapper(TicketParser? ticketParser = null) : IScheduledMapper
{
    private readonly TicketParser _ticketParser = ticketParser ?? new TicketParser();

    public IDictionary<MovieSlug, IList<IScheduled>> MapFromDto(IEnumerable<SqlScheduledDto> dtos)
    {
        var map = new Dictionary<MovieSlug, IList<IScheduled>>();

        foreach (var dto in dtos)
        {
            var slug = new MovieSlug(dto.Slug);
            var scheduled = new Scheduled(
                dto.ShowStart.ToDateTime(),
                TimeSpan.FromMinutes(dto.Duration));

            AddTicketsToScheduled(scheduled, dto.Reservations);

            if (!map.TryGetValue(slug, out var list))
            {
                list = new List<IScheduled>();
                map[slug] = list;
            }

            list.Add(scheduled);
        }

        return map;
    }

    public SqlScheduledDto MapFromReader(DbDataReader reader)
        => new(
            (string)reader["slug"],
            Convert.ToInt64(reader["show_start"]),
            Convert.ToInt32(reader["duration"]),
            reader["tickets"] as string ?? string.Empty);

    public IDictionary<MovieSlug, IList<IScheduled>> MapFromDto(IEnumerable<CsvScheduledDto> dtos)
    {
        var map = new Dictionary<MovieSlug, IList<IScheduled>>();

        foreach (var dto in dtos)
        {
            var slug = new MovieSlug(dto.Slug);
            var scheduled = new Scheduled(
                DateParser.ParseDateTime(dto.Date, dto.StartTime),
                DateParser.CalculateDuration(dto.StartTime, dto.EndTime));

            AddTicketsToScheduled(scheduled, dto.Reservations);

            if (!map.TryGetValue(slug, out var list))
            {
                list = new List<IScheduled>();
                map[slug] = list;
            }

            list.Add(scheduled);
        }

        return map;
    }

    private void AddTicketsToScheduled(IScheduled scheduled, string reservations)
    {
        foreach (var ticket in _ticketParser.ParseTickets(reservations))
        {
            scheduled.AddTicket(ticket);
        }
    }
}
