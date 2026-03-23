package org.helmo.planclap_admin.presentations.presenters;

import org.helmo.planclap_admin.domains.core.*;
import org.helmo.planclap_admin.domains.exceptions.RepositoryException;
import org.helmo.planclap_admin.domains.iservices.SearchMovieToPlan;
import org.helmo.planclap_admin.presentations.iview.SearchMoviePresenterView;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.mockito.Mock;
import org.mockito.MockitoAnnotations;

import java.util.Optional;
import java.util.Set;

import static org.mockito.Mockito.*;

public class SearchMoviePresenterTest {

    private final MovieSessions example = new MovieSessions(
            new Movie(Name.of("example"),
                    MovieDescription.of("description"),
                    MovieDuration.of(100),
                    PosterURI.of("www.poster.com"),
                    CineCheckGroup.of(CineCheckAge.AL, Set.of())),
            SessionAmount.of(3)
    );

    @Mock
    SearchMovieToPlan services;

    @Mock
    SearchMoviePresenterView view;

    @BeforeEach
    public void setup() {
        MockitoAnnotations.openMocks(this);
    }

    @Test
    void should_display_movie_if_found() throws RepositoryException {
        when(services.searchMovieToPlan(any())).thenReturn(Optional.of(example));

        when(view.getSlug()).thenReturn("example");

        var presenter = SearchMoviePresenter.of(services, view);

        presenter.execute();

        verify(view, times(1)).displayMovie(any());
    }

    @Test
    void should_display_error_if_not_found() throws RepositoryException {
        when(services.searchMovieToPlan(any())).thenReturn(Optional.empty());

        when(view.getSlug()).thenReturn("example");

        var presenter = SearchMoviePresenter.of(services, view);

        presenter.execute();

        verify(view, times(1)).notFound();
    }

    @Test
    void should_display_error_when_bad_input() throws RepositoryException {
        when(services.searchMovieToPlan(any()))
                .thenThrow(IllegalArgumentException.class)
                .thenReturn(Optional.of(example));

        when(view.getSlug()).thenReturn("example");

        var presenter = SearchMoviePresenter.of(services, view);

        presenter.execute();

        verify(view, times(1)).displayMovie(any());
        verify(view, times(1)).displayMovie(any());
    }

    @Test
    void should_display_error_when_repo_exception() throws RepositoryException {
        when(services.searchMovieToPlan(any()))
                .thenThrow(RepositoryException.class)
                .thenReturn(Optional.of(example));

        when(view.getSlug()).thenReturn("example");

        var presenter = SearchMoviePresenter.of(services, view);

        presenter.execute();

        verify(view, times(1)).displayMovie(any());
        verify(view, times(1)).displayMovie(any());
    }

}
