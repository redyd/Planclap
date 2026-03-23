package org.helmo.planclap_admin.domains.core;

import java.time.LocalDateTime;
import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;

/**
 * Classe de gestion de points d'un planning
 */
public final class PlanningScorer {

    private static final int SCORE_AGE = 100;
    private static final int SCORE_EXCEED = 45;
    private static final int SCORE_MULTIPLE_IN_DAY = 25;

    /**
     * Calcule les points d'âges d'une journée de planning.
     *
     * @param list la liste des films de la journée
     * @param breakPoint l'heure de jonction entre les films pour enfants et les films pour adulte
     * @return le score
     */
    public int scoreForAge(List<PlannedMovie> list, LocalDateTime breakPoint) {
        return list.stream()
                .mapToInt(movie -> {
                    boolean children = movie.isForChildren() && movie.getStartTime().isAfter(breakPoint);
                    boolean adult = !movie.isForChildren() && movie.getStartTime().isBefore(breakPoint);
                    return (children || adult) ? -SCORE_AGE : SCORE_AGE;
                })
                .sum();
    }

    /**
     * Calcule les points pour le dépassement d'horaire d'une journée de planning.
     *
     * @param list la liste des films de la journée
     * @param endOfDay l'heure de fin d'une journée
     * @return le score
     */
    public int scoreForExceed(List<PlannedMovie> list, LocalDateTime endOfDay) {
        if (list.isEmpty()) {
            return 0;
        }
        var last = list.getLast();
        return last.getStartTime().isBefore(endOfDay) ? SCORE_EXCEED : -SCORE_EXCEED;
    }

    /**
     * Calcule les points pour la duplication d'un film dans une journée.
     *
     * @param list la liste des films de la journée
     * @return le score
     */
    public int scoreForMultiple(List<PlannedMovie> list) {
        final int tolerance = 1;
        Map<Name, Long> counts = list.stream()
                .collect(Collectors.groupingBy(PlannedMovie::getSlug, Collectors.counting()));

        long score = 0;
        for (long count : counts.values()) {
            if (count > tolerance) {
                score -= (count - tolerance) * SCORE_MULTIPLE_IN_DAY;
            }
            else {
                score += SCORE_MULTIPLE_IN_DAY;
            }
        }
        return (int) score;
    }
}
