package org.helmo.planclap_admin.infrastructures.mapper;

import org.helmo.planclap_admin.domains.core.MovieSessions;
import org.helmo.planclap_admin.domains.core.SessionAmount;
import org.helmo.planclap_admin.infrastructures.dto.MovieSessionsDto;

import java.util.Set;

public class MovieSessionsMapper {

    private MovieSessionsMapper() {
    }

    public static MovieSessions mapDTO(MovieSessionsDto dto) {
        return new MovieSessions(
                MovieMapper.from(
                        dto.title(),
                        dto.duration(),
                        dto.poster(),
                        dto.description(),
                        Set.of(dto.cinechecks())),
                SessionAmount.of(dto.seances()));
    }

    public static MovieSessionsDto mapToDTO(MovieSessions movieSessions) {
        var movie = movieSessions.movie();
        return new MovieSessionsDto(
                movie.getSlug().toString(),
                movie.getTitle().toString(),
                movie.getDuration().get(),
                movie.getPoster().url().toString(),
                movie.getDescription().toString(),
                CineCheckGroupMapper.mapToDTO(movie.getCineChecksGroup()),
                movieSessions.sessionsAmount().toInt()
        );
    }
}
