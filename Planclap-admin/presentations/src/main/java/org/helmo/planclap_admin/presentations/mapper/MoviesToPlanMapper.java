package org.helmo.planclap_admin.presentations.mapper;

import org.helmo.planclap_admin.domains.core.MovieSessions;
import org.helmo.planclap_admin.domains.core.MoviesToPlan;
import org.helmo.planclap_admin.presentations.view_models.MovieSessionsViewModel;
import org.helmo.planclap_admin.presentations.view_models.MoviesToPlanViewModel;

import java.time.format.DateTimeFormatter;
import java.util.ArrayList;
import java.util.List;

public class MoviesToPlanMapper {

    private MoviesToPlanMapper() {}

    public static MoviesToPlanViewModel mapToVM(MoviesToPlan moviesToPlan) {
        List<MovieSessionsViewModel> movieSessionsViewModels = new ArrayList<>();

        for (MovieSessions movie : moviesToPlan.get()) {
            movieSessionsViewModels.add(MovieSessionsMapper.mapToVM(movie));
        }

        return new MoviesToPlanViewModel(
                moviesToPlan.getDateForPlan().format(DateTimeFormatter.ofPattern("dd/MM/yyyy")),
                movieSessionsViewModels
        );
    }

}
