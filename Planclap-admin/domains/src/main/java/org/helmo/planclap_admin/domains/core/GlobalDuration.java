package org.helmo.planclap_admin.domains.core;

/**
 * Représente une durée globale en minutes.
 * Hérite de la classe Duration.
 * Utilisée pour représenter les sommes de durées dans l'application.
 */
public class GlobalDuration extends Duration {

    private GlobalDuration(int duration) {
        super(duration);
    }

    /**
     * Crée une durée globale à partir d'un nombre de minutes.
     *
     * @param minutes le nombre de minutes
     * @return une instance de GlobalDuration de minimum 0 minute
     */
    public static GlobalDuration of(int minutes) {
        return new GlobalDuration(Math.max(0, minutes));
    }

    /**
     * Crée une durée globale à partir d'un nombre d'heures.
     *
     * @param hours le nombre d'heures
     * @return une instance de GlobalDuration de minimum 0 heure
     */
    public static GlobalDuration ofHours(int hours) {
        return new GlobalDuration(Math.max(0, hours * 60));
    }

    /**
     * Ajoute une durée.
     *
     * @param duration la durée à ajouter
     * @return une nouvelle durée ajoutée de {@code duration}
     */
    public GlobalDuration plus(Duration duration) {
        return GlobalDuration.of(duration.get() + this.get());
    }

    @Override
    public String toString() {
        return "GlobalDuration[%s]".formatted(toHoursFormat());
    }
}
