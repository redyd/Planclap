package org.helmo.planclap_admin.infrastructures.dto;

public record MovieDto(String title, String description, int duration, String poster, String[] cineChecks) {
}
