package org.helmo.planclap_admin.infrastructures.helper;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import org.helmo.planclap_admin.domains.core.MoviesToPlan;
import org.helmo.planclap_admin.domains.exceptions.RepositoryException;
import org.helmo.planclap_admin.infrastructures.dto.MoviesToPlanDto;
import org.helmo.planclap_admin.infrastructures.mapper.MoviesToPlanMapper;

import java.io.IOException;
import java.io.Reader;
import java.io.Writer;
import java.nio.file.Files;
import java.nio.file.Path;
import java.time.LocalDate;

public class JsonHandler {

    private final Gson gson = new GsonBuilder().setPrettyPrinting().create();

    public MoviesToPlan readAndParse(Reader reader, LocalDate date) {
        MoviesToPlanDto dto = gson.fromJson(reader, MoviesToPlanDto.class);
        MoviesToPlanDto verified = dto != null ? dto : MoviesToPlanDto.empty();
        return MoviesToPlanMapper.mapDTO(date, verified);
    }

    public MoviesToPlanDto read(Reader reader) {
        MoviesToPlanDto dto = gson.fromJson(reader, MoviesToPlanDto.class);
        return dto != null ? dto : MoviesToPlanDto.empty();
    }

    public void write(Writer writer, MoviesToPlanDto dto) {
        gson.toJson(dto, writer);
    }

    public void ensureDirectoryExists(Path directory, LocalDate date) throws RepositoryException {
        Path file = getFilePath(directory, date);
        try {
            if (!file.toFile().exists()) {
                Files.createFile(file);
            }
        } catch (IOException e) {
            throw new RepositoryException("Could not create JSON file: " + e.getMessage());
        }
    }

    public Path getFilePath(Path directory, LocalDate date) {
        return directory.resolve(date + ".json");
    }

}
