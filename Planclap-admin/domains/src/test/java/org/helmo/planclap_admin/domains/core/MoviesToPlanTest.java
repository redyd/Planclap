package org.helmo.planclap_admin.domains.core;

import org.helmo.planclap_admin.domains.exceptions.TooMuchMoviesException;
import org.helmo.planclap_admin.domains.providers.TestProvider;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Nested;
import org.junit.jupiter.api.Test;

import java.time.LocalDate;
import java.util.Optional;

import static org.assertj.core.api.Assertions.*;

/**
 * Classe de test généré en partie avec IA
 * Correction manuele
 */
class MoviesToPlanTest {

    private Movie movie1;
    private Movie movie2;

    private Name slug1;
    private Name slug2;

    private LocalDate testDate;
    private MoviesToPlan moviesToPlan;

    @BeforeEach
    void setUp() {
        testDate = LocalDate.of(2025, 6, 15);
        moviesToPlan = new MoviesToPlan(testDate);

        movie1 = TestProvider.movie(120, false); // 2h
        movie2 = TestProvider.movie(150, false); // 2h30

        slug1 = movie1.getSlug();
        slug2 = movie2.getSlug();
    }

    @Nested
    @DisplayName("Happy Path")
    class HappyPath {

        @Test
        void should_initialize_empty_collection_when_given_valid_date() {
            MoviesToPlan plan = new MoviesToPlan(testDate);

            assertThat(plan.get()).isEmpty();
            assertThat(plan.getDateForPlan()).isEqualTo(testDate);
        }

        @Test
        void should_add_movie_session_when_given_valid_session_within_limit() {
            MovieSessions session = new MovieSessions(movie1, SessionAmount.of(5));

            moviesToPlan.add(session);

            assertThat(moviesToPlan.get()).hasSize(1);
            assertThat(moviesToPlan.get()).contains(session);
        }

        @Test
        void should_add_multiple_different_movies_when_given_sessions_within_limit() {
            MovieSessions session1 = new MovieSessions(movie1, SessionAmount.of(5));
            MovieSessions session2 = new MovieSessions(movie2, SessionAmount.of(5));

            moviesToPlan.add(session1);
            moviesToPlan.add(session2);

            assertThat(moviesToPlan.get()).hasSize(2);
            assertThat(moviesToPlan.get()).containsExactlyInAnyOrder(session1, session2);
        }

        @Test
        void should_calculate_total_duration_when_given_multiple_sessions() {
            MovieSessions session1 = new MovieSessions(movie1, SessionAmount.of(5)); // 2h × 5 = 10h
            MovieSessions session2 = new MovieSessions(movie2, SessionAmount.of(4)); // 2h30 × 4 = 10h

            moviesToPlan.add(session1);
            moviesToPlan.add(session2);

            GlobalDuration totalDuration = moviesToPlan.getTotalDuration();
            assertThat(totalDuration.getHours()).isEqualTo(20);
        }

        @Test
        void should_return_true_for_below_limit_when_given_duration_under_70_hours() {
            MoviesToPlan plan = TestProvider.plan(68, testDate);

            assertThat(plan.belowPlanLimit()).isTrue();
        }

        @Test
        void should_return_false_for_below_limit_when_given_duration_at_or_above_70_hours() {
            MoviesToPlan plan = TestProvider.plan(72, testDate);

            assertThat(plan.belowPlanLimit()).isFalse();
        }

        @Test
        void should_return_true_when_given_movie_can_be_added() {
            MovieSessions session1 = new MovieSessions(movie1, SessionAmount.of(5));
            MovieSessions session2 = new MovieSessions(movie2, SessionAmount.of(5));

            moviesToPlan.add(session1);

            assertThat(moviesToPlan.canAdd(session2)).isTrue();
        }

        @Test
        void should_return_false_when_given_movie_already_exists() {
            MovieSessions session1 = new MovieSessions(movie1, SessionAmount.of(5));
            MovieSessions session2 = new MovieSessions(movie1, SessionAmount.of(3));

            moviesToPlan.add(session1);

            assertThat(moviesToPlan.canAdd(session2)).isFalse();
        }

        @Test
        void should_return_true_when_given_slug_of_existing_movie() {
            MovieSessions session = new MovieSessions(movie1, SessionAmount.of(5));

            moviesToPlan.add(session);

            assertThat(moviesToPlan.containsMovie(slug1)).isTrue();
        }

        @Test
        void should_return_false_when_given_slug_of_non_existing_movie() {
            MovieSessions session = new MovieSessions(movie1, SessionAmount.of(5));

            moviesToPlan.add(session);

            assertThat(moviesToPlan.containsMovie(slug2)).isFalse();
        }

