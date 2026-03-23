package org.helmo.planclap_admin.domains.core;

import java.time.LocalDateTime;

/**
 * Classe utilitaire pour convertir un film en film à planifier
 */
public class PlannedMovieFactory {

    private PlannedMovieFactory() {
    }

    public static PlannedMovie from(Movie movie, LocalDateTime startTime) {
        LocalDateTime endTime = startTime.plusMinutes(movie.getDuration().get());
        return new PlannedMovie(startTime, endTime, movie.getSlug(), movie.isForChildren());
    }

}
