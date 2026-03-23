package org.helmo.planclap_admin.presentations.presenters;

import org.helmo.planclap_admin.domains.core.MovieSessions;
import org.helmo.planclap_admin.domains.core.Name;
import org.helmo.planclap_admin.domains.exceptions.RepositoryException;
import org.helmo.planclap_admin.domains.iservices.SearchMovieToPlan;
import org.helmo.planclap_admin.domains.iservices.Executable;
import org.helmo.planclap_admin.presentations.iview.SearchMoviePresenterView;
import org.helmo.planclap_admin.presentations.mapper.MovieSessionsMapper;

import java.util.Optional;

public class SearchMoviePresenter implements Executable {

    private final SearchMovieToPlan services;
    private final SearchMoviePresenterView view;

    private SearchMoviePresenter(SearchMovieToPlan services, SearchMoviePresenterView view) {
        this.services = services;
        this.view = view;
    }

    public static SearchMoviePresenter of(SearchMovieToPlan services, SearchMoviePresenterView view) {
        return new SearchMoviePresenter(services, view);
    }

    @Override
    public void execute() {
        Optional<MovieSessions> movie = getMovie();
        if (movie.isPresent()) {
            view.displayMovie(MovieSessionsMapper.mapToVM(movie.get()));
        } else {
            view.notFound();
        }
    }

    private Optional<MovieSessions> getMovie() {
        try {
            return services.searchMovieToPlan(Name.of(view.getSlug()).toMinimal());
        } catch (IllegalArgumentException e) {
            view.displayError("Nom invalide");
            return getMovie();
        } catch (RepositoryException e) {
            view.displayError("Erreur de lors de l'ouverture des resources");
            return getMovie();
        }
    }
}
