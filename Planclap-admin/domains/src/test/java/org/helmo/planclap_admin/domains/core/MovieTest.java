package org.helmo.planclap_admin.domains.core;

import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Nested;
import org.junit.jupiter.api.Test;

import java.util.Set;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.Mockito.*;

/**
 * Classe de test générée en partie par IA
 * Correction manuelle
 */
class MovieTest {

    @Nested
    @DisplayName("Happy path")
    class HappyPath {

        @Test
        void should_return_expected_values_when_given_valid_movie() {
            Name title = mock(Name.class);
            when(title.toMinimal()).thenReturn(mock(Name.class));

            MovieDescription description = mock(MovieDescription.class);
            MovieDuration duration = mock(MovieDuration.class);
            PosterURI poster = mock(PosterURI.class);
            CineCheckGroup cineCheck = mock(CineCheckGroup.class);
            when(cineCheck.isForChildren()).thenReturn(true);

            Movie movie = new Movie(title, description, duration, poster, cineCheck);

            assertEquals(title.toMinimal(), movie.getSlug());
            assertEquals(title, movie.getTitle());
            assertEquals(description, movie.getDescription());
            assertEquals(duration, movie.getDuration());
            assertEquals(poster, movie.getPoster());
            assertEquals(cineCheck, movie.getCineChecksGroup());
            assertTrue(movie.isForChildren());
        }

        @Test
        void should_return_false_for_children_when_given_non_child_movie() {
            CineCheckGroup cineCheck = mock(CineCheckGroup.class);
            when(cineCheck.isForChildren()).thenReturn(false);

            Movie movie = new Movie(
                    mock(Name.class),
                    mock(MovieDescription.class),
                    mock(MovieDuration.class),
                    mock(PosterURI.class),
                    cineCheck
            );

            assertFalse(movie.isForChildren());
        }

        @Test
        void should_respect_equals_and_hashcode_when_given_equal_movies() {
            Name title = mock(Name.class);
            when(title.toMinimal()).thenReturn(mock(Name.class));

            MovieDescription desc = mock(MovieDescription.class);
            MovieDuration duration = mock(MovieDuration.class);
            PosterURI poster = mock(PosterURI.class);
            CineCheckGroup cineCheck = mock(CineCheckGroup.class);

            Movie m1 = new Movie(title, desc, duration, poster, cineCheck);
            Movie m2 = new Movie(title, desc, duration, poster, cineCheck);

            assertEquals(m1, m2);
            assertEquals(m1.hashCode(), m2.hashCode());
        }

        @Test
        void should_not_be_equal_when_given_different_movies() {
            Movie m1 = new Movie(
                    mock(Name.class),
                    mock(MovieDescription.class),
                    mock(MovieDuration.class),
                    mock(PosterURI.class),
                    mock(CineCheckGroup.class)
            );

            Movie m2 = new Movie(
                    mock(Name.class),
                    mock(MovieDescription.class),
                    mock(MovieDuration.class),
                    mock(PosterURI.class),
                    mock(CineCheckGroup.class)
            );

            assertNotEquals(m1, m2);
            assertNotEquals(m1.hashCode(), m2.hashCode());
        }

        @Test
        void should_not_be_equal_when_given_null_or_other_class() {
            Movie m = new Movie(
                    mock(Name.class),
                    mock(MovieDescription.class),
                    mock(MovieDuration.class),
                    mock(PosterURI.class),
                    mock(CineCheckGroup.class)
            );

            assertNotEquals(m, null);
            assertNotEquals(m, "not a movie");
        }

        @Test
        void should_not_be_equals_when_differents_name() {
            Movie m1 = new Movie(
                    Name.of("Movie1"),
                    MovieDescription.of("Description"),
                    MovieDuration.of(100),
                    PosterURI.of("www.poster.com"),
                    CineCheckGroup.of(CineCheckAge.EIGHTEEN, Set.of()));

            Movie m2 = new Movie(
                    Name.of("Movie"),
                    MovieDescription.of("Description"),
                    MovieDuration.of(100),
                    PosterURI.of("www.poster.com"),
                    CineCheckGroup.of(CineCheckAge.EIGHTEEN, Set.of()));

            assertNotEquals(m1, m2);
        }

