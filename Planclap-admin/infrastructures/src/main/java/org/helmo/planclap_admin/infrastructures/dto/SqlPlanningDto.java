package org.helmo.planclap_admin.infrastructures.dto;

import java.util.List;

public record SqlPlanningDto(List<SqlPlannedMovieDto> plannedMovies) {
}
