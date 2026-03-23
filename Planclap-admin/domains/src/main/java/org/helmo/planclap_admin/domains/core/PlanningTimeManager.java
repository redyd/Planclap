package org.helmo.planclap_admin.domains.core;

import java.time.DayOfWeek;
import java.time.LocalDate;
import java.time.LocalDateTime;
import java.time.LocalTime;
import java.util.List;

/**
 * Gère les opérations temporelles pour le planning
 */
public final class PlanningTimeManager {

    private static final LocalTime BREAKPOINT = LocalTime.of(16, 30);
    private static final LocalTime START_OF_THE_DAY = LocalTime.of(12, 0);
    private static final LocalTime END_OF_THE_DAY = LocalTime.of(23, 0);

    private final LocalDate baseDate;

    public PlanningTimeManager(LocalDate baseDate) {
        if (baseDate.getDayOfWeek() != DayOfWeek.MONDAY) {
            throw new IllegalArgumentException("La date doit être un lundi");
        }
        this.baseDate = baseDate;
    }

    /**
     * La date du lundi du planning.
     *
     * @return la date du lundi de la semaine
     */
    public LocalDate getBaseDate() {
        return baseDate;
    }

    /**
     * Retourne l'heure de séparation entre films enfants/adultes.
     *
     * @return l'heure à laquelle on devrait planifier des films pour adultes
     */
    public LocalDateTime getBreakpoint(WeekDay day) {
        return LocalDateTime.of(baseDate.plusDays(day.getDayPosition()), BREAKPOINT);
    }

    /**
     * Calcule le LocalDateTime du début d'une journée.
     *
     * @param day la journée concernée
     * @return le LocalDateTime du début de la journée théorique
     */
    public LocalDateTime startOfDay(WeekDay day) {
        return LocalDateTime.of(baseDate.plusDays(day.getDayPosition()), START_OF_THE_DAY);
    }

    /**
     * Calcule le LocalDateTime du début d'une journée.
     *
     * @param day la journée concernée
     * @return le LocalDateTime de la fin de la journée théorique
     */
    public LocalDateTime endOfDay(WeekDay day) {
        return LocalDateTime.of(baseDate.plusDays(day.getDayPosition()), END_OF_THE_DAY);
    }

    /**
     * Permet de calculer le quart d'heure suivant.
     * Si l'heure passée est une heure "quart", alors le quart d'heure suivant sera retourné
     *
     * @param time le temps à passer au quart d'heure suivant
     * @return le temps {@code time} au quart d'heure suivant
     */
    public LocalDateTime toNextQuarter(LocalDateTime time) {
        int minutesToAdd = Math.abs((time.getMinute() % 15) - 15);
        return time.plusMinutes(minutesToAdd);
    }

    /**
     * Détermine si une heure dépasse de la journée.
     *
     * @param time la durée à vérifier
     * @param day la journée
     * @return {@code true} si la durée dépasse des limites imposées, sinon {@code false}
     */
    public boolean doesExceed(LocalDateTime time, WeekDay day) {
        return !time.isBefore(endOfDay(day));
    }

    /**
     * Récupère la fin de séance du dernier film de la journée.
     *
     * @param movies une liste de films ordonnés dans l'ordre logique de la journée
     * @return le LocalDateTime de la fin de la séance de la dernière séance
     */
    public LocalDateTime getLastEndOfDay(List<PlannedMovie> movies) {
        return movies.getLast().getEndTime();
    }
}