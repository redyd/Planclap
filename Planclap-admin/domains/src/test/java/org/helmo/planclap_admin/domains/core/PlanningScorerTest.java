package org.helmo.planclap_admin.domains.core;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;

import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.List;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.Mockito.*;

class PlanningScorerTest {

    private PlanningScorer scorer;
    private Name slugMock1;
    private Name slugMock2;
    private Name slugMock3;

    @BeforeEach
    void setUp() {
        scorer = new PlanningScorer();
        slugMock1 = mock(Name.class);
        slugMock2 = mock(Name.class);
        slugMock3 = mock(Name.class);
    }

    @Test
    void should_return_positive_score_when_children_movies_before_breakpoint() {
        LocalDateTime breakPoint = LocalDateTime.of(2025, 10, 26, 18, 0);
        LocalDateTime startTime = LocalDateTime.of(2025, 10, 26, 14, 0);
        LocalDateTime endTime = LocalDateTime.of(2025, 10, 26, 16, 0);

        PlannedMovie childMovie = new PlannedMovie(startTime, endTime, slugMock1, true);
        List<PlannedMovie> movies = List.of(childMovie);

        int score = scorer.scoreForAge(movies, breakPoint);

        assertEquals(100, score);
    }

    @Test
    void should_return_negative_score_when_children_movies_after_breakpoint() {
        LocalDateTime breakPoint = LocalDateTime.of(2025, 10, 26, 18, 0);
        LocalDateTime startTime = LocalDateTime.of(2025, 10, 26, 20, 0);
        LocalDateTime endTime = LocalDateTime.of(2025, 10, 26, 22, 0);

        PlannedMovie childMovie = new PlannedMovie(startTime, endTime, slugMock1, true);
        List<PlannedMovie> movies = List.of(childMovie);

        int score = scorer.scoreForAge(movies, breakPoint);

        assertEquals(-100, score);
    }

    @Test
    void should_return_positive_score_when_adult_movies_after_breakpoint() {
        LocalDateTime breakPoint = LocalDateTime.of(2025, 10, 26, 18, 0);
        LocalDateTime startTime = LocalDateTime.of(2025, 10, 26, 20, 0);
        LocalDateTime endTime = LocalDateTime.of(2025, 10, 26, 22, 0);

        PlannedMovie adultMovie = new PlannedMovie(startTime, endTime, slugMock1, false);
        List<PlannedMovie> movies = List.of(adultMovie);

        int score = scorer.scoreForAge(movies, breakPoint);

        assertEquals(100, score);
    }

    @Test
    void should_return_negative_score_when_adult_movies_before_breakpoint() {
        LocalDateTime breakPoint = LocalDateTime.of(2025, 10, 26, 18, 0);
        LocalDateTime startTime = LocalDateTime.of(2025, 10, 26, 14, 0);
        LocalDateTime endTime = LocalDateTime.of(2025, 10, 26, 16, 0);

        PlannedMovie adultMovie = new PlannedMovie(startTime, endTime, slugMock1, false);
        List<PlannedMovie> movies = List.of(adultMovie);

        int score = scorer.scoreForAge(movies, breakPoint);

        assertEquals(-100, score);
    }

    @Test
    void should_calculate_correct_total_score_when_given_multiple_movies() {
        LocalDateTime breakPoint = LocalDateTime.of(2025, 10, 26, 18, 0);

        PlannedMovie childMovieGood = new PlannedMovie(
                LocalDateTime.of(2025, 10, 26, 14, 0),
                LocalDateTime.of(2025, 10, 26, 16, 0),
                slugMock1, true);

        PlannedMovie adultMovieBad = new PlannedMovie(
                LocalDateTime.of(2025, 10, 26, 16, 0),
                LocalDateTime.of(2025, 10, 26, 18, 0),
                slugMock2, false);

        PlannedMovie adultMovieGood = new PlannedMovie(
                LocalDateTime.of(2025, 10, 26, 20, 0),
                LocalDateTime.of(2025, 10, 26, 22, 0),
                slugMock3, false);

        List<PlannedMovie> movies = List.of(childMovieGood, adultMovieBad, adultMovieGood);

        int score = scorer.scoreForAge(movies, breakPoint);

        assertEquals(100, score);
    }

