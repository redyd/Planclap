package org.helmo.planclap_admin.domains.core;

/**
 * Représente la durée d'un film en minutes.
 */
public class MovieDuration extends Duration {

    private MovieDuration(int duration) {
        super(duration);
    }

    /**
     * Multiplie la durée actuelle et retourne une nouvelle durée.
     *
     * @param n multiplicateur
     * @return une nouvelle durée si {@code n} est supérieur à 0, sinon 0
     */
    public GlobalDuration multiplyBy(int n) {
        return GlobalDuration.of(get() * n);
    }

    /**
     * Crée une durée de film comprise entre 1 et 240 minutes.
     *
     * @param minutes la durée en minutes du film, entre 1 et 240
     * @return un nouveau MovieDuration représentant une durée de film
     * @throws IllegalArgumentException si la création échoue
     */
    public static MovieDuration of(int minutes) {
        if (minutes <= 0 || minutes > 240) {
            throw new IllegalArgumentException("La duree du film doit etre comprise entre 1 et 240 minutes.");
        }
        return new MovieDuration(minutes);
    }
}
