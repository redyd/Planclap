package org.helmo.planclap_admin.domains.core;

import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Nested;
import org.junit.jupiter.api.Test;


import static org.junit.jupiter.api.Assertions.*;

/**
 * Classe de test générée en partie par IA
 * Correction manuelle
 */
class CineCheckTest {

    @Nested
    @DisplayName("Happy path")
    class HappyPath {

        @Test
        void should_return_correct_descriptions_when_enum_values_given() {
            assertAll(() -> assertEquals("Violence", CineCheck.VIOLENCE.getDescription()), () -> assertEquals("Peur", CineCheck.FEAR.getDescription()), () -> assertEquals("Sexe", CineCheck.SEX.getDescription()), () -> assertEquals("Paroles grossieres", CineCheck.RUDE.getDescription()), () -> assertEquals("Discrimination", CineCheck.DISCRIMINATION.getDescription()), () -> assertEquals("Drogues, alcool et fumer", CineCheck.DRUGS.getDescription()));
        }

        @Test
        void should_return_true_when_string_matches_name_or_description_given_any_case() {
            assertAll(() -> assertTrue(CineCheck.VIOLENCE.equalsValue("VIOLENCE")), () -> assertTrue(CineCheck.VIOLENCE.equalsValue("violence")), () -> assertTrue(CineCheck.VIOLENCE.equalsValue("Violence")), () -> assertTrue(CineCheck.FEAR.equalsValue("Peur")), () -> assertTrue(CineCheck.FEAR.equalsValue("peur")), () -> assertTrue(CineCheck.DRUGS.equalsValue("Drogues, alcool et fumer")));
        }

    }

    @Nested
    @DisplayName("Error path")
    class ErrorPath {

        @Test
        void should_return_false_when_invalid_or_empty_value_given_to_stringEquals() {
            assertAll(() -> assertFalse(CineCheck.VIOLENCE.equalsValue(null)), () -> assertFalse(CineCheck.VIOLENCE.equalsValue("")), () -> assertFalse(CineCheck.VIOLENCE.equalsValue("other")), () -> assertFalse(CineCheck.DRUGS.equalsValue("alcool")), () -> assertFalse(CineCheck.SEX.equalsValue("sexual")));
        }

    }
}
