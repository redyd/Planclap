package org.helmo.planclap_admin.domains.services;

import org.helmo.planclap_admin.domains.core.PlanningGenerator;
import org.helmo.planclap_admin.domains.core.PlanningTimeManager;
import org.helmo.planclap_admin.domains.core.MoviesToPlan;
import org.helmo.planclap_admin.domains.core.Planning;
import org.helmo.planclap_admin.domains.exceptions.PlanningServiceException;
import org.helmo.planclap_admin.domains.exceptions.RepositoryException;
import org.helmo.planclap_admin.domains.repository.PlanningRepository;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Nested;
import org.junit.jupiter.api.Test;

import java.time.LocalDate;

import static org.junit.jupiter.api.Assertions.assertThrows;
import static org.mockito.Mockito.*;

/**
 * Classe de test généré en partie avec IA
 * Correction manuele
 */
class PlanningServiceTest {

    private PlanningGenerator generator;
    private PlanningRepository planningRepository;
    private MoviesToPlanServices moviesToPlanServices;
    private final LocalDate date = LocalDate.of(2025, 10, 20);

    @BeforeEach
    void setUp() {
        generator = mock(PlanningGenerator.class);
        planningRepository = mock(PlanningRepository.class);
        moviesToPlanServices = mock(MoviesToPlanServices.class);
    }

    @Nested
    @DisplayName("Happy path")
    class HappyPath {

        @Test
        void should_save_planning_when_valid_plan_given() throws RepositoryException, PlanningServiceException {
            when(moviesToPlanServices.getMoviesToPlan()).thenReturn(new MoviesToPlan(date));
            when(generator.generate(any(MoviesToPlan.class))).thenReturn(new Planning(new PlanningTimeManager(date)));
            doNothing().when(planningRepository).encode(any(Planning.class), any(LocalDate.class));

            PlanningService service = new PlanningService(generator, planningRepository, moviesToPlanServices);
            service.generatePlanning();

            verify(planningRepository, times(1)).encode(any(Planning.class), any(LocalDate.class));
            verify(generator, times(1)).generate(any(MoviesToPlan.class));
            verify(moviesToPlanServices, times(1)).getMoviesToPlan();
        }

    }

    @Nested
    @DisplayName("Error cases")
    class ErrorCases {

        @Test
        void should_throw_exception_when_error_occured_in_generator() throws RepositoryException {
            var moviesToPlan = mock(MoviesToPlan.class);
            when(moviesToPlanServices.getMoviesToPlan()).thenReturn(moviesToPlan);
            when(generator.generate(moviesToPlan)).thenThrow(IllegalArgumentException.class);

            var plan = new PlanningService(generator, planningRepository, moviesToPlanServices);
            assertThrows(PlanningServiceException.class, plan::generatePlanning);
        }

    }

}
