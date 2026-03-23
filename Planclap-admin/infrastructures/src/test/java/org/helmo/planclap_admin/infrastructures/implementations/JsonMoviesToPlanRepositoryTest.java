package org.helmo.planclap_admin.infrastructures.implementations;

import org.helmo.planclap_admin.domains.core.GlobalDuration;
import org.helmo.planclap_admin.domains.core.MovieSessions;
import org.helmo.planclap_admin.domains.core.Name;
import org.helmo.planclap_admin.domains.core.SessionAmount;
import org.helmo.planclap_admin.domains.exceptions.RepositoryException;
import org.helmo.planclap_admin.domains.exceptions.TooMuchMoviesException;
import org.helmo.planclap_admin.domains.repository.MoviesToPlanRepository;
import org.helmo.planclap_admin.infrastructures.dto.MovieSessionsDto;
import org.helmo.planclap_admin.infrastructures.mapper.MovieSessionsMapper;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Nested;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.io.TempDir;

import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.time.LocalDate;

import static org.helmo.planclap_admin.infrastructures.provider.TestProvider.movie;
import static org.junit.jupiter.api.Assertions.*;

class JsonMoviesToPlanRepositoryTest {

    MoviesToPlanRepository repo;
    Path path;
    LocalDate date;

    @Nested
    @DisplayName("Happy path")
    class HappyPath {

        @BeforeEach
        void setUp(@TempDir Path tempDir) throws IOException {
            path = tempDir.resolve(LocalDate.now() + ".json");
            Path data = Path.of("src", "test", "resources", "files", "movies.json");
            Files.copy(data, path);
            date = LocalDate.now();

            repo = new JsonMoviesToPlanRepository(tempDir);
        }

        @Test
        void should_get_every_movies_when_get_all() throws RepositoryException {
            var movies = repo.getMoviesToPlan(date);

            assertEquals(9, movies.get().size());
            assertTrue(movies.getTotalDuration().getHours() <= 77);
        }

        @Test
        void should_add_movie_when_not_exist() throws RepositoryException {
            var movieSessions = MovieSessionsMapper.mapDTO(new MovieSessionsDto(
                    "new-movieSessions", "New Movie", 60, "poster", "description", new String[]{"AL"}, 3));

            var movie = movieSessions.movie();

            assertDoesNotThrow(() -> repo.addMovieToPlan(movieSessions, date));
            var movies = repo.getMoviesToPlan(date);
            assertTrue(movies.containsMovie(movie.getSlug()));
        }

        @Test
        void should_find_existing_movie() throws RepositoryException {
            var exists = repo.isAlreadyPlanned(Name.of("wicked"), date);
            assertTrue(exists);
        }

    }

    @Nested
    @DisplayName("Error cases")
    class ErrorCases {

        @TempDir
        Path tempDir;

        void setUp(String fileName) {
            try {
                path = tempDir.resolve(LocalDate.now() + ".json");
                Path data = Path.of("src", "test", "resources", "files", fileName + ".json");
                Files.copy(data, path);
                date = LocalDate.now();

                repo = new JsonMoviesToPlanRepository(tempDir);
            } catch (IOException ignored) {
            }
        }

        @Test
        void should_create_file_if_not_found() throws RepositoryException {
            var json = new JsonMoviesToPlanRepository(tempDir);
            var plan = json.getMoviesToPlan(LocalDate.now());

            assertEquals(GlobalDuration.of(0), plan.getTotalDuration());
        }

        @Test
        void should_not_add_movie_when_slug_already_exist() {
            setUp("movies");

            var movie = MovieSessionsMapper.mapDTO(new MovieSessionsDto(
                    "wicked", "Wicked", 60, "poster", "description", new String[]{"AL"}, 2));

            assertThrows(TooMuchMoviesException.class, () -> repo.addMovieToPlan(movie, date));
        }

        @Test
        void should_not_add_movie_when_exceed_plan() {
            setUp("movies");

            var movie = MovieSessionsMapper.mapDTO(new MovieSessionsDto(
                    "wicked-2", "Wicked 2", 200, "poster", "description", new String[]{"AL"}, 8));

            assertThrows(TooMuchMoviesException.class, () -> repo.addMovieToPlan(movie, date));
        }

        @Test
        void should_return_empty_plan_and_overwrite_when_invalid_data() throws RepositoryException, IOException {
            setUp("movies-with-invalid");
            var movie = repo.getMoviesToPlan(date); // essaie de récupérer les plans avec un film de 166666 minutes
            long lineCountBefore = countLines(path);

            assertEquals(0, movie.getTotalDuration().get());

            repo.addMovieToPlan(new MovieSessions(movie(120, true), SessionAmount.of(2)), date);

            movie = repo.getMoviesToPlan(date);

            assertEquals(240, movie.getTotalDuration().get());

            assertNotEquals(lineCountBefore, countLines(path));
        }

        @Test
        void should_return_empty_plan_and_overwrite_when_plan_exceed() throws RepositoryException, IOException {
            setUp("movies-exceed");
            var movie = repo.getMoviesToPlan(date); // essaie de récupérer les plans avec un film de 166666 minutes
            long lineCountBefore = countLines(path);

            assertEquals(0, movie.getTotalDuration().get());

            repo.addMovieToPlan(new MovieSessions(movie(120, true), SessionAmount.of(2)), date);

            movie = repo.getMoviesToPlan(date);

            assertEquals(240, movie.getTotalDuration().get());

            assertNotEquals(lineCountBefore, countLines(path));
        }
    }

    public static long countLines(Path filePath) throws IOException {
        try (var lines = Files.lines(filePath)) {
            return lines.count();
        }
    }

}
