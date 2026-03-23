package org.helmo.planclap_admin.domains.services;

import org.helmo.planclap_admin.domains.core.MovieSessions;
import org.helmo.planclap_admin.domains.core.Name;
import org.helmo.planclap_admin.domains.core.MoviesToPlan;
import org.helmo.planclap_admin.domains.exceptions.RepositoryException;
import org.helmo.planclap_admin.domains.iservices.PublishMovieEvents;
import org.helmo.planclap_admin.domains.repository.MoviesToPlanRepository;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Nested;
import org.junit.jupiter.api.Test;
import org.mockito.*;

import java.time.LocalDate;
import java.util.Optional;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.Mockito.*;

/**
 * Classe de test générée en partie par IA
 * Correction manuelle
 */
class MoviesToPlanServicesTest {

    @Mock
    private MoviesToPlanRepository repository;

    @Mock
    private PublishMovieEvents notifier;

    @Mock
    private MoviesToPlan moviesToPlan;

    @Mock
    private MovieSessions movieSessions;

    @Mock
    private Name slug;

    @InjectMocks
    private MoviesToPlanServices services;

    private LocalDate nextMonday;

    @BeforeEach
    void setUp() {
        MockitoAnnotations.openMocks(this);
        services = Mockito.spy(new MoviesToPlanServices(repository, notifier));
        nextMonday = services.getNextMonday();
        doReturn(nextMonday).when(services).getNextMonday();
    }

    @Nested
    @DisplayName("Happy path")
    class HappyPath {

        @Test
        void should_add_movie_to_plan_and_notify_when_given_valid_movie() throws RepositoryException {
            // Act
            services.addMovieToPlan(movieSessions);

            // Assert
            verify(repository).addMovieToPlan(movieSessions, nextMonday);
            verify(notifier).notifySubscribers();
        }

        @Test
        void should_return_true_when_slug_exists_in_repository() throws RepositoryException {
            // Arrange
            when(repository.isAlreadyPlanned(slug, nextMonday)).thenReturn(true);

            // Act
            boolean result = services.slugExists(slug);

            // Assert
            assertTrue(result);
            verify(repository).isAlreadyPlanned(slug, nextMonday);
        }

        @Test
        void should_return_false_when_slug_does_not_exist_in_repository() throws RepositoryException {
            // Arrange
            when(repository.isAlreadyPlanned(slug, nextMonday)).thenReturn(false);

            // Act
            boolean result = services.slugExists(slug);

            // Assert
            assertFalse(result);
            verify(repository).isAlreadyPlanned(slug, nextMonday);
        }

        @Test
        void should_return_can_add_movie_when_repository_allows_it() throws RepositoryException {
            // Arrange
            when(repository.getMoviesToPlan(nextMonday)).thenReturn(moviesToPlan);
            when(moviesToPlan.canAdd(movieSessions)).thenReturn(true);

            // Act
            boolean result = services.canAddMovieToPlan(movieSessions);

            // Assert
            assertTrue(result);
            verify(repository).getMoviesToPlan(nextMonday);
            verify(moviesToPlan).canAdd(movieSessions);
        }

        @Test
        void should_return_movies_to_plan_when_given_valid_request() throws RepositoryException {
            // Arrange
            when(repository.getMoviesToPlan(nextMonday)).thenReturn(moviesToPlan);

            // Act
            MoviesToPlan result = services.getMoviesToPlan();

            // Assert
            assertEquals(moviesToPlan, result);
            verify(repository).getMoviesToPlan(nextMonday);
        }

        @Test
        void should_return_optional_movie_when_search_by_slug() throws RepositoryException {
            // Arrange
            when(repository.getMoviesToPlan(nextMonday)).thenReturn(moviesToPlan);
            Optional<MovieSessions> expected = Optional.of(movieSessions);
            when(moviesToPlan.search(slug)).thenReturn(expected);

            // Act
            Optional<MovieSessions> result = services.searchMovieToPlan(slug);

            // Assert
            assertEquals(expected, result);
            verify(moviesToPlan).search(slug);
        }

    }

    @Nested
    @DisplayName("Error cases")
    class ErrorCases {

        @Test
        void should_throw_repository_exception_when_add_movie_fails() throws RepositoryException {
            // Arrange
            doThrow(new RepositoryException("fail")).when(repository).addMovieToPlan(movieSessions, nextMonday);

            // Act + Assert
            assertThrows(RepositoryException.class, () -> services.addMovieToPlan(movieSessions));
            verify(notifier, never()).notifySubscribers();
        }

        @Test
        void should_throw_repository_exception_when_get_movies_fails() throws RepositoryException {
            // Arrange
            when(repository.getMoviesToPlan(nextMonday)).thenThrow(new RepositoryException("error"));

            // Act + Assert
            assertThrows(RepositoryException.class, () -> services.getMoviesToPlan());
        }

    }
}
