package org.helmo.planclap_admin.domains.core;

/**
 * Enumération représentant les jours de la semaine avec une valeur entière associée.
 * Lundi est représenté par 0, mardi par 1, et ainsi de suite jusqu'à dimanche qui est représenté par 6.
 */
public enum WeekDay {
    MONDAY(0),
    TUESDAY(1),
    WEDNESDAY(2),
    THURSDAY(3),
    FRIDAY(4),
    SATURDAY(5),
    SUNDAY(6);

    private final int value;

    WeekDay(int value) {
        this.value = value;
    }

    /**
     * Récupère la valeur entière associée au jour de la semaine.
     *
     * @return la valeur entière du jour de la semaine
     */
    public int getDayPosition() {
        return value;
    }

    /**
     * Récupère le jour de la semaine correspondant à une valeur entière.
     *
     * @param value la valeur entière du jour de la semaine (0 pour lundi, 1 pour mardi, ..., 6 pour dimanche)
     * @return le jour de la semaine correspondant
     * @throws IllegalArgumentException si la valeur n'est pas comprise entre 0 et 6
     */
    public static WeekDay from(int value) {
        if (value < 0 || value > 6) {
            throw new IllegalArgumentException("Valeur de jour invalide: " + value);
        }
        return WeekDay.values()[value];
    }
}
