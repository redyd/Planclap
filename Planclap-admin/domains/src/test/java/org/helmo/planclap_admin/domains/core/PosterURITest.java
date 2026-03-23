package org.helmo.planclap_admin.domains.core;

import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Nested;
import org.junit.jupiter.api.Test;

import java.net.URI;
import java.net.URISyntaxException;

import static org.junit.jupiter.api.Assertions.*;

/**
 * Utilisation partielle de l'IA
 */
class PosterURITest {

    @Nested
    @DisplayName("Happy path")
    class HappyPath {

        @Test
        void should_create_poster_uri_when_given_valid_string() throws URISyntaxException {
            String uriStr = "http://example.com/poster.jpg";
            PosterURI poster = PosterURI.of(uriStr);

            assertEquals(new URI(uriStr), poster.url());
        }

        @Test
        void should_return_same_uri_for_equals_when_given_same_uri() {
            PosterURI p1 = PosterURI.of("http://example.com/poster.jpg");
            PosterURI p2 = PosterURI.of("http://example.com/poster.jpg");

            assertEquals(p1, p2);
            assertEquals(p1.hashCode(), p2.hashCode());
        }

        @Test
        void should_not_equal_when_given_different_uri() {
            PosterURI p1 = PosterURI.of("http://example.com/poster1.jpg");
            PosterURI p2 = PosterURI.of("http://example.com/poster2.jpg");

            assertNotEquals(p1, p2);
            assertNotEquals(p1.hashCode(), p2.hashCode());
        }

        @Test
        void should_not_equal_when_given_null_or_other_class() {
            PosterURI p = PosterURI.of("http://example.com/poster.jpg");

            assertNotEquals(p, null);
            assertNotEquals(p, "not a posteruri");
        }
    }

    @Nested
    @DisplayName("Error cases")
    class ErrorCases {

        @Test
        void should_throw_exception_when_given_invalid_uri_string() {
            assertThrows(IllegalArgumentException.class, () -> PosterURI.of("ht!tp://bad uri"));
        }

        @Test
        void should_throw_exception_when_nul_or_blank_given() {
            assertThrows(IllegalArgumentException.class, () -> PosterURI.of(""));
            assertThrows(IllegalArgumentException.class, () -> PosterURI.of(null));
        }
    }
}
