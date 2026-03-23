package org.helmo.planclap_admin.domains.core;

import java.util.Objects;

/**
 * Représente le nombre de séances de film.
 */
public class SessionAmount {
    private final int getSessionAmount;

    private SessionAmount(int getSessionAmount) {
        this.getSessionAmount = getSessionAmount;
    }

    public int toInt() {
        return getSessionAmount;
    }

    /**
     * Crée une nouvelle quantité de film.
     *
     * @param sessionAmount la quantité
     * @return la quantité
     * @throws IllegalArgumentException si le nombre n'est pas compris entre 1 et 9
     */
    public static SessionAmount of(int sessionAmount) {
        if (sessionAmount < 1 || sessionAmount > 9) {
            throw new IllegalArgumentException("Le nombre de séances doit être compris entre 1 et 9.");
        }
        return new SessionAmount(sessionAmount);
    }

    @Override
    public boolean equals(Object o) {
        return (o instanceof SessionAmount that) && (getSessionAmount == that.getSessionAmount);
    }

    @Override
    public int hashCode() {
        return Objects.hashCode(getSessionAmount);
    }
}
