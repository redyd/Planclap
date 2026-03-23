package org.helmo.planclap_admin.domains.core;

import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Nested;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.*;

/**
 * Classe de test générée en partie par IA
 * Correction manuelle
 */
public class SessionAmountTest {

    @Nested
    @DisplayName("Happy path")
    class HappyPath {

        @Test
        void should_create_valid_session_amount() throws IllegalArgumentException {
            for (int i = 1; i <= 9; i++) {
                SessionAmount amount = SessionAmount.of(i);
                assertEquals(i, amount.toInt(), "SessionAmount value should match input");
            }
        }

        @Test
        void should_consider_equal_when_same_value() throws IllegalArgumentException {
            SessionAmount s1 = SessionAmount.of(3);
            SessionAmount s2 = SessionAmount.of(3);

            assertEquals(s1, s2);
            assertEquals(s1.hashCode(), s2.hashCode());
        }

        @Test
        void should_not_consider_equal_when_different_value() throws IllegalArgumentException {
            SessionAmount s1 = SessionAmount.of(2);
            SessionAmount s2 = SessionAmount.of(5);

            assertNotEquals(s1, s2);
        }

    }

    @Nested
    @DisplayName("Error cases")
    class ErrorCases {

        @Test
        void should_throw_exception_when_value_below_1() {
            assertThrows(IllegalArgumentException.class, () -> SessionAmount.of(0));
            assertThrows(IllegalArgumentException.class, () -> SessionAmount.of(-5));
        }

        @Test
        void should_throw_exception_when_value_above_9() {
            assertThrows(IllegalArgumentException.class, () -> SessionAmount.of(10));
            assertThrows(IllegalArgumentException.class, () -> SessionAmount.of(99));
        }

        @Test
        void should_return_false_for_invalid_equals_comparisons() throws IllegalArgumentException {
            SessionAmount s = SessionAmount.of(4);

            assertAll(
                    () -> assertNotEquals(null, s),
                    () -> assertNotEquals("string", s)
            );
        }

        @Test
        void should_not_be_equals_when_obj_given() {
            SessionAmount s = SessionAmount.of(4);

            assertNotEquals(s, new Object());
        }

    }
}
