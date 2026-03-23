package org.helmo.planclap_admin.infrastructures.mapper;

import org.helmo.planclap_admin.domains.core.*;
import org.helmo.planclap_admin.infrastructures.dto.MovieDto;

import java.util.Arrays;
import java.util.Collection;

public class MovieMapper {

    private MovieMapper() {
    }

    public static Movie from(String title, int duration, String url, String description, Collection<String> checks) {
        var movieTitle = Name.of(title);
        var movieDescription = MovieDescription.of(description);
        var movieDuration = MovieDuration.of(duration);
        var poster = PosterURI.of(url);
        var group = CineCheckGroupMapper.map(checks);

        return new Movie(movieTitle, movieDescription, movieDuration, poster, group);
    }

    public static Movie mapDto(MovieDto dto) {
        return from(
                dto.title(),
                dto.duration(),
                dto.poster(),
                dto.description(),
                Arrays.asList(dto.cineChecks())
        );
    }
}
