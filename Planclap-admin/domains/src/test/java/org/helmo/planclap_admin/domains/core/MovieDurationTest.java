package org.helmo.planclap_admin.domains.core;

import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Nested;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.*;

/**
 * Classe de test générée en partie par IA
 * Correction manuelle
 */
class MovieDurationTest {

    @Nested
    @DisplayName("Happy path")
    class HappyPath {

        @Test
        void shouldCreateMovieDuration_whenValueIsBetween1And240() {
            MovieDuration d1 = MovieDuration.of(1);
            MovieDuration d2 = MovieDuration.of(120);
            MovieDuration d3 = MovieDuration.of(240);

            assertEquals(1, d1.get());
            assertEquals(120, d2.get());
            assertEquals(240, d3.get());
        }

        @Test
        void shouldReturnFormattedHHMM_whenCallingToHoursFormat_givenDurationInMinutes() {
            MovieDuration d = MovieDuration.of(135);
            assertEquals("02 h 15", d.toHoursFormat());
        }

        @Test
        void shouldReturnHoursAsDouble_whenCallingToHours_givenDurationInMinutes() {
            MovieDuration d = MovieDuration.of(90);
            assertEquals(1.5, d.toHours(), 0.0001);
        }

        @Test
        void shouldRespectEqualsAndHashCodeContract_whenComparingMovieDurations() {
            MovieDuration d1 = MovieDuration.of(90);
            MovieDuration d2 = MovieDuration.of(90);
            MovieDuration d3 = MovieDuration.of(120);

            assertEquals(d1, d2);
            assertEquals(d1.hashCode(), d2.hashCode());
            assertNotEquals(d1, d3);
            assertNotEquals(null, d1);
        }

        @Test
        void shouldReturnGlobalDurationOfProduct_whenCallingMultiplyBy_givenPositiveMultiplier() {
            MovieDuration d = MovieDuration.of(100);
            GlobalDuration result = d.multiplyBy(3);

            assertEquals(300, result.get());
        }

        @Test
        void shouldReturnZero_whenCallingMultiplyBy_givenZeroMultiplier() {
            MovieDuration d = MovieDuration.of(100);
            GlobalDuration result = d.multiplyBy(0);

            assertEquals(0, result.get());
        }

        @Test
        void shouldReturnConsistentStringFormats_whenCallingToHoursAndToHoursFormat() {
            MovieDuration d = MovieDuration.of(75);
            assertEquals("01 h 15", d.toHoursFormat());
            assertEquals(1.25, d.toHours(), 0.0001);
        }
    }

    @Nested
    @DisplayName("Error cases")
    class ErrorCases {

        @Test
        void shouldThrowException_whenCreatingMovieDuration_givenValueBelow1() {
            assertThrows(IllegalArgumentException.class, () -> MovieDuration.of(0));
            assertThrows(IllegalArgumentException.class, () -> MovieDuration.of(-5));
        }

        @Test
        void shouldThrowException_whenCreatingMovieDuration_givenValueAbove240() {
            assertThrows(IllegalArgumentException.class, () -> MovieDuration.of(241));
            assertThrows(IllegalArgumentException.class, () -> MovieDuration.of(999));
        }
    }
}
