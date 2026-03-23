package org.helmo.planclap_admin.presentations.presenters;

import org.helmo.planclap_admin.domains.exceptions.RepositoryException;
import org.helmo.planclap_admin.domains.core.MoviesToPlan;
import org.helmo.planclap_admin.domains.iservices.GetMoviesToPlan;
import org.helmo.planclap_admin.domains.iservices.PublishMovieEvents;
import org.helmo.planclap_admin.presentations.iview.DisplayMoviesToPlanPresenterView;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.mockito.Mock;
import org.mockito.MockitoAnnotations;

import java.time.LocalDate;

import static org.mockito.Mockito.*;

public class DisplayMovieToPlanPresenterTest {

    @Mock
    GetMoviesToPlan services;

    @Mock
    PublishMovieEvents events;

    @Mock
    DisplayMoviesToPlanPresenterView view;

    @BeforeEach
    public void setup() {
        MockitoAnnotations.openMocks(this);
    }

    @Test
    void should_display_to_view() throws RepositoryException {
        var presenter = DisplayMoviesToPlanPresenter.of(services, view, events);
        when(services.getMoviesToPlan()).thenReturn(new MoviesToPlan(LocalDate.now()));
        presenter.execute();

        verify(view, times(1)).displayMoviesToPlan(any());
        verify(view, times(1)).displayTotalDuration(any());
    }

    @Test
    void should_display_error_message_when_repository_exception() throws RepositoryException {
        when(services.getMoviesToPlan()).thenThrow(RepositoryException.class);
        var presenter = DisplayMoviesToPlanPresenter.of(services, view, events);

        presenter.execute();

        verify(view, times(1)).displayErrorMessage(anyString());
    }

    @Test
    void should_execute_when_notification() throws RepositoryException {
        var presenter = DisplayMoviesToPlanPresenter.of(services, view, events);
        when(services.getMoviesToPlan()).thenReturn(new MoviesToPlan(LocalDate.now()));

        presenter.onMovieAdded();

        verify(view, times(1)).displayMoviesToPlan(any());
        verify(view, times(1)).displayTotalDuration(any());
    }

}
