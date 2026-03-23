package org.helmo.planclap_admin.domains.core;

import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Nested;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.*;

/**
 * Classe de test générée en partie par IA
 * Correction manuelle
 */
class DurationTest {

    @Nested
    @DisplayName("Happy path")
    class HappyPath {

        @Test
        void should_return_value_in_minutes_when_given_valid_duration() {
            Duration d = GlobalDuration.of(120);
            assertEquals(120, d.get());
        }

        @Test
        void should_return_hours_as_double_when_given_duration_in_minutes() {
            Duration d = GlobalDuration.of(90);
            assertEquals(1.5, d.getHours(), 0.0001);
        }

        @Test
        void should_return_formatted_string_when_given_duration() {
            Duration d = GlobalDuration.of(135);
            assertEquals("02 h 15", d.toHoursFormat());
        }

        @Test
        void should_return_formatted_string_with_zero_padding_when_given_less_than_ten_minutes() {
            Duration d = GlobalDuration.of(7);
            assertEquals("00 h 07", d.toHoursFormat());
        }

        @Test
        void should_return_double_in_hours_when_given_minutes() {
            Duration d = GlobalDuration.of(180);
            assertEquals(3.0, d.toHours(), 0.0001);
        }

        @Test
        void should_return_true_when_given_equal_durations() {
            Duration d1 = GlobalDuration.of(100);
            Duration d2 = GlobalDuration.of(100);
            assertEquals(d1, d2);
        }

        @Test
        void should_return_same_hashcode_when_given_equal_durations() {
            Duration d1 = GlobalDuration.of(200);
            Duration d2 = GlobalDuration.of(200);
            assertEquals(d1.hashCode(), d2.hashCode());
        }

        @Test
        void should_return_false_when_given_different_durations() {
            Duration d1 = GlobalDuration.of(100);
            Duration d2 = GlobalDuration.of(50);
            assertNotEquals(d1, d2);
        }

        @Test
        void should_return_false_when_given_null() {
            Duration d = GlobalDuration.of(100);
            assertNotEquals(d, null);
        }

        @Test
        void should_return_false_when_given_different_type() {
            Duration d = GlobalDuration.of(100);
            assertNotEquals(d, "not a duration");
        }

        @Test
        void should_return_true_when_given_same_reference() {
            Duration d = GlobalDuration.of(60);
            assertEquals(d, d);
        }
    }

    @Nested
    @DisplayName("error cases")
    class ErrorCases {

        @Test
        void should_handle_zero_duration_when_given_zero_value() {
            Duration d = GlobalDuration.of(0);
            assertEquals(0, d.get());
            assertEquals("00 h 00", d.toHoursFormat());
            assertEquals(0.0, d.toHours());
        }

        @Test
        void should_handle_large_value_when_given_high_duration() {
            Duration d = GlobalDuration.of(Integer.MAX_VALUE);
            assertTrue(d.get() > 0);
            assertDoesNotThrow(d::toHoursFormat);
            assertDoesNotThrow(d::toHours);
        }

        @Test
        void should_return_non_equal_hashcode_when_given_different_values() {
            Duration d1 = GlobalDuration.of(1);
            Duration d2 = GlobalDuration.of(2);
            assertNotEquals(d1.hashCode(), d2.hashCode());
        }
    }
}
