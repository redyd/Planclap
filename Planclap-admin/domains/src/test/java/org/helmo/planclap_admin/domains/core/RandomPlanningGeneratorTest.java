package org.helmo.planclap_admin.domains.core;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Nested;
import org.junit.jupiter.api.Test;

import java.time.DayOfWeek;
import java.time.LocalDate;
import java.time.LocalDateTime;
import java.time.LocalTime;
import java.util.HashSet;

import static org.helmo.planclap_admin.domains.providers.TestProvider.*;
import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.Mockito.*;

class RandomPlanningGeneratorTest {

    private RandomPlanningGenerator generator;

    @BeforeEach
    void setUp() {
        generator = new RandomPlanningGenerator();
    }

    @Nested
    @DisplayName("Happy path")
    class HappyPath {

        @Test
        void should_generate_planning_when_correct_hours() {
            var plan = plan(74, LocalDate.of(2025, 10, 20));

            var planning = generator.generate(plan);

            assertEquals(74, plan.getTotalDuration().getHours());
            assertTrue(planning.size() > 0);
        }

        @Test
        void should_generate_randomized_planning() {
            var plan = plan(74, LocalDate.of(2025, 10, 20));

            assertNotEquals(generator.generate(plan), generator.generate(plan));
        }

        @Test
        void last_day_should_be_in_the_same_week() {
            var plan = plan(74, LocalDate.of(2025, 10, 20));
            var planning = generator.generate(plan);

            assertEquals(DayOfWeek.SUNDAY, planning.getAt(WeekDay.SUNDAY).getLast().getStartTime().getDayOfWeek());
        }

        @Test
        void should_not_generated_movie_that_start_at_23() {
            for (int i = 0; i < 100; i++) {
                var planning = generator.generate(plan(74, LocalDate.of(2025, 10, 20)));

                for (int j = 0; j < 7; j++) {
                    var list = planning.getAt(WeekDay.values()[j]);
                    var times = new HashSet<LocalDateTime>();
                    for (var movie : list) {
                        if (movie.getStartTime().toLocalTime().isAfter(LocalTime.of(22, 59))) {
                            fail("this movie start at/after 23pm: " + movie.getStartTime());
                        }
                        times.add(movie.getStartTime());
                    }
                    if (times.size() != list.size()) {
                        fail("mutliple film for one session");
                    }
                }
            }
        }
    }

    @Nested
    @DisplayName("Error cases")
    class ErrorCases {

        @Test
        void should_throw_exception_when_plan_below_limit() {
            MoviesToPlan plan = mock(MoviesToPlan.class);
            when(plan.belowPlanLimit()).thenReturn(true);

            assertThrows(IllegalArgumentException.class, () -> generator.generate(plan));
        }

    }
}
