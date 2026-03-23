package org.helmo.planclap_admin.infrastructures.implementations;

import org.helmo.planclap_admin.domains.annotations.Log;
import org.helmo.planclap_admin.infrastructures.logged_operations.LoggedFileOperations;
import org.helmo.planclap_admin.domains.core.Planning;
import org.helmo.planclap_admin.domains.exceptions.RepositoryException;
import org.helmo.planclap_admin.domains.repository.PlanningRepository;
import org.helmo.planclap_admin.infrastructures.dto.CsvPlannedMovieDto;
import org.helmo.planclap_admin.infrastructures.dto.CsvPlanningDto;
import org.helmo.planclap_admin.infrastructures.mapper.PlanningMapper;

import java.io.BufferedWriter;
import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.time.LocalDate;

/**
 * Classe de test générée en partie par IA
 * Correction manuelle
 */
public class CsvPlanningRepository implements PlanningRepository {

    private final Path directory;
    private final static String HEADER = "date,startTime,endTime,slug\n";

    public CsvPlanningRepository(Path directory) {
        this.directory = directory;
    }

    @Override
    @Log
    public void encode(Planning planning, LocalDate date) throws RepositoryException {
        CsvPlanningDto dto = PlanningMapper.mapToCsvDto(planning);
        try (BufferedWriter writer = LoggedFileOperations.openForWriting(resetPath(date))) {
            writer.write(HEADER);

            for (CsvPlannedMovieDto plannedMovie : dto.plannedMovies()) {
                writer.write(dtoToCSVLine(plannedMovie));
            }
        } catch (IOException e) {
            throw new RepositoryException("Error while writing CSV: " + e.getMessage());
        }
    }

    private String dtoToCSVLine(CsvPlannedMovieDto dto) {
        return String.format("%s,%s,%s,%s\n",
                dto.startDate(),
                dto.startTime(),
                dto.endTime(),
                dto.slug());
    }

    private Path resetPath(LocalDate date) throws IOException {
        Path path = directory.resolve(date + ".csv");
        if (Files.exists(path)) {
            Files.delete(path);
        }
        return directory.resolve(date + ".csv");
    }
}