        @Test
        void should_not_be_equals_when_differents_poster() {
            Movie m1 = new Movie(
                    Name.of("Movie"),
                    MovieDescription.of("Description"),
                    MovieDuration.of(100),
                    PosterURI.of("www.poster.com"),
                    CineCheckGroup.of(CineCheckAge.EIGHTEEN, Set.of()));

            Movie m2 = new Movie(
                    Name.of("Movie"),
                    MovieDescription.of("Description"),
                    MovieDuration.of(100),
                    PosterURI.of("www.poster.com/view.php"),
                    CineCheckGroup.of(CineCheckAge.EIGHTEEN, Set.of()));

            assertNotEquals(m1, m2);
        }

        @Test
        void should_not_be_equals_when_differents_duration() {
            Movie m1 = new Movie(
                    Name.of("Movie"),
                    MovieDescription.of("Description"),
                    MovieDuration.of(100),
                    PosterURI.of("www.poster.com"),
                    CineCheckGroup.of(CineCheckAge.EIGHTEEN, Set.of()));

            Movie m2 = new Movie(
                    Name.of("Movie"),
                    MovieDescription.of("Description"),
                    MovieDuration.of(101),
                    PosterURI.of("www.poster.com"),
                    CineCheckGroup.of(CineCheckAge.EIGHTEEN, Set.of()));

            assertNotEquals(m1, m2);
        }

        @Test
        void should_not_be_equals_when_differents_cinechecks() {
            Movie m1 = new Movie(
                    Name.of("Movie"),
                    MovieDescription.of("Description"),
                    MovieDuration.of(100),
                    PosterURI.of("www.poster.com"),
                    CineCheckGroup.of(CineCheckAge.AL, Set.of()));

            Movie m2 = new Movie(
                    Name.of("Movie"),
                    MovieDescription.of("Description"),
                    MovieDuration.of(100),
                    PosterURI.of("www.poster.com"),
                    CineCheckGroup.of(CineCheckAge.EIGHTEEN, Set.of()));

            assertNotEquals(m1, m2);
        }

        @Test
        void should_not_be_equals_when_differents_description() {
            Movie m1 = new Movie(
                    Name.of("Movie"),
                    MovieDescription.of("Description super trop coool"),
                    MovieDuration.of(100),
                    PosterURI.of("www.poster.com"),
                    CineCheckGroup.of(CineCheckAge.EIGHTEEN, Set.of()));

            Movie m2 = new Movie(
                    Name.of("Movie"),
                    MovieDescription.of("Description"),
                    MovieDuration.of(100),
                    PosterURI.of("www.poster.com"),
                    CineCheckGroup.of(CineCheckAge.EIGHTEEN, Set.of()));

            assertNotEquals(m1, m2);
        }
    }

    @Nested
    @DisplayName("Error cases")
    class ErrorCases {

        @Test
        void should_handle_cinecheck_null_behavior_when_given_mock() {
            CineCheckGroup cineCheck = mock(CineCheckGroup.class);
            when(cineCheck.isForChildren()).thenReturn(false);

            Movie m = new Movie(
                    mock(Name.class),
                    mock(MovieDescription.class),
                    mock(MovieDuration.class),
                    mock(PosterURI.class),
                    cineCheck
            );

            assertFalse(m.isForChildren());
        }

        @Test
        void should_throw_exception_when_null_given() {
            assertThrows(NullPointerException.class, () -> new Movie(null, mock(MovieDescription.class), mock(MovieDuration.class), mock(PosterURI.class), mock(CineCheckGroup.class)));
            assertThrows(NullPointerException.class, () -> new Movie(mock(Name.class), null, mock(MovieDuration.class), mock(PosterURI.class), mock(CineCheckGroup.class)));
            assertThrows(NullPointerException.class, () -> new Movie(mock(Name.class), mock(MovieDescription.class), null, mock(PosterURI.class), mock(CineCheckGroup.class)));
            assertThrows(NullPointerException.class, () -> new Movie(mock(Name.class), mock(MovieDescription.class), mock(MovieDuration.class), null, mock(CineCheckGroup.class)));
            assertThrows(NullPointerException.class, () -> new Movie(mock(Name.class), mock(MovieDescription.class), mock(MovieDuration.class), mock(PosterURI.class), null));
        }
    }
}
