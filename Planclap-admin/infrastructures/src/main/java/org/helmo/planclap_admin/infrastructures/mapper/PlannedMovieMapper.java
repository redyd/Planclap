package org.helmo.planclap_admin.infrastructures.mapper;

import org.helmo.planclap_admin.domains.core.PlannedMovie;
import org.helmo.planclap_admin.infrastructures.dto.CsvPlannedMovieDto;
import org.helmo.planclap_admin.infrastructures.dto.SqlPlannedMovieDto;

import java.time.ZoneOffset;
import java.time.format.DateTimeFormatter;

public class PlannedMovieMapper {

    private PlannedMovieMapper() {
    }

    public static CsvPlannedMovieDto mapToCsvDto(PlannedMovie plannedMovie) {
        DateTimeFormatter formatter = DateTimeFormatter.ofPattern("HH:mm");
        return new CsvPlannedMovieDto(
                plannedMovie.getDate().toString(),
                plannedMovie.getStartTime().format(formatter),
                plannedMovie.getEndTime().format(formatter),
                plannedMovie.getSlug().toMinimal().toString()
        );
    }

    public static SqlPlannedMovieDto mapToSqlDto(PlannedMovie plannedMovie) {
        return new SqlPlannedMovieDto(plannedMovie.getSlug().toMinimal().toString(), plannedMovie.getStartTime().toEpochSecond(ZoneOffset.UTC));
    }

}
