package org.helmo.planclap_admin.infrastructures.dto;

public record MovieSessionsDto(String slug, String title, int duration, String poster, String description,
                               String[] cinechecks, int seances) {
}
