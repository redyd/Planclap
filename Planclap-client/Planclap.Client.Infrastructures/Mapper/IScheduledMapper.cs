using System.Data.Common;
using Planclap.Client.Domains.core;
using Planclap.Client.Domains.IEntities;
using Planclap.Client.Infrastructures.Dto;

namespace Planclap.Client.Infrastructures.Mapper;

public interface IScheduledMapper
{
    IDictionary<MovieSlug, IList<IScheduled>> MapFromDto(IEnumerable<SqlScheduledDto> dtos);

    SqlScheduledDto MapFromReader(DbDataReader reader);

    IDictionary<MovieSlug, IList<IScheduled>> MapFromDto(IEnumerable<CsvScheduledDto> dtos);
}
