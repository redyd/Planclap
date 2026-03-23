package org.helmo.planclap_admin.domains.core;

import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Nested;
import org.junit.jupiter.api.Test;

import java.time.LocalDate;
import java.time.LocalDateTime;

import static org.junit.jupiter.api.Assertions.*;

class PlannedMovieTest {

    @Nested
    @DisplayName("Happy path")
    class HappyPath {

        /* SET_START */
        @Test
        void should_keep_correct_duration_when_set_start_time_in_future() {
            LocalDateTime start = LocalDateTime.of(2025, 11, 21, 10, 30);
            LocalDateTime end = LocalDateTime.of(2025, 11, 21, 11, 45); // durée 75 min

            PlannedMovie m1 = new PlannedMovie(start, end, Name.of("Movie"), true);

            m1.setStart(LocalDateTime.of(2025, 11, 21, 11, 45));

            assertEquals(75, m1.getDuration());
        }

        @Test
        void should_keep_correct_duration_when_set_start_time_in_past() {
            LocalDateTime start = LocalDateTime.of(2025, 11, 21, 10, 30);
            LocalDateTime end = LocalDateTime.of(2025, 11, 21, 11, 45); // durée 75 min

            PlannedMovie m1 = new PlannedMovie(start, end, Name.of("Movie"), true);

            m1.setStart(LocalDateTime.of(2025, 11, 21, 9, 45));

            assertEquals(75, m1.getDuration());
        }

        @Test
        void should_keep_same_values_when_set_start_at_same_time() {
            LocalDateTime start = LocalDateTime.of(2025, 11, 21, 10, 30);
            LocalDateTime end = LocalDateTime.of(2025, 11, 21, 11, 45);

            PlannedMovie m1 = new PlannedMovie(start, end, Name.of("Movie"), true);
            PlannedMovie m1Before = m1.copyOf();

            m1.setStart(start);

            assertEquals(m1, m1Before);
        }

        /* GET_DURATION */
        @Test
        void should_get_correct_duration() {
            LocalDateTime start = LocalDateTime.of(2025, 11, 21, 10, 30);
            LocalDateTime end = LocalDateTime.of(2025, 11, 21, 11, 45);

            PlannedMovie m1 = new PlannedMovie(start, end, Name.of("Movie"), true);

            assertEquals(75, m1.getDuration());
        }

        /* OVERRIDE METHODS */
        @Test
        void should_have_a_correct_string_representation() {
            LocalDateTime start = LocalDateTime.of(2025, 11, 21, 10, 30);
            LocalDateTime end = LocalDateTime.of(2025, 11, 21, 11, 45);

            PlannedMovie m1 = new PlannedMovie(start, end, Name.of("Movie"), true);

            assertEquals("PlannedMovie{start=2025-11-21T10:30, end=2025-11-21T11:45, slug=Movie, isChildMovie=true}", m1.toString());
        }

        @Test
        void should_have_same_hashcode_if_equals() {
            LocalDateTime start = LocalDateTime.of(2025, 11, 21, 10, 30);
            LocalDateTime end = LocalDateTime.of(2025, 11, 21, 11, 45);

            var m1 = new PlannedMovie(start, end, Name.of("Movie"), true);
            var m2 = new PlannedMovie(start, end, Name.of("Movie"), true);

            assertEquals(m1.hashCode(), m2.hashCode());
        }

        @Test
        void should_return_its_start_date() {
            LocalDateTime start = LocalDateTime.of(2025, 11, 21, 10, 30);
            LocalDateTime end = LocalDateTime.of(2025, 11, 21, 11, 45);

            var plannedMovie = new PlannedMovie(start, end, Name.of("Movie"), true);

            assertEquals(LocalDate.of(2025, 11, 21), plannedMovie.getDate());
        }
    }

    @Nested
    @DisplayName("Error cases")
    class ErrorCases {

        /* INIT */
        @Test
        void should_throw_exception_when_end_time_is_before_start_time() {
            LocalDateTime start = LocalDateTime.of(2025, 11, 21, 12, 30);
            LocalDateTime end = LocalDateTime.of(2025, 11, 21, 11, 45);

            assertThrows(IllegalArgumentException.class,
                    () -> new PlannedMovie(start, end, Name.of("Movie"), true));
        }

        @Test
        void should_not_be_equals_if_not_same_class() {
            LocalDateTime start = LocalDateTime.of(2025, 11, 21, 10, 30);
            LocalDateTime end = LocalDateTime.of(2025, 11, 21, 11, 45);

            var m1 = new PlannedMovie(start, end, Name.of("Movie"), true);

            assertNotEquals(m1, LocalDateTime.of(2025, 11, 21, 11, 45));
        }

        @Test
        void should_not_be_equals_if_differents_public() {
            LocalDateTime start = LocalDateTime.of(2025, 11, 21, 10, 30);
            LocalDateTime end = LocalDateTime.of(2025, 11, 21, 11, 45);

            var m1 = new PlannedMovie(start, end, Name.of("Movie"), true);
            var m2 = new PlannedMovie(start, end, Name.of("Movie"), false);

            assertNotEquals(m1, m2);
        }

        @Test
        void should_not_be_equals_if_differents_start_or_end_time() {
            LocalDateTime start1 = LocalDateTime.of(2025, 11, 21, 10, 30);
            LocalDateTime start2 = LocalDateTime.of(2025, 11, 21, 10, 35);

            LocalDateTime end1 = LocalDateTime.of(2025, 11, 21, 11, 45);
            LocalDateTime end2 = LocalDateTime.of(2025, 11, 21, 12, 0);

            var m1 = new PlannedMovie(start1, end1, Name.of("Movie"), true);
            var m2 = new PlannedMovie(start2, end1, Name.of("Movie"), true);

            assertNotEquals(m1, m2);

            var m3 = new PlannedMovie(start1, end1, Name.of("Movie"), false);
            var m4 = new PlannedMovie(start1, end2, Name.of("Movie"), false);

            assertNotEquals(m1, m2);
            assertNotEquals(m3, m4);
        }

        @Test
        void should_not_be_equals_if_differents_slug() {
            LocalDateTime start = LocalDateTime.of(2025, 11, 21, 10, 30);
            LocalDateTime end = LocalDateTime.of(2025, 11, 21, 11, 45);

            var m1 = new PlannedMovie(start, end, Name.of("Movie"), true);
            var m2 = new PlannedMovie(start, end, Name.of("Movie 2"), true);

            assertNotEquals(m1, m2);
        }
    }
}
