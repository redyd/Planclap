package org.helmo.planclap_admin.presentations.presenters;

import org.helmo.planclap_admin.domains.core.*;
import org.helmo.planclap_admin.domains.exceptions.*;
import org.helmo.planclap_admin.domains.iservices.AddMovieToPlan;
import org.helmo.planclap_admin.presentations.iview.AddMovieToPlanPresenterView;
import org.helmo.planclap_admin.presentations.view_models.CineCheckAgeViewModel;
import org.helmo.planclap_admin.presentations.view_models.CineCheckViewModel;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Nested;
import org.junit.jupiter.api.Test;

import java.util.List;

import static org.mockito.Mockito.*;

/**
 * Classe de test générée en partie par IA
 * Correction manuelle
 */
class AddMovieToPlanPresenterTest {

    private AddMovieToPlan services;
    private AddMovieToPlanPresenterView view;
    private AddMovieToPlanPresenter presenter;

    @BeforeEach
    void setUp() {
        services = mock(AddMovieToPlan.class);
        view = mock(AddMovieToPlanPresenterView.class);
        presenter = AddMovieToPlanPresenter.of(services, view);
    }

    @Nested
    class HappyPath {

        @Test
        void should_add_movie_to_plan_when_data_is_valid_given_service_allows_addition() throws Exception {
            // Arrange
            when(view.getTitle()).thenReturn("Inception");
            when(view.getDescription()).thenReturn("A mind-bending thriller");
            when(view.getPosterURI()).thenReturn("https://poster.com/inception.jpg");
            when(view.getDuration()).thenReturn(120);
            when(view.getSessionQuantity()).thenReturn(3);
            when(view.getCineCheckAge(any(CineCheckAgeViewModel.class))).thenReturn("16");
            when(view.getCineChecks(any(CineCheckViewModel.class))).thenReturn(List.of("Violence", "Peur"));
            when(services.slugExists(any(Name.class))).thenReturn(false);
            when(services.canAddMovieToPlan(any(MovieSessions.class))).thenReturn(true);

            // Act
            presenter.execute();

            // Assert
            verify(services, times(1)).addMovieToPlan(any(MovieSessions.class));
            verify(view, times(1)).displaySuccessMessage("Film correctement ajoute !");
            verify(view, never()).displayErrorMessage(startsWith("Erreur"));
        }

        @Test
        void should_display_error_message_when_service_refuses_to_add_movie_given_valid_inputs() throws Exception {
            when(view.getTitle()).thenReturn("Dune");
            when(view.getDescription()).thenReturn("Epic sci-fi");
            when(view.getPosterURI()).thenReturn("poster.jpg");
            when(view.getDuration()).thenReturn(180);
            when(view.getSessionQuantity()).thenReturn(2);
            when(view.getCineCheckAge(any())).thenReturn("12");
            when(view.getCineChecks(any())).thenReturn(List.of("Paroles grossieres"));
            when(services.slugExists(any())).thenReturn(false);
            when(services.canAddMovieToPlan(any())).thenReturn(false);

            presenter.execute();

            verify(view).displayErrorMessage("Impossible d'ajouter ce film a planifier");
            verify(services, never()).addMovieToPlan(any());
        }
    }

    @Nested
    class ErrorCases {

        @Test
        void should_display_error_message_when_service_throws_movies_to_plan_repo_exception_given_valid_movie() throws Exception {
            when(view.getTitle()).thenReturn("Matrix");
            when(view.getDescription()).thenReturn("Neo awakens");
            when(view.getPosterURI()).thenReturn("poster.png");
            when(view.getDuration()).thenReturn(130);
            when(view.getSessionQuantity()).thenReturn(3);
            when(view.getCineCheckAge(any())).thenReturn("16");
            when(view.getCineChecks(any())).thenReturn(List.of("Violence"));
            when(services.slugExists(any())).thenReturn(false);
            when(services.canAddMovieToPlan(any())).thenReturn(true);
            doThrow(new RepositoryException("DB error")).when(services).addMovieToPlan(any());

            presenter.execute();

            verify(view, times(1)).displayErrorMessage(any());
        }

        @Test
        void should_retry_asking_for_title_when_name_is_invalid_given_name_exception_thrown() throws Exception {
            when(view.getTitle())
                    .thenReturn(null)
                    .thenReturn("")
                    .thenReturn("Avatar");
            when(view.getDescription()).thenReturn("Blue people");
            when(view.getPosterURI()).thenReturn("poster.jpg");
            when(view.getDuration()).thenReturn(150);
            when(view.getSessionQuantity()).thenReturn(2);
            when(view.getCineCheckAge(any())).thenReturn("AL");
            when(view.getCineChecks(any())).thenReturn(List.of("Sexe"));
            when(services.slugExists(any())).thenReturn(false);
            when(services.canAddMovieToPlan(any())).thenReturn(true);

            presenter.execute();

            verify(view, times(2)).displayErrorMessage(any());
            verify(view, times(3)).getTitle();
        }

        @Test
        void should_retry_asking_for_description_when_movie_description_exception_thrown() throws Exception {
            when(view.getTitle()).thenReturn("Avatar");
            when(view.getDescription())
                    .thenReturn(null)
                    .thenReturn("")
                    .thenReturn("Epic movie");
            when(view.getPosterURI()).thenReturn("poster.jpg");
            when(view.getDuration()).thenReturn(150);
            when(view.getSessionQuantity()).thenReturn(2);
            when(view.getCineCheckAge(any())).thenReturn("12");
            when(view.getCineChecks(any())).thenReturn(List.of("Peur"));
            when(services.slugExists(any())).thenReturn(false);
            when(services.canAddMovieToPlan(any())).thenReturn(true);

            presenter.execute();

            verify(view, times(2)).displayErrorMessage(any());
            verify(view, times(3)).getDescription();
        }

