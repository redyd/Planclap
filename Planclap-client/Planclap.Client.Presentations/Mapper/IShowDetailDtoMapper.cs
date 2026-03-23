using Planclap.Client.Domains.core;
using Planclap.Client.Presentations.Dtos;

namespace Planclap.Client.Presentations.Mapper;

public interface IShowDetailDtoMapper
{
    ShowDetailDto MapToDto(MovieSession movie);
}
