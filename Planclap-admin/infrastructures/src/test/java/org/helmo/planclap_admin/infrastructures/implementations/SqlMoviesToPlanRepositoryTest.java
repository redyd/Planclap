package org.helmo.planclap_admin.infrastructures.implementations;

import org.helmo.planclap_admin.domains.core.*;
import org.helmo.planclap_admin.domains.exceptions.RepositoryException;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.io.TempDir;

import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.time.LocalDate;
import java.util.List;
import java.util.Properties;

import static org.junit.jupiter.api.Assertions.*;

public class SqlMoviesToPlanRepositoryTest {

    public static final Properties PROPS = new Properties();
    public String jdbcUrl = "";
    private SqlMoviesToPlanRepository repo;

    @BeforeEach
    void setUp(@TempDir Path temp) throws IOException {
        var filePath = temp.resolve("planclap-test.sqlite");
        Files.copy(Path.of("src", "test", "resources", "db", "planclap-test.sqlite"), filePath);
        jdbcUrl = "jdbc:sqlite:%s".formatted(filePath);
        repo = new SqlMoviesToPlanRepository(jdbcUrl, PROPS);
    }

    @Test
    void should_fetch_every_movies_to_plan() throws RepositoryException {
        var targetDate = LocalDate.of(2025, 11, 24);
        var result = repo.getMoviesToPlan(targetDate);

        assertEquals(result.getDateForPlan(), targetDate);
        assertEquals(5, result.get().size());
    }

    @Test
    void should_add_a_full_movie_and_a_movie_to_plan_when_insert_movie_to_plan() throws RepositoryException {
        var targetDate = LocalDate.of(2025, 11, 24);

        int sizeBefore = repo.getMoviesToPlan(targetDate).get().size();

        var movie = new Movie(
                Name.of("Java my beloved"),
                MovieDescription.of("Ma super description"),
                MovieDuration.of(120),
                PosterURI.of("https://myposter.com"),
                CineCheckGroup.of(CineCheckAge.AL, List.of(CineCheck.RUDE, CineCheck.DRUGS))
        );
        var session = new MovieSessions(movie, SessionAmount.of(3));

        repo.addMovieToPlan(session, targetDate);

        int sizeAfter = repo.getMoviesToPlan(targetDate).get().size();

        assertEquals(sizeBefore + 1, sizeAfter);
    }

    @Test
    void should_throw_exception_if_the_movie_is_already_planned_and_exists() throws RepositoryException {
        var targetDate = LocalDate.of(2025, 11, 24);

        var movie = new Movie(
                Name.of("Vaiana 2"), // same slug as viana-2 -> should not be added
                MovieDescription.of("Ma super description"),
                MovieDuration.of(120),
                PosterURI.of("https://myposter.com"),
                CineCheckGroup.of(CineCheckAge.EIGHTEEN, List.of(CineCheck.RUDE, CineCheck.DRUGS)) // vaiana-2 has just AL check
        );

        var session = new MovieSessions(movie, SessionAmount.of(3));

        assertThrows(RepositoryException.class, () -> repo.addMovieToPlan(session, targetDate));

        var result = repo.getMoviesToPlan(targetDate);

        assertEquals(5, result.get().size()); // is not changed
    }

    @Test
    void should_not_overwrite_cinechecks_if_the_movie_is_already_planned() throws RepositoryException {
        var targetDate = LocalDate.of(2025, 11, 24);

        var movie = new Movie(
                Name.of("Vaiana 2"), // same slug as viana-2 -> should not be added
                MovieDescription.of("Ma super description"),
                MovieDuration.of(120),
                PosterURI.of("https://myposter.com"),
                CineCheckGroup.of(CineCheckAge.EIGHTEEN, List.of(CineCheck.RUDE, CineCheck.DRUGS)) // vaiana-2 has just AL check
        );

        var session = new MovieSessions(movie, SessionAmount.of(3));

        assertThrows(RepositoryException.class, () -> repo.addMovieToPlan(session, targetDate));

        var result = repo.getMoviesToPlan(targetDate);

        var notEdited = result.get().stream()
                .filter(mv -> mv.equalsOnSlug(Name.of("vaiana-2")))
                .findFirst()
                .orElseThrow();

        assertTrue(notEdited.movie().getCineChecksGroup().getCineCheck().isEmpty());
        assertEquals(CineCheckAge.AL, notEdited.movie().getCineChecksGroup().getAge());
    }

    @Test
    void should_find_if_the_movie_is_already_planned_and_exists() throws RepositoryException {
        var targetDate = LocalDate.of(2025, 11, 24);

        assertTrue(repo.isAlreadyPlanned(Name.of("vaiana-2"), targetDate));
        assertFalse(repo.isAlreadyPlanned(Name.of("hulk"), targetDate));
    }

    @Test
    void should_handle_when_different_week()  throws RepositoryException {
        var firstDate = LocalDate.of(2025, 11, 17);
        var secondDate = LocalDate.of(2025, 11, 24);

        assertFalse(repo.isAlreadyPlanned(Name.of("vice-versa-2"), firstDate));
        assertTrue(repo.isAlreadyPlanned(Name.of("vice-versa-2"), secondDate));
    }

    @Test
    void should_get_any_movie() throws RepositoryException {
        assertTrue(repo.getOnlyMovie(Name.of("vaiana-2")).isPresent()); // from week 1
        assertTrue(repo.getOnlyMovie(Name.of("vice-versa-2")).isPresent()); // from week 2
    }

}
