package org.helmo.planclap_admin.presentations.mapper;

import org.helmo.planclap_admin.domains.core.MovieSessions;
import org.helmo.planclap_admin.presentations.view_models.MovieSessionsViewModel;

public class MovieSessionsMapper {

    private MovieSessionsMapper() {}

    public static MovieSessionsViewModel mapToVM(MovieSessions movieSessions) {
        var movie = movieSessions.movie();
        return new MovieSessionsViewModel(
                movie.getTitle().toString(),
                CineCheckGroupMapper.mapGroupToVM(movie.getCineChecksGroup()),
                movie.getPoster().url().toString(),
                movie.getDuration().toHoursFormat(),
                movie.getDescription().toString(),
                movieSessions.sessionsAmount().toInt()
        );
    }

}
