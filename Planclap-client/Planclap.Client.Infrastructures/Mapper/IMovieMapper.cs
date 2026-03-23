using System.Data.Common;
using Planclap.Client.Domains.core;
using Planclap.Client.Infrastructures.Dto;

namespace Planclap.Client.Infrastructures.Mapper;

public interface IMovieMapper
{
    MovieDto MapFromReader(DbDataReader reader);

    IList<Movie> MapFromDto(IList<MovieDto> movieDtos);
}
