package org.helmo.planclap_admin.domains.core;

import org.junit.jupiter.api.Nested;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.Mockito.*;

/**
 * Utilisation partielle de l'IA
 */
class MovieSessionsTest {

    @Nested
    class HappyPath {

        @Test
        void should_return_movie_when_given_valid_movie_session() {
            Movie movie = mock(Movie.class);
            SessionAmount sessionsAmount = mock(SessionAmount.class);

            MovieSessions ms = new MovieSessions(movie, sessionsAmount);

            assertEquals(movie, ms.movie());
        }

        @Test
        void should_return_total_duration_when_given_movie_and_sessions() {
            Movie movie = mock(Movie.class);
            MovieDuration duration = mock(MovieDuration.class);
            when(movie.getDuration()).thenReturn(duration);

            SessionAmount sessionsAmount = mock(SessionAmount.class);
            when(sessionsAmount.toInt()).thenReturn(3);

            GlobalDuration expectedTotal = mock(GlobalDuration.class);
            when(duration.multiplyBy(3)).thenReturn(expectedTotal);

            MovieSessions ms = new MovieSessions(movie, sessionsAmount);

            assertEquals(expectedTotal, ms.getTotalDuration());
        }

        @Test
        void should_return_true_when_equals_on_slug_given_same_slug() {
            Name slug = mock(Name.class);
            Movie movie = mock(Movie.class);
            when(movie.getSlug()).thenReturn(slug);

            MovieSessions ms = new MovieSessions(movie, mock(SessionAmount.class));

            assertTrue(ms.equalsOnSlug(slug));
        }

        @Test
        void should_return_true_when_same_slug() {
            var movie1 = mockMovieWithSlug("slug-123");
            var movie2 = mockMovieWithSlug("slug-123");
            var sessions1 = new MovieSessions(movie1, SessionAmount.of(3));
            var sessions2 = new MovieSessions(movie2, SessionAmount.of(5));

            assertEquals(sessions1, sessions2);
        }

        @Test
        void should_return_false_when_different_slug() {
            var movie1 = mockMovieWithSlug("slug-123");
            var movie2 = mockMovieWithSlug("slug-456");
            var sessions1 = new MovieSessions(movie1, SessionAmount.of(3));
            var sessions2 = new MovieSessions(movie2, SessionAmount.of(3));

            assertNotEquals(sessions1, sessions2);
        }

        @Test
        void should_return_false_when_compared_with_null() {
            var movie = mockMovieWithSlug("slug-xyz");
            var sessions = new MovieSessions(movie, SessionAmount.of(2));

            assertNotEquals(sessions, null);
        }

        @Test
        void should_return_false_when_compared_with_different_class() {
            var movie = mockMovieWithSlug("slug-xyz");
            var sessions = new MovieSessions(movie, SessionAmount.of(2));

            Object other = "not a MovieSessions";
            assertNotEquals(sessions, other);
        }

        @Test
        void should_return_true_when_same_instance() {
            var movie = mockMovieWithSlug("slug-xyz");
            var sessions = new MovieSessions(movie, SessionAmount.of(2));

            assertEquals(sessions, sessions);
        }

    }

    @Nested
    class ErrorCases {

        @Test
        void should_return_false_for_equals_on_slug_when_given_different_slug() {
            Name slug1 = mock(Name.class);
            Name slug2 = mock(Name.class);
            Movie movie = mock(Movie.class);
            when(movie.getSlug()).thenReturn(slug1);

            MovieSessions ms = new MovieSessions(movie, mock(SessionAmount.class));

            assertFalse(ms.equalsOnSlug(slug2));
        }

        @Test
        void should_not_equal_when_given_null_or_other_class() {
            MovieSessions ms = new MovieSessions(mock(Movie.class), mock(SessionAmount.class));

            assertNotEquals(null, ms);
            assertNotEquals("not a moviesession", ms);
        }
    }

    private Movie mockMovieWithSlug(String slugValue) {
        var slug = Name.of(slugValue);
        var movie = org.mockito.Mockito.mock(Movie.class);
        org.mockito.Mockito.when(movie.getSlug()).thenReturn(slug);
        return movie;
    }
}