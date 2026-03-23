package org.helmo.planclap_admin.domains.core;

import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Nested;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.*;

/**
 * Classe de test générée en partie par IA
 * Correction manuelle
 */
class MovieDescriptionTest {

    @Nested
    @DisplayName("Happy path")
    class HappyPath {

        @Test
        void should_create_movie_description_when_valid_text_given()  {
            var description = MovieDescription.of("Un film d'action captivant");
            assertNotNull(description);
        }

        @Test
        void should_create_movie_description_when_length_is_exactly_200_characters()  {
            String longDescription = "a".repeat(200);
            var description = MovieDescription.of(longDescription);
            assertNotNull(description);
        }

        @Test
        void should_create_movie_description_when_length_is_1_character()  {
            var description = MovieDescription.of("a");
            assertNotNull(description);
        }

        @Test
        void should_return_true_when_same_description() {
            var desc1 = MovieDescription.of("Un super film");
            var desc2 = MovieDescription.of("Un super film");

            assertEquals(desc1, desc2);
        }

        @Test
        void should_return_false_when_different_description() {
            var desc1 = MovieDescription.of("Un super film");
            var desc2 = MovieDescription.of("Un autre film");

            assertNotEquals(desc1, desc2);
        }

        @Test
        void should_return_false_when_compared_with_null() {
            var desc = MovieDescription.of("Un film");
            assertNotEquals(desc, null);
        }

        @Test
        void should_return_false_when_compared_with_different_class() {
            var desc = MovieDescription.of("Un film");
            Object other = "Un film";
            assertNotEquals(desc, other);
        }

        @Test
        void should_have_same_hashcode_when_equals() {
            var desc1 = MovieDescription.of("Un film");
            var desc2 = MovieDescription.of("Un film");

            assertEquals(desc1.hashCode(), desc2.hashCode());
        }

        @Test
        void should_have_correct_string_representation() {
            var desc1 = MovieDescription.of("Un film");

            assertEquals("Un film", desc1.toString());
        }

    }

    @Nested
    @DisplayName("Error path")
    class ErrorPath {

        @Test
        void should_throw_exception_when_description_is_null() {
            assertThrows(IllegalArgumentException.class, () -> MovieDescription.of(null));
        }

        @Test
        void should_throw_exception_when_description_is_empty() {
            assertThrows(IllegalArgumentException.class, () -> MovieDescription.of(""));
        }

        @Test
        void should_throw_exception_when_description_is_too_long() {
            String tooLong = "a".repeat(241);
            assertThrows(IllegalArgumentException.class, () -> MovieDescription.of(tooLong));
        }
    }
}
