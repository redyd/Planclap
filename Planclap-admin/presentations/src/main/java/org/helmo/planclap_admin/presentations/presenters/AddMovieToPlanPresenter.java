package org.helmo.planclap_admin.presentations.presenters;

import org.helmo.planclap_admin.domains.core.Movie;
import org.helmo.planclap_admin.domains.core.MovieSessions;
import org.helmo.planclap_admin.domains.core.SessionAmount;
import org.helmo.planclap_admin.domains.iservices.AddMovieToPlan;
import org.helmo.planclap_admin.domains.iservices.Executable;
import org.helmo.planclap_admin.presentations.helper.AddMovieToPlanHandler;
import org.helmo.planclap_admin.presentations.helper.MovieBuilder;
import org.helmo.planclap_admin.presentations.iview.AddMovieToPlanPresenterView;

public class AddMovieToPlanPresenter implements Executable {

    private final MovieBuilder builder;
    private final AddMovieToPlanHandler executor;

    private AddMovieToPlanPresenter(AddMovieToPlan services, AddMovieToPlanPresenterView view) {
        this.builder = new MovieBuilder(view);
        this.executor = new AddMovieToPlanHandler(services, view);
    }

    public static AddMovieToPlanPresenter of(AddMovieToPlan services, AddMovieToPlanPresenterView view) {
        return new AddMovieToPlanPresenter(services, view);
    }

    @Override
    public void execute() {
        var title = builder.buildTitle(slug -> !executor.slugExists(slug));
        var maybeMovie = executor.getOnlyMovie(title.toMinimal());
        Movie movie = maybeMovie.orElseGet(() -> builder.build(title));

        SessionAmount sessionAmount = builder.getValidatedSessionAmount();
        MovieSessions sessions = new MovieSessions(movie, sessionAmount);

        executor.execute(sessions);
    }
}