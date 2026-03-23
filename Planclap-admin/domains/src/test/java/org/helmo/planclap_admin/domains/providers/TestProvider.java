package org.helmo.planclap_admin.domains.providers;

import org.helmo.planclap_admin.domains.core.*;

import java.time.LocalDate;
import java.util.*;

import static org.mockito.Mockito.mock;
import static org.mockito.Mockito.when;

public class TestProvider {

    private final static Random R = new Random();
    private final static int MIN_DURATION = 60;
    private final static int MAX_DURATION = 200;

    public static Set<MovieSessions> mockedSessions(List<String> names) {
        Set<MovieSessions> sessions = new HashSet<>();

        for (String name : names) {
            Movie movie = mock(Movie.class);
            SessionAmount sessionAmount = mock(SessionAmount.class);
            MovieSessions mock = new MovieSessions(movie, sessionAmount);
            when(movie.getSlug()).thenReturn(Name.of(name));
            sessions.add(mock);
        }

        return sessions;
    }

    public static MoviesToPlan plan(int hours, LocalDate date) {
        var plan = new MoviesToPlan(date);
        int remaining = hours * 60;

        for (int i = 1; i <= MAX_DURATION; i++) {
            if (remaining <= 200) {
                break;
            }
            var toAdd = movie();
            plan.add(new MovieSessions(toAdd, SessionAmount.of(1)));
            remaining -= toAdd.getDuration().get();
        }

        plan.add(new MovieSessions(movie(remaining, false), SessionAmount.of(1)));

        return plan;
    }

    public static Movie movie(int duration, boolean forChildren) {
        CineCheckAge age = forChildren ? CineCheckAge.AL : CineCheckAge.EIGHTEEN;

        return new Movie(
                Name.of("Movie%d".formatted(R.nextInt())),
                MovieDescription.of("Description%d".formatted(R.nextInt())),
                MovieDuration.of(duration),
                PosterURI.of("www.poster.com/%d/png".formatted(R.nextInt())),
                CineCheckGroup.of(age, List.of())
        );
    }

    public static Movie movie() {
        return movie(R.nextInt(MIN_DURATION, MAX_DURATION), R.nextBoolean());
    }

    public static List<Movie> movies(int size) {
        List<Movie> movies = new ArrayList<>(size);

        for (int i = 0; i < size; i++) {
            movies.add(movie(R.nextInt(MIN_DURATION, MAX_DURATION), R.nextBoolean()));
        }

        return movies;
    }

}