    @Test
    void should_return_positive_score_when_last_movie_before_end_of_day() {
        LocalDateTime endOfDay = LocalDateTime.of(2025, 10, 26, 23, 0);
        LocalDateTime startTime = LocalDateTime.of(2025, 10, 26, 20, 0);
        LocalDateTime endTime = LocalDateTime.of(2025, 10, 26, 22, 0);

        PlannedMovie movie = new PlannedMovie(startTime, endTime, slugMock1, false);
        List<PlannedMovie> movies = List.of(movie);

        int score = scorer.scoreForExceed(movies, endOfDay);

        assertEquals(45, score);
    }

    @Test
    void should_return_negative_score_when_last_movie_after_end_of_day() {
        LocalDateTime endOfDay = LocalDateTime.of(2025, 10, 26, 22, 0);
        LocalDateTime startTime = LocalDateTime.of(2025, 10, 26, 22, 30);
        LocalDateTime endTime = LocalDateTime.of(2025, 10, 27, 0, 30);

        PlannedMovie movie = new PlannedMovie(startTime, endTime, slugMock1, false);
        List<PlannedMovie> movies = List.of(movie);

        int score = scorer.scoreForExceed(movies, endOfDay);

        assertEquals(-45, score);
    }

    @Test
    void should_return_zero_when_given_empty_list_for_exceed() {
        LocalDateTime endOfDay = LocalDateTime.of(2025, 10, 26, 23, 0);
        List<PlannedMovie> movies = new ArrayList<>();

        int score = scorer.scoreForExceed(movies, endOfDay);

        assertEquals(0, score);
    }

    @Test
    void should_return_positive_score_when_all_different_movies() {
        PlannedMovie movie1 = new PlannedMovie(
                LocalDateTime.of(2025, 10, 26, 14, 0),
                LocalDateTime.of(2025, 10, 26, 16, 0),
                slugMock1, true);

        PlannedMovie movie2 = new PlannedMovie(
                LocalDateTime.of(2025, 10, 26, 16, 0),
                LocalDateTime.of(2025, 10, 26, 18, 0),
                slugMock2, false);

        PlannedMovie movie3 = new PlannedMovie(
                LocalDateTime.of(2025, 10, 26, 18, 0),
                LocalDateTime.of(2025, 10, 26, 20, 0),
                slugMock3, true);

        List<PlannedMovie> movies = List.of(movie1, movie2, movie3);

        int score = scorer.scoreForMultiple(movies);

        assertEquals(75, score);
    }

    @Test
    void should_return_negative_score_when_duplicate_movies_above_tolerance() {
        PlannedMovie movie1 = new PlannedMovie(
                LocalDateTime.of(2025, 10, 26, 14, 0),
                LocalDateTime.of(2025, 10, 26, 16, 0),
                slugMock1, true);

        PlannedMovie movie2 = new PlannedMovie(
                LocalDateTime.of(2025, 10, 26, 16, 0),
                LocalDateTime.of(2025, 10, 26, 18, 0),
                slugMock1, true);

        PlannedMovie movie3 = new PlannedMovie(
                LocalDateTime.of(2025, 10, 26, 18, 0),
                LocalDateTime.of(2025, 10, 26, 20, 0),
                slugMock1, true);

        List<PlannedMovie> movies = List.of(movie1, movie2, movie3);

        int score = scorer.scoreForMultiple(movies);

        assertTrue(score < 0);
    }

    @Test
    void should_calculate_correct_score_when_given_mixed_duplicates() {
        PlannedMovie movie1 = new PlannedMovie(
                LocalDateTime.of(2025, 10, 26, 14, 0),
                LocalDateTime.of(2025, 10, 26, 16, 0),
                slugMock1, true);

        PlannedMovie movie2 = new PlannedMovie(
                LocalDateTime.of(2025, 10, 26, 16, 0),
                LocalDateTime.of(2025, 10, 26, 18, 0),
                slugMock1, true);

        PlannedMovie movie3 = new PlannedMovie(
                LocalDateTime.of(2025, 10, 26, 18, 0),
                LocalDateTime.of(2025, 10, 26, 20, 0),
                slugMock2, false);

        List<PlannedMovie> movies = List.of(movie1, movie2, movie3);

        int score = scorer.scoreForMultiple(movies);

        assertEquals(0, score);
    }

    @Test
    void should_return_zero_when_given_empty_list_for_multiple() {
        List<PlannedMovie> movies = new ArrayList<>();

        int score = scorer.scoreForMultiple(movies);

        assertEquals(0, score);
    }
}