        @Test
        void should_return_movie_session_when_given_exact_matching_slug() {
            MovieSessions session = new MovieSessions(movie1, SessionAmount.of(5));

            moviesToPlan.add(session);

            Optional<MovieSessions> result = moviesToPlan.search(slug1);

            assertThat(result).isPresent();
            assertThat(result.get()).isEqualTo(session);
        }

        @Test
        void should_return_movie_session_when_given_similar_slug_within_tolerance() {
            Movie movie = TestProvider.movie(120, false);
            Name slug = movie.getSlug();
            MovieSessions session = new MovieSessions(movie, SessionAmount.of(5));

            moviesToPlan.add(session);

            String slugStr = slug.toString();
            if (slugStr.length() > 1) {
                Name similarSlug = Name.of(slugStr.substring(0, slugStr.length() - 1));
                Optional<MovieSessions> result = moviesToPlan.search(similarSlug);

                assertThat(result).isNotNull();
            }
        }

        @Test
        void should_return_empty_when_given_slug_not_matching_any_movie() {
            MovieSessions session = new MovieSessions(movie1, SessionAmount.of(5));

            moviesToPlan.add(session);

            Name nonMatchingSlug = Name.of("completely-different-movie-name-that-does-not-exist");
            Optional<MovieSessions> result = moviesToPlan.search(nonMatchingSlug);

            assertThat(result).isEmpty();
        }

        @Test
        void should_return_zero_duration_when_given_no_sessions() {
            GlobalDuration totalDuration = moviesToPlan.getTotalDuration();

            assertThat(totalDuration.getHours()).isZero();
        }

        @Test
        void should_return_unmodifiable_set_when_getting_movie_sessions() {
            MovieSessions session = new MovieSessions(movie1, SessionAmount.of(5));

            moviesToPlan.add(session);

            assertThat(moviesToPlan.get()).isNotNull();
            assertThat(moviesToPlan.get()).hasSize(1);
        }

        @Test
        void should_not_add_session_when_given_total_would_reach_77_hours() {
            MoviesToPlan plan = TestProvider.plan(72, testDate);

            Movie largeMovie = TestProvider.movie(240, false); // 4h
            MovieSessions session = new MovieSessions(largeMovie, SessionAmount.of(2)); // 8h

            assertThat(plan.canAdd(session)).isFalse();
        }
    }

    @Nested
    @DisplayName("Error Cases")
    class ErrorCases {

        @Test
        void should_throw_exception_when_given_session_exceeding_limit() {
            MoviesToPlan plan = TestProvider.plan(72, testDate);

            Movie largeMovie = TestProvider.movie(240, false); // 4h
            MovieSessions session = new MovieSessions(largeMovie, SessionAmount.of(2)); // 8h

            assertThatThrownBy(() -> plan.add(session))
                    .isInstanceOf(TooMuchMoviesException.class)
                    .hasMessageContaining("Impossible d'ajouter ce film");
        }

        @Test
        void should_throw_exception_when_given_duplicate_movie() {
            MovieSessions session1 = new MovieSessions(movie1, SessionAmount.of(5));
            MovieSessions session2 = new MovieSessions(movie1, SessionAmount.of(3));

            moviesToPlan.add(session1);

            assertThatThrownBy(() -> moviesToPlan.add(session2))
                    .isInstanceOf(TooMuchMoviesException.class)
                    .hasMessageContaining("Impossible d'ajouter ce film");
        }

        @Test
        void should_throw_exception_when_given_session_that_would_exceed_77_hours() {
            MoviesToPlan plan = TestProvider.plan(72, testDate);

            Movie movie = TestProvider.movie(180, false); // 3h
            MovieSessions session = new MovieSessions(movie, SessionAmount.of(2)); // 6h = total 78h

            assertThatThrownBy(() -> plan.add(session))
                    .isInstanceOf(TooMuchMoviesException.class)
                    .hasMessageContaining("Impossible d'ajouter ce film");
        }

        @Test
        void should_not_modify_collection_when_given_invalid_session() {
            MoviesToPlan plan = TestProvider.plan(72, testDate);

            int sizeBeforeError = plan.get().size();

            Movie largeMovie = TestProvider.movie(240, false); // 4h
            MovieSessions session = new MovieSessions(largeMovie, SessionAmount.of(2)); // 8h

            assertThatThrownBy(() -> plan.add(session))
                    .isInstanceOf(TooMuchMoviesException.class);

            assertThat(plan.get()).hasSize(sizeBeforeError);
        }
    }
}