package org.helmo.planclap_admin.infrastructures.implementations;

import org.helmo.planclap_admin.domains.annotations.Log;
import org.helmo.planclap_admin.infrastructures.logged_operations.LoggedFileOperations;
import org.helmo.planclap_admin.domains.core.*;
import org.helmo.planclap_admin.domains.exceptions.RepositoryException;
import org.helmo.planclap_admin.domains.repository.MoviesToPlanRepository;
import org.helmo.planclap_admin.infrastructures.helper.JsonHandler;
import org.helmo.planclap_admin.infrastructures.mapper.MoviesToPlanMapper;

import java.io.IOException;
import java.nio.file.Path;
import java.time.LocalDate;
import java.util.Optional;

public class JsonMoviesToPlanRepository implements MoviesToPlanRepository {

    private final Path directory;
    private final JsonHandler handler;

    public JsonMoviesToPlanRepository(Path directory) {
        this.directory = directory;
        this.handler = new JsonHandler();
    }

    @Override
    @Log
    public MoviesToPlan getMoviesToPlan(LocalDate date) throws RepositoryException {
        handler.ensureDirectoryExists(directory, date);

        try (var reader = LoggedFileOperations.openForReading(handler.getFilePath(directory, date))) {
            return handler.readAndParse(reader, date);
        } catch (IOException e) {
            throw new RepositoryException("Erreur lors de la lecture du JSON: " + e.getMessage());
        }
    }

    @Override
    @Log
    public void addMovieToPlan(MovieSessions toAdd, LocalDate date) throws RepositoryException {
        handler.ensureDirectoryExists(directory, date);

        try (var reader = LoggedFileOperations.openForReading(handler.getFilePath(directory, date))) {
            MoviesToPlan plan = handler.readAndParse(reader, date);

            plan.add(toAdd);
            var dto = MoviesToPlanMapper.mapToDTO(plan);

            try (var writer = LoggedFileOperations.openForWriting(handler.getFilePath(directory, date))) {
                handler.write(writer, dto);
            }

        } catch (IOException e) {
            throw new RepositoryException("Erreur lors de la lecture du JSON: " + e.getMessage());
        }
    }

    @Override
    @Log
    public boolean isAlreadyPlanned(Name slug, LocalDate date) throws RepositoryException {
        handler.ensureDirectoryExists(directory, date);

        try (var reader = LoggedFileOperations.openForReading(handler.getFilePath(directory, date))) {
            MoviesToPlan plan = handler.readAndParse(reader, date);
            return plan.containsMovie(slug);
        } catch (IOException e) {
            throw new RepositoryException("Erreur lors de la lecture du JSON: " + e.getMessage());
        }
    }

    @Override
    public Optional<Movie> getOnlyMovie(Name slug) {
        return Optional.empty();
    }

}
