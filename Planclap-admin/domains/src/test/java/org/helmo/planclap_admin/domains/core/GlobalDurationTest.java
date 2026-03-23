package org.helmo.planclap_admin.domains.core;

import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Nested;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.*;

/**
 * Classe de test générée en partie par IA
 * Correction manuelle
 */
class GlobalDurationTest {

    @Nested
    @DisplayName("happy path")
    class HappyPath {

        @Test
        void should_clamp_to_zero_when_given_negative_value() {
            GlobalDuration d = GlobalDuration.of(-50);
            assertEquals(0, d.get());
        }

        @Test
        void should_keep_value_when_given_positive_value() {
            GlobalDuration d = GlobalDuration.of(200);
            assertEquals(200, d.get());
        }

        @Test
        void should_clamp_to_zero_when_given_negative_hours() {
            GlobalDuration d = GlobalDuration.ofHours(-5);
            assertEquals(0, d.get());
        }

        @Test
        void should_convert_hours_to_minutes_when_given_positive_hours() {
            GlobalDuration d = GlobalDuration.ofHours(3);
            assertEquals(180, d.get());
        }

        @Test
        void should_return_formatted_hhmm_when_given_duration_in_minutes() {
            GlobalDuration d = GlobalDuration.of(125);
            assertEquals("02 h 05", d.toHoursFormat());
        }

        @Test
        void should_return_formatted_hhmm_with_zero_padding_when_given_small_duration() {
            GlobalDuration d = GlobalDuration.of(7);
            assertEquals("00 h 07", d.toHoursFormat());
        }

        @Test
        void should_return_hours_as_double_when_given_duration_in_minutes() {
            GlobalDuration d = GlobalDuration.of(180);
            assertEquals(3.0, d.toHours(), 0.0001);
        }

        @Test
        void should_add_durations_when_given_another_duration() {
            GlobalDuration d1 = GlobalDuration.of(100);
            Duration d2 = GlobalDuration.of(50);
            GlobalDuration result = d1.plus(d2);
            assertEquals(150, result.get());
        }

        @Test
        void should_return_same_value_when_given_zero_duration() {
            GlobalDuration d1 = GlobalDuration.of(100);
            Duration d2 = GlobalDuration.of(0);
            GlobalDuration result = d1.plus(d2);
            assertEquals(100, result.get());
        }

        @Test
        void should_respect_equals_and_hashcode_contract_when_given_equal_and_unequal_values() {
            GlobalDuration d1 = GlobalDuration.of(90);
            GlobalDuration d2 = GlobalDuration.of(90);
            GlobalDuration d3 = GlobalDuration.of(150);
            assertEquals(d1, d2);
            assertEquals(d1.hashCode(), d2.hashCode());
            assertNotEquals(d1, d3);
            assertNotEquals(d1.hashCode(), d3.hashCode());
        }

        @Test
        void should_return_expected_string_representation_when_given_valid_duration() {
            GlobalDuration d = GlobalDuration.of(90);
            String s = d.toHoursFormat();
            assertEquals("01 h 30", s);
        }

        @Test
        void should_have_a_correct_string_representation() {
            GlobalDuration d = GlobalDuration.of(120);

            assertEquals("GlobalDuration[02 h 00]", d.toString());
        }
    }

    @Nested
    @DisplayName("error cases")
    class ErrorCases {

        @Test
        void should_throw_nullpointer_when_given_null_duration() {
            GlobalDuration d = GlobalDuration.of(60);
            assertThrows(NullPointerException.class, () -> d.plus(null));
        }

        @Test
        void should_not_return_negative_when_given_negative_sum() {
            GlobalDuration d1 = GlobalDuration.of(-10);
            GlobalDuration d2 = GlobalDuration.of(-20);
            GlobalDuration result = d1.plus(d2);
            assertEquals(0, result.get());
        }

        @Test
        void should_handle_large_values_when_given_integer_max_value() {
            GlobalDuration d1 = GlobalDuration.of(Integer.MAX_VALUE);
            Duration d2 = GlobalDuration.of(1);
            GlobalDuration result = d1.plus(d2);
            assertTrue(result.get() >= 0);
        }
    }
}
