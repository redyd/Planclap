package org.helmo.planclap_admin.domains.core;

import java.time.LocalDate;
import java.time.LocalDateTime;
import java.time.ZoneOffset;
import java.util.Objects;

/**
 * Représente un film planifié à une date et une heure donnée
 */
public final class PlannedMovie {

    private LocalDateTime start;
    private LocalDateTime end;
    private final Name slug;
    private final boolean isChildMovie;

    public PlannedMovie(LocalDateTime start, LocalDateTime end, Name slug, boolean isChildMovie) {
        if (start.isAfter(end)) {
            throw new IllegalArgumentException("Impossible de definir une heure de debut apres l'heure de fin");
        }

        this.start = start;
        this.end = end;
        this.slug = slug;
        this.isChildMovie = isChildMovie;
    }

    public LocalDate getDate() {
        return start.toLocalDate();
    }

    public long getEpochOfStart() {
        return start.toEpochSecond(ZoneOffset.UTC);
    }

    public long getEpochOfEnd() {
        return end.toEpochSecond(ZoneOffset.UTC);
    }

    public LocalDateTime getStartTime() {
        return start;
    }

    public LocalDateTime getEndTime() {
        return end;
    }

    public Name getSlug() {
        return slug;
    }

    public boolean isForChildren() {
        return isChildMovie;
    }

    /**
     * Crée une copie de l'objet courant.
     *
     * @return Une nouvelle instance de PlannedMovie avec les mêmes valeurs.
     */
    public PlannedMovie copyOf() {
        return new PlannedMovie(start, end, slug, isChildMovie);
    }

    /**
     * Modifie l'heure de début et de fin du film planifié.
     *
     * @param startTime heure de début
     */
    public void setStart(LocalDateTime startTime) {
        long duration = getDuration();
        LocalDateTime endTime = startTime.plusMinutes(duration);

        this.start = startTime;
        this.end = endTime;
    }

    /**
     * Récupère en minutes la durée du film.
     *
     * @return la durée du film en minute
     */
    public long getDuration() {
        return java.time.Duration.between(start, end).toMinutes();
    }

    @Override
    public String toString() {
        return "PlannedMovie{" +
                "start=" + start +
                ", end=" + end +
                ", slug=" + slug +
                ", isChildMovie=" + isChildMovie +
                '}';
    }

    @Override
    public boolean equals(Object o) {
        if (!(o instanceof PlannedMovie that)) {
            return false;
        }
        return isChildMovie == that.isChildMovie
                && Objects.equals(start, that.start)
                && Objects.equals(end, that.end)
                && Objects.equals(slug, that.slug);
    }

    @Override
    public int hashCode() {
        return Objects.hash(start, end, slug, isChildMovie);
    }
}
