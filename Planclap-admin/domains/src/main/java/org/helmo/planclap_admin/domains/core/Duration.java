package org.helmo.planclap_admin.domains.core;

import java.util.Objects;

/**
 * Représente une durée en minutes.
 */
public abstract class Duration {
    private final int minutes;

    public Duration(int minutes) {
        this.minutes = minutes;
    }

    /**
     * Retourne la durée en minutes.
     *
     * @return la durée en minutes
     */
    public int get() {
        return minutes;
    }

    /**
     * Convertis la durée en heure.
     *
     * @return un double représentant la durée
     */
    public double getHours() {
        return minutes / 60.d;
    }

    /**
     * Converti la durée à un format HH:mm
     *
     * @return la durée au format HH:mm
     */
    public String toHoursFormat() {
        int hours = this.minutes / 60;
        int minutes = this.minutes % 60;

        return String.format("%02d h %02d", hours, minutes);
    }

    /**
     * Convertis la durée en heure.
     *
     * @return un double représentant la durée
     */
    public double toHours() {
        return minutes / 60.d;
    }

    @Override
    public boolean equals(Object o) {
        return (o instanceof Duration duration1) && (minutes == duration1.minutes);
    }

    @Override
    public int hashCode() {
        return Objects.hashCode(minutes);
    }

}
