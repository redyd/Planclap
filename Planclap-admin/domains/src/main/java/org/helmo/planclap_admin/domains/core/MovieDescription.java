package org.helmo.planclap_admin.domains.core;

import java.util.Objects;

/**
 * Classe de description de film (entre 1 et 140 caractères)
 */
public class MovieDescription {
    private final String description;

    private MovieDescription(String description) {
        this.description = description;
    }

    /**
     * Crée un MovieDescription.
     *
     * @param description description entre 1 et 240 caractères
     * @return un MovieDescription
     * @throws IllegalArgumentException si la description est invalide
     */
    public static MovieDescription of(String description) {
        if (description == null || description.isEmpty()  || description.length() > 200) {
            throw new IllegalArgumentException("La description d'un film doit contenir entre 1 et 200 caracteres.");
        }
        return new MovieDescription(description);
    }

    @Override
    public boolean equals(Object o) {
        if (o == null || getClass() != o.getClass()) {
            return false;
        }
        MovieDescription that = (MovieDescription) o;
        return description.equals(that.description);
    }

    @Override
    public int hashCode() {
        return Objects.hashCode(description);
    }

    @Override
    public String toString() {
        return description;
    }
}