        @Test
        void should_retry_asking_for_age_when_cine_checks_group_exception_thrown() throws Exception {
            when(view.getTitle()).thenReturn("Avatar");
            when(view.getDescription()).thenReturn("A movie");
            when(view.getPosterURI()).thenReturn("poster.jpg");
            when(view.getDuration()).thenReturn(150);
            when(view.getSessionQuantity()).thenReturn(2);
            when(view.getCineCheckAge(any()))
                    .thenReturn("")
                    .thenReturn("12+")
                    .thenReturn("6");
            when(view.getCineChecks(any())).thenReturn(List.of("Peur"));
            when(services.slugExists(any())).thenReturn(false);
            when(services.canAddMovieToPlan(any())).thenReturn(true);

            presenter.execute();

            verify(view, times(2)).displayErrorMessage(any());
            verify(view, times(3)).getCineCheckAge(any());
        }

        @Test
        void should_retry_asking_for_duration_when_movie_duration_exception_thrown() throws Exception {
            when(view.getTitle()).thenReturn("Avatar");
            when(view.getDescription()).thenReturn("A movie");
            when(view.getPosterURI()).thenReturn("poster.jpg");
            when(view.getDuration())
                    .thenReturn(999)
                    .thenReturn(150);
            when(view.getSessionQuantity()).thenReturn(2);
            when(view.getCineCheckAge(any())).thenReturn("12");
            when(view.getCineChecks(any())).thenReturn(List.of("Peur"));
            when(services.slugExists(any())).thenReturn(false);
            when(services.canAddMovieToPlan(any())).thenReturn(true);

            presenter.execute();

            verify(view).displayErrorMessage("La duree du film doit etre comprise entre 1 et 240 minutes.");
            verify(view, atLeast(2)).getDuration();
        }

        @Test
        void should_retry_asking_for_session_quantity_when_session_amount_exception_thrown() throws Exception {
            when(view.getTitle()).thenReturn("Avatar");
            when(view.getDescription()).thenReturn("A movie");
            when(view.getPosterURI()).thenReturn("poster.jpg");
            when(view.getDuration()).thenReturn(150);
            when(view.getSessionQuantity())
                    .thenReturn(-3)
                    .thenReturn(3);
            when(view.getCineCheckAge(any())).thenReturn("18");
            when(view.getCineChecks(any())).thenReturn(List.of("Drogues, alcool et fumer"));
            when(services.slugExists(any())).thenReturn(false);
            when(services.canAddMovieToPlan(any())).thenReturn(true);

            presenter.execute();

            verify(view, times(1)).displayErrorMessage(any());
            verify(view, atLeast(2)).getSessionQuantity();
        }

        @Test
        void should_retry_asking_for_title_when_slug_already_exists_given_existing_slug() throws Exception {
            when(view.getTitle())
                    .thenReturn("Avatar")
                    .thenReturn("Avatar 2");
            when(view.getDescription()).thenReturn("A movie");
            when(view.getPosterURI()).thenReturn("poster.jpg");
            when(view.getDuration()).thenReturn(150);
            when(view.getSessionQuantity()).thenReturn(2);
            when(view.getCineCheckAge(any())).thenReturn("12");
            when(view.getCineChecks(any())).thenReturn(List.of("Peur"));
            when(services.slugExists(any()))
                    .thenReturn(true)
                    .thenReturn(false);
            when(services.canAddMovieToPlan(any())).thenReturn(true);

            presenter.execute();

            verify(view).displayErrorMessage("Nom de film deja existant");
            verify(view, atLeast(2)).getTitle();
        }

        @Test
        void should_display_error_when_too_much_movie() throws RepositoryException {
            when(view.getTitle()).thenReturn("Avatar");
            when(view.getDescription()).thenReturn("A movie");
            when(view.getPosterURI()).thenReturn("poster.jpg");
            when(view.getDuration()).thenReturn(150);
            when(view.getSessionQuantity()).thenReturn(2);
            when(view.getCineCheckAge(any())).thenReturn("12");
            when(view.getCineChecks(any())).thenReturn(List.of("Peur"));

            when(services.canAddMovieToPlan(any())).thenReturn(true);
            doThrow(TooMuchMoviesException.class).when(services).addMovieToPlan(any());

            presenter.execute();

            verify(view).displayErrorMessage(any());
        }

        @Test
        void should_display_error_message_when_invalid_poster() {
            when(view.getTitle()).thenReturn("Avatar");
            when(view.getDescription()).thenReturn("A movie");
            when(view.getPosterURI()).thenReturn("", "poster.jpg");
            when(view.getDuration()).thenReturn(150);
            when(view.getSessionQuantity()).thenReturn(2);
            when(view.getCineCheckAge(any())).thenReturn("12");
            when(view.getCineChecks(any())).thenReturn(List.of("Peur"));

            presenter.execute();

            verify(view).displayErrorMessage("L'URL ne peut pas etre nulle");
        }

        @Test
        void should_display_error_when_slug_check_throws_exception() throws RepositoryException {
            when(view.getTitle()).thenReturn("Avatar");

            when(services.slugExists(any())).thenThrow(RepositoryException.class).thenReturn(false);

            when(view.getDescription()).thenReturn("A movie");
            when(view.getPosterURI()).thenReturn("poster.jpg");
            when(view.getDuration()).thenReturn(150);
            when(view.getSessionQuantity()).thenReturn(2);
            when(view.getCineCheckAge(any())).thenReturn("12");
            when(view.getCineChecks(any())).thenReturn(List.of("Peur"));

            presenter.execute();

            verify(view).displayErrorMessage("Une erreur s'est produite lors de la verification");
        }
    }
}
