package org.helmo.planclap_admin.domains.core;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Nested;
import org.junit.jupiter.api.Test;

import java.util.HashSet;
import java.util.List;
import java.util.Set;

import static org.helmo.planclap_admin.domains.providers.TestProvider.mockedSessions;
import static org.junit.jupiter.api.Assertions.*;

/**
 * Test généré à l'aide de l'IA
 */
class SlugSearcherTest {

    private SlugSearcher slugSearcher;

    @BeforeEach
    void setUp() {
        slugSearcher = new SlugSearcher();
        Set<MovieSessions> sessions = new HashSet<>(mockedSessions(
                List.of("Dora 2", "Viana 2", "Cars 3", "Les aventures de Tchoupi", "Avengers")
        ));

        // Ajout dans l’instance du searcher
        for (var session : sessions) {
            slugSearcher.AddMovieSessions(session);
        }
    }

    @Nested
    @DisplayName("Happy path")
    class HappyPath {

        @Test
        void should_find_movie_when_same_slug_given() {
            var result = slugSearcher.search(Name.of("Cars 3"));

            assertTrue(result.isPresent());
            assertEquals("Cars 3", result.get().movie().getSlug().toString());
        }

        @Test
        void should_find_movie_when_distance_is_1() {
            var result1 = slugSearcher.search(Name.of("cars 3"));
            var result2 = slugSearcher.search(Name.of("viana 2"));

            assertTrue(result1.isPresent());
            assertTrue(result2.isPresent());

            assertEquals("Cars 3",  result1.get().movie().getSlug().toString());
            assertEquals("Viana 2", result2.get().movie().getSlug().toString());
        }

        @Test
        void should_find_movie_when_distance_is_2() {
            var result1 = slugSearcher.search(Name.of("Cars"));
            var result2 = slugSearcher.search(Name.of("les aventures de tchoupi"));

            assertTrue(result1.isPresent());
            assertTrue(result2.isPresent());

            assertEquals("Cars 3",  result1.get().movie().getSlug().toString());
            assertEquals("Les aventures de Tchoupi", result2.get().movie().getSlug().toString());
        }

        @Test
        void should_find_movie_when_distance_is_3() {
            var result1 = slugSearcher.search(Name.of("cars"));
            var result2 = slugSearcher.search(Name.of("avagers"));

            assertTrue(result1.isPresent());
            assertTrue(result2.isPresent());

            assertEquals("Cars 3",  result1.get().movie().getSlug().toString());
            assertEquals("Avengers", result2.get().movie().getSlug().toString());
        }

        @Test
        void should_handle_multiple_movies_with_same_distance() {
            var result = slugSearcher.search(Name.of("Dora"));

            assertTrue(result.isPresent());
            assertEquals("Dora 2", result.get().movie().getSlug().toString());
        }

        @Test
        void should_return_first_when_multiple_matches() {
            var multipleSearcher = new SlugSearcher();
            var multipleSessions = new HashSet<>(mockedSessions(
                    List.of("Test 1", "Test 2", "Test 3")
            ));
            for (var session : multipleSessions) {
                multipleSearcher.AddMovieSessions(session);
            }

            var result = multipleSearcher.search(Name.of("Tes"));

            assertTrue(result.isPresent());
            assertTrue(result.get().movie().getSlug().toString().startsWith("Test"));
        }

        @Test
        void should_add_to_existing_list_in_map() {
            var similarSearcher = new SlugSearcher();
            var similarSessions = new HashSet<>(mockedSessions(
                    List.of("ABC", "ABD", "ABE") // Tous à distance 1 de "ABX"
            ));
            for (var session : similarSessions) {
                similarSearcher.AddMovieSessions(session);
            }

            var result = similarSearcher.search(Name.of("ABX"));
            assertTrue(result.isPresent());
        }

        @Test
        void should_stop_flattening_when_distance_exceeds_tolerance() {
            var mixedSearcher = new SlugSearcher();
            var mixedSessions = new HashSet<>(mockedSessions(
                    List.of("Perfect", "VeryDifferentMovie")
            ));
            for (var session : mixedSessions) {
                mixedSearcher.AddMovieSessions(session);
            }

            var result = mixedSearcher.search(Name.of("Perfet"));

            assertTrue(result.isPresent());
            assertEquals("Perfect", result.get().movie().getSlug().toString());
        }
    }

    @Nested
    @DisplayName("Error cases")
    class ErrorCases {

        @Test
        void should_not_find_movie_when_distance_is_greater_than_3() {
            var result1 = slugSearcher.search(Name.of("Cars 4: remake"));
            var result2 = slugSearcher.search(Name.of("Les aventures de"));

            assertTrue(result1.isEmpty());
            assertTrue(result2.isEmpty());
        }

        @Test
        void should_return_empty_when_no_sessions() {
            var emptySearcher = new SlugSearcher();

            var result = emptySearcher.search(Name.of("Any movie"));

            assertTrue(result.isEmpty());
        }

        @Test
        void should_return_empty_when_all_exceed_tolerance() {
            var differentSearcher = new SlugSearcher();
            var differentSessions = new HashSet<>(mockedSessions(
                    List.of("Completely different title here")
            ));
            for (var session : differentSessions) {
                differentSearcher.AddMovieSessions(session);
            }

            var result = differentSearcher.search(Name.of("ABC"));

            assertTrue(result.isEmpty());
        }
    }
}
