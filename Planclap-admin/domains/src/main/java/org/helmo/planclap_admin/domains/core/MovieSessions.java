package org.helmo.planclap_admin.domains.core;

import java.util.Objects;

/**
 * Représente un film avec un certain nombre de séances.
 */
public record MovieSessions(Movie movie, SessionAmount sessionsAmount) {

    /**
     * Calcule la durée totale des séances pour ce film.
     *
     * @return la durée totale des séances
     */
    public GlobalDuration getTotalDuration() {
        return movie.getDuration().multiplyBy(sessionsAmount.toInt());
    }

    /**
     * Définit l'égalité basée sur un slug.
     *
     * @param slug le slug à comparer
     * @return {@code true} si les slugs sont identiques, sinon {@code false}
     */
    public boolean equalsOnSlug(Name slug) {
        return Objects.equals(movie().getSlug(), slug);
    }

    @Override
    public boolean equals(Object o) {
        if (o == null || getClass() != o.getClass()) {
            return false;
        }
        MovieSessions that = (MovieSessions) o;
        return equalsOnSlug(that.movie().getSlug());
    }

    @Override
    public int hashCode() {
        return Objects.hash(movie().getSlug());
    }
}
