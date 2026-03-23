package org.helmo.planclap_admin.domains.core;

import org.helmo.planclap_admin.domains.core.StripUtils;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.*;

/**
 * Classe de test générée en partie par IA
 * Correction manuelle
 */
public class StripUtilsTest {

    @Test
    void should_remove_accents_from_simple_words() {
        String input = "éèêëàâäçôöûüîïñ";
        String expected = "eeeeaaacoouuiin";
        String actual = StripUtils.stripAccents(input);
        assertEquals(expected, actual);
    }

    @Test
    void should_handle_mixed_characters() {
        String input = "Café crème brûlé";
        String expected = "Cafe creme brule";
        assertEquals(expected, StripUtils.stripAccents(input));
    }

    @Test
    void should_return_empty_when_input_is_empty() {
        assertEquals("", StripUtils.stripAccents(""));
    }

    @Test
    void should_return_empty_when_input_is_null() {
        assertEquals("", StripUtils.stripAccents(null));
    }

    @Test
    void should_not_alter_string_without_accents() {
        String input = "Bonjour, test 123!";
        assertEquals(input, StripUtils.stripAccents(input));
    }

    @Test
    void should_handle_uppercase_letters_with_accents() {
        String input = "ÉLÈVE À L'UNIVERSITÉ";
        String expected = "ELEVE A L'UNIVERSITE";
        assertEquals(expected, StripUtils.stripAccents(input));
    }

    @Test
    void should_correctly_strip_accents_in_mixed_case_text() {
        String input = "Ça c'Est l'été À Noël!";
        String expected = "Ca c'Est l'ete A Noel!";
        assertEquals(expected, StripUtils.stripAccents(input));
    }

    @Test
    void should_keep_special_characters_and_numbers_intact() {
        String input = "Hôtel 123 @#%!";
        String expected = "Hotel 123 @#%!";
        assertEquals(expected, StripUtils.stripAccents(input));
    }
}
