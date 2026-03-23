package org.helmo.planclap_admin.domains.core;

import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Nested;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.*;

/**
 * Classe de test généré en partie avec IA
 * Correction manuele
 */
class WeekDayTest {

    @Nested
    @DisplayName("Happy path")
    class HappyPath {

        @Test
        void should_return_correct_value_when_getDayPosition_called() {
            assertEquals(0, WeekDay.MONDAY.getDayPosition());
            assertEquals(1, WeekDay.TUESDAY.getDayPosition());
            assertEquals(2, WeekDay.WEDNESDAY.getDayPosition());
            assertEquals(3, WeekDay.THURSDAY.getDayPosition());
            assertEquals(4, WeekDay.FRIDAY.getDayPosition());
            assertEquals(5, WeekDay.SATURDAY.getDayPosition());
            assertEquals(6, WeekDay.SUNDAY.getDayPosition());
        }

        @Test
        void should_return_correct_weekday_when_from_given_valid_value() {
            assertEquals(WeekDay.MONDAY, WeekDay.from(0));
            assertEquals(WeekDay.TUESDAY, WeekDay.from(1));
            assertEquals(WeekDay.WEDNESDAY, WeekDay.from(2));
            assertEquals(WeekDay.THURSDAY, WeekDay.from(3));
            assertEquals(WeekDay.FRIDAY, WeekDay.from(4));
            assertEquals(WeekDay.SATURDAY, WeekDay.from(5));
            assertEquals(WeekDay.SUNDAY, WeekDay.from(6));
        }
    }

    @Nested
    @DisplayName("Error cases")
    class ErrorCases {

        @Test
        void should_throw_exception_when_from_given_negative_value() {
            assertThrows(IllegalArgumentException.class, () -> WeekDay.from(-1));
        }

        @Test
        void should_throw_exception_when_from_given_value_greater_than_six() {
            assertThrows(IllegalArgumentException.class, () -> WeekDay.from(7));
        }
    }
}
