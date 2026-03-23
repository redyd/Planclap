package org.helmo.planclap_admin.domains.core;

import java.util.*;

/**
 * Maintient un planning cohérent
 */
public class Planning {

    private final Map<WeekDay, List<PlannedMovie>> week;
    private final PlanningTimeManager timeManager;
    private final PlanningScorer scorer = new PlanningScorer();

    /**
     * Instancie un planning avec tous les jours de la semaine.
     *
     * @param manager le gérant des dates
     */
    public Planning(PlanningTimeManager manager) {
        this.timeManager = manager;

        week = new HashMap<>();
        for (WeekDay w : WeekDay.values()) {
            week.put(w, new LinkedList<>());
        }
    }

    public long epochOfBeginning() {
        return week.values().stream()
                .flatMap(List::stream)
                .mapToLong(PlannedMovie::getEpochOfStart)
                .min()
                .orElse(0);
    }

    public long epochOfEnd() {
        return week.values().stream()
                .flatMap(List::stream)
                .mapToLong(PlannedMovie::getEpochOfEnd)
                .max()
                .orElse(0);
    }

    /**
     * Retourne la liste des films planifiés pour un jour donné.
     * La liste retournée est non modifiable.
     *
     * @param day le jour pour lequel obtenir les films planifiés
     * @return la liste non modifiable des films planifiés pour le jour donné
     */
    public List<PlannedMovie> getAt(WeekDay day) {
        return List.copyOf(week.get(day));
    }

    /**
     * Retourne le nombre total de films planifiés dans la semaine.
     *
     * @return le nombre total de films planifiés
     */
    public int size() {
        int size = 0;
        for (List<PlannedMovie> movies : week.values()) {
            size += movies.size();
        }
        return size;
    }

    /**
     * Ajoute un film en respectant les horaires et la gestion des quarts d'heure.
     *
     * @param day   le jour pour lequel ajouter le film
     * @param movie le film à ajouter
     * @return {@code true si l'ajout a réussi}, sinon {@code false}
     */
    public boolean add(WeekDay day, Movie movie) {
        if (week.get(day).isEmpty()) {
            var startTime = timeManager.startOfDay(day);
            // vérifie que même le premier film ne dépasse pas
            return !timeManager.doesExceed(startTime, day)
                    && week.get(day).add(PlannedMovieFactory.from(movie, startTime));
        } else if (timeManager.doesExceed(timeManager.toNextQuarter(timeManager.getLastEndOfDay(getAt(day))), day)) {
            return false;
        } else {
            var startTime = timeManager.toNextQuarter(timeManager.getLastEndOfDay(getAt(day)));
            return week.get(day).add(PlannedMovieFactory.from(movie, startTime));
        }
    }

    /**
     * Permute deux films dans le planning en s'assurant que le planning reste valide.
     * Si la permutation n'est pas possible, le planning reste inchangé.
     *
     * @param day1 Jour de la semaine du premier film
     * @param pos1 Position du premier film dans la liste des films planifiés pour ce jour
     * @param day2 Jour de la semaine du second film
     * @param pos2 Position du second film dans la liste des films planifiés pour ce jour
     * @return {@code true} si la permutation a réussi, sinon {@code false}
     */
    public boolean permute(WeekDay day1, int pos1, WeekDay day2, int pos2) {
        int correctPos1 = ensureCorrectIndex(day1, pos1);
        int correctPos2 = ensureCorrectIndex(day2, pos2);

        if (day1 == day2) {
            return simplePermutation(day1, correctPos1, correctPos2);
        } else {
            return complexPermutation(day1, correctPos1, day2, correctPos2);
        }
    }

    /**
     * Calcule un score pour le planning.
     * Plus le score est élevé, plus le planning est considéré comme bon.
     *
     * @return le score du planning
     */
    public int score() {
        int score = 0;
        int i = 0;

        for (List<PlannedMovie> list : week.values()) {
            var day = WeekDay.values()[i++];
            score += scorer.scoreForAge(list, timeManager.getBreakpoint(day));
            score += scorer.scoreForExceed(list, timeManager.endOfDay(day));
            score += scorer.scoreForMultiple(list);
        }

        return size() == 0 ? score : score / size();
    }

    /**
     * Crée une copie profonde du planning.
     *
     * @return une copie du planning
     */
    public Planning copyOf() {
        Planning copy = new Planning(this.timeManager);

        for (WeekDay day : WeekDay.values()) {
            List<PlannedMovie> movies = new LinkedList<>();
            for (PlannedMovie movie : this.week.get(day)) {
                movies.add(movie.copyOf());
            }
            copy.week.put(day, movies);
        }

        return copy;
    }

    @Override
    public boolean equals(Object o) {
        if (!(o instanceof Planning planning)) {
            return false;
        }
        return Objects.equals(week, planning.week) && Objects.equals(timeManager.getBaseDate(), planning.timeManager.getBaseDate());
    }

    @Override
    public int hashCode() {
        return Objects.hash(week, timeManager.getBaseDate());
    }

    private int ensureCorrectIndex(WeekDay day, int i) {
        return Math.abs(i) % getAt(day).size();
    }

    private boolean simplePermutation(WeekDay day1, int pos1, int pos2) {
        var movies = new LinkedList<>(week.get(day1));

        PlannedMovie m1 = movies.get(pos1);
        PlannedMovie m2 = movies.get(pos2);

        movies.set(pos1, m2);
        movies.set(pos2, m1);

        var corrected = correctDay(movies, day1);

        if (doesExceed(day1, corrected)) {
            return false;
        }

        week.put(day1, corrected);
        return true;
    }

    private boolean complexPermutation(WeekDay day1, int pos1, WeekDay day2, int pos2) {
        var moviesDay1 = new ArrayList<>(week.get(day1));
        var moviesDay2 = new ArrayList<>(week.get(day2));

        PlannedMovie m1 = moviesDay1.get(pos1);
        PlannedMovie m2 = moviesDay2.get(pos2);

        moviesDay1.set(pos1, m2);
        moviesDay2.set(pos2, m1);

        var corrected1 = correctDay(moviesDay1, day1);
        var corrected2 = correctDay(moviesDay2, day2);

        if (oneOfBothExceed(day1, day2, corrected1, corrected2)) {
            return false;
        }

        week.put(day1, corrected1);
        week.put(day2, corrected2);
        return true;
    }

    private boolean oneOfBothExceed(WeekDay day1, WeekDay day2, List<PlannedMovie> corrected1, List<PlannedMovie> corrected2) {
        return doesExceed(day1, corrected1) || doesExceed(day2, corrected2);
    }

    private boolean doesExceed(WeekDay day1, List<PlannedMovie> corrected1) {
        for (PlannedMovie movie : corrected1) {
            if (timeManager.doesExceed(movie.getStartTime(), day1)) {
                return true;
            }
        }
        return false;
    }

    private List<PlannedMovie> correctDay(List<PlannedMovie> movies, WeekDay day) {
        if (movies.isEmpty()) {
            return movies;
        }

        List<PlannedMovie> correctedMovies = new ArrayList<>();
        var currentStart = timeManager.startOfDay(day);

        for (PlannedMovie movie : movies) {
            PlannedMovie corrected = new PlannedMovie(
                    currentStart,
                    currentStart.plusMinutes(movie.getDuration()),
                    movie.getSlug(),
                    movie.isForChildren()
            );

            correctedMovies.add(corrected);

            currentStart = timeManager.toNextQuarter(corrected.getEndTime());
        }

        return correctedMovies;
    }
}