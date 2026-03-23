package org.helmo.planclap_admin.infrastructures.dto;

import java.util.HashSet;
import java.util.Set;

public record MoviesToPlanDto(Set<MovieSessionsDto> movies) {
    public static MoviesToPlanDto empty() {
        return new MoviesToPlanDto(new HashSet<>());
    }
}
