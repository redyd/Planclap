package org.helmo.planclap_admin.domains.core;

import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Nested;
import org.junit.jupiter.api.Test;


import static org.junit.jupiter.api.Assertions.*;

/**
 * Classe de test générée en partie par IA
 * Correction manuelle
 */
class CineCheckAgeTest {

    @Nested
    @DisplayName("Happy path")
    class HappyPath {

        @Test
        void should_success_when_correct_int_values() {
            assertAll(
                    () -> assertEquals(0, CineCheckAge.AL.getAge()),
                    () -> assertEquals(6, CineCheckAge.SIX.getAge()),
                    () -> assertEquals(9, CineCheckAge.NINE.getAge()),
                    () -> assertEquals(12, CineCheckAge.TWELVE.getAge()),
                    () -> assertEquals(14, CineCheckAge.FOURTEEN.getAge()),
                    () -> assertEquals(16, CineCheckAge.SIXTEEN.getAge()),
                    () -> assertEquals(18, CineCheckAge.EIGHTEEN.getAge())
            );
        }

        @Test
        void should_success_when_every_case() {
            assertAll(
                    () -> assertTrue(CineCheckAge.SIX.equalsValue("six")),
                    () -> assertTrue(CineCheckAge.SIX.equalsValue("SIX")),
                    () -> assertTrue(CineCheckAge.SIX.equalsValue("6"))
            );
        }

        @Test
        void should_return_correct_string_representation_when_toliteralstring() {
            assertEquals("AL", CineCheckAge.AL.toLiteralString());
            assertEquals("12", CineCheckAge.TWELVE.toLiteralString());
            assertEquals("9", CineCheckAge.NINE.toLiteralString());
        }
    }

    @Nested
    @DisplayName("Error path")
    class ErrorPath {

        @Test
        void should_return_false_when_invalid_value_given_to_stringEquals() {
            assertAll(
                    () -> assertFalse(CineCheckAge.AL.equalsValue(null)),
                    () -> assertFalse(CineCheckAge.AL.equalsValue("")),
                    () -> assertFalse(CineCheckAge.SIX.equalsValue("7")),
                    () -> assertFalse(CineCheckAge.EIGHTEEN.equalsValue("adult"))
            );
        }
    }
}
