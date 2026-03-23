package org.helmo.planclap_admin.infrastructures.implementations;

import org.helmo.planclap_admin.domains.core.PlanningTimeManager;
import org.helmo.planclap_admin.domains.core.WeekDay;
import org.helmo.planclap_admin.domains.core.Planning;
import org.helmo.planclap_admin.domains.exceptions.RepositoryException;
import org.helmo.planclap_admin.infrastructures.logged_operations.LoggedFileOperations;
import org.helmo.planclap_admin.infrastructures.mapper.MovieMapper;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Nested;
import org.junit.jupiter.api.Test;
import org.mockito.MockedStatic;
import org.mockito.Mockito;

import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.time.LocalDate;
import java.util.HashSet;
import java.util.List;
import java.util.Set;

import static org.helmo.planclap_admin.infrastructures.provider.TestProvider.movie;
import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.ArgumentMatchers.any;

/**
 * Classe de test généré en partie avec IA
 * Correction manuele
 */
class CsvPlanningRepositoryTest {

    Path tempDir;
    CsvPlanningRepository repository;
    LocalDate date;

    @BeforeEach
    void setUp() throws Exception {
        tempDir = Files.createTempDirectory("csv-planning-test");
        repository = new CsvPlanningRepository(tempDir);
        date = LocalDate.of(2025, 10, 20);
    }

    @Nested
    @DisplayName("Happy path")
    class HappyPath {

        @Test
        void should_overwrite_file_when_already_exists() throws Exception {
            Path filePath = tempDir.resolve(date + ".csv");

            Files.writeString(filePath, "OLD_CONTENT\n");

            Planning planning = new Planning(new PlanningTimeManager(date));
            planning.add(WeekDay.MONDAY, movie(100, true));

            repository.encode(planning, date);

            // Assert
            String content = Files.readString(filePath);
            assertTrue(content.startsWith("date,startTime,endTime,slug"),
                    "Le fichier doit commencer par le header CSV");
            assertTrue(content.contains("movie"),
                    "Le nouveau contenu doit être écrit");
            assertFalse(content.contains("OLD_CONTENT"),
                    "L'ancien contenu ne doit plus être présent — le fichier doit être écrasé");
        }

        @Test
        void should_create_file_if_not_exists() throws Exception {
            // Arrange
            Planning planning = new Planning(new PlanningTimeManager(date));
            planning.add(WeekDay.MONDAY, movie(100, true));

            repository.encode(planning, date);

            // Assert
            Path filePath = tempDir.resolve(date + ".csv");

            assertTrue(Files.exists(filePath), "Le fichier CSV doit être créé s'il n'existe pas");
            String content = Files.readString(filePath);

            assertTrue(content.contains("movie"), "Le contenu du film doit être écrit dans le fichier");
        }

        @Test
        void should_create_csv_file_on_encode() throws RepositoryException {
            Planning planning = new Planning(new PlanningTimeManager(date));

            planning.add(WeekDay.MONDAY, MovieMapper.from("movie", 120, "url.com", "description", new HashSet<>(Set.of("AL"))));

            repository.encode(planning, date);

            Path file = tempDir.resolve(date + ".csv");
            assertTrue(Files.exists(file));
        }

        @Test
        void should_write_header_in_csv_file() throws Exception {
            Planning planning = new Planning(new PlanningTimeManager(date));

            repository.encode(planning, date);

            Path file = tempDir.resolve(date + ".csv");
            List<String> lines = Files.readAllLines(file);

            assertFalse(lines.isEmpty());
            assertEquals("date,startTime,endTime,slug", lines.getFirst());
        }

        @Test
        void should_write_planned_movies_in_csv_file() throws Exception {
            Planning planning = new Planning(new PlanningTimeManager(date));

            planning.add(WeekDay.MONDAY, MovieMapper.from("test-movie", 60, "url.com", "description", new HashSet<>(Set.of("AL"))));

            repository.encode(planning, date);

            Path file = tempDir.resolve(date + ".csv");
            List<String> lines = Files.readAllLines(file);

            assertEquals(2, lines.size()); // header + 1 movie
            assertTrue(lines.get(1).contains("test-movie"));
            assertTrue(lines.get(1).contains("12:00"));
            assertTrue(lines.get(1).contains("13:00"));
        }

    }

    @Nested
    @DisplayName("Error cases")
    class ErrorCases {

        @Test
        void should_throw_repository_exception_when_io_error_occurs() {
            CsvPlanningRepository repo = new CsvPlanningRepository(tempDir);
            Planning planning = new Planning(new PlanningTimeManager(date));

            try (MockedStatic<LoggedFileOperations> mocked =
                         Mockito.mockStatic(LoggedFileOperations.class)) {

                mocked.when(() ->
                        LoggedFileOperations.openForWriting(any(Path.class))
                ).thenThrow(new IOException("Disk error"));

                assertThrows(RepositoryException.class,
                        () -> repo.encode(planning, date));
            }
        }

        @Test
        void should_throw_repository_exception_on_invalid_write() {
            CsvPlanningRepository repo = new CsvPlanningRepository(Path.of("/invalid-directory"));
            Planning planning = new Planning(new PlanningTimeManager(date));

            assertThrows(RepositoryException.class, () -> repo.encode(planning, date));
        }

    }
}
