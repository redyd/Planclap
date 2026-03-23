package org.helmo.planclap_admin.presentations.presenters;

import org.helmo.planclap_admin.domains.exceptions.PlanningServiceException;
import org.helmo.planclap_admin.domains.exceptions.RepositoryException;
import org.helmo.planclap_admin.domains.iservices.GeneratePlanning;
import org.helmo.planclap_admin.presentations.iview.GeneratePlanningPresenterView;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.mockito.Mock;
import org.mockito.MockitoAnnotations;

import static org.mockito.Mockito.*;

public class GeneratePlanningPresenterTest {

    @Mock
    GeneratePlanning services;

    @Mock
    GeneratePlanningPresenterView view;

    @BeforeEach
    public void setup() {
        MockitoAnnotations.openMocks(this);
    }

    @Test
    void should_display_to_view() {
        var presenter = GeneratePlanningPresenter.of(services, view);

        presenter.execute();

        verify(view, times(1)).displaySuccess(any());
    }

    @Test
    void should_display_error_message_when_repository_exception() throws RepositoryException, PlanningServiceException {
        var presenter = GeneratePlanningPresenter.of(services, view);
        doThrow(RepositoryException.class).when(services).generatePlanning();

        presenter.execute();

        verify(view, times(1)).displayError(any());
    }

    @Test
    void should_display_error_message_when_service_exception() throws RepositoryException, PlanningServiceException {
        var presenter = GeneratePlanningPresenter.of(services, view);
        doThrow(PlanningServiceException.class).when(services).generatePlanning();

        presenter.execute();

        verify(view, times(1)).displayError(any());
    }
}
