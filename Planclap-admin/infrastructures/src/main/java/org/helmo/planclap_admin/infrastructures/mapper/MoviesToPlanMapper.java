package org.helmo.planclap_admin.infrastructures.mapper;

import org.helmo.planclap_admin.domains.core.MoviesToPlan;
import org.helmo.planclap_admin.domains.exceptions.TooMuchMoviesException;
import org.helmo.planclap_admin.infrastructures.dto.MoviesToPlanDto;

import java.time.LocalDate;
import java.util.stream.Collectors;

public class MoviesToPlanMapper {

    private MoviesToPlanMapper() {
    }

    public static MoviesToPlan mapDTO(LocalDate date, MoviesToPlanDto movies) {
        try {
            MoviesToPlan plan = new MoviesToPlan(date);
            movies.movies().forEach(dto -> plan.add(MovieSessionsMapper.mapDTO(dto)));

            return plan;
        } catch (TooMuchMoviesException | IllegalArgumentException e) {
            return new MoviesToPlan(date);
        }
    }

    public static MoviesToPlanDto mapToDTO(MoviesToPlan moviesToPlan) {
        var movieDTOs = moviesToPlan.get().stream()
                .map(MovieSessionsMapper::mapToDTO)
                .collect(Collectors.toSet());
        return new MoviesToPlanDto(movieDTOs);
    }
}
