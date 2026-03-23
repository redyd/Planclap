package org.helmo.planclap_admin.domains.core;

import org.helmo.planclap_admin.domains.exceptions.TooMuchMoviesException;

import java.time.LocalDate;
import java.util.*;

/**
 * Classe représentant tous les films à planifier pour une date donnée
 */
public class MoviesToPlan {

    private final static int MAX_HOURS_TO_PLAN = 77;
    private final static int MIN_HOURS_TO_PLAN = 70;

    private final Set<MovieSessions> movieSessions;
    private final LocalDate dateForPlan;
    private final SlugSearcher searcher;

    public MoviesToPlan(LocalDate dateForPlan) {
        this.movieSessions = new HashSet<>();
        this.dateForPlan = dateForPlan;
        this.searcher = new SlugSearcher();
    }

    /**
     * Vérifie si la durée totale des films est en dessous de la limite minimale requise
     *
     * @return vrai si la durée totale est en dessous de la limite, sinon faux
     */
    public boolean belowPlanLimit() {
        return getTotalDuration().getHours() < MIN_HOURS_TO_PLAN;
    }

    /**
     * Retourne l'ensemble des sessions de films à planifier
     *
     * @return un ensemble non modifiable des sessions de films
     */
    public Set<MovieSessions> get() {
        return movieSessions;
    }

    /**
     * Retourne la date pour laquelle le plan est prévu
     *
     * @return la date du plan
     */
    public LocalDate getDateForPlan() {
        return dateForPlan;
    }

    /**
     * Ajoute une session de films si la durée totale ne dépasse pas la limite
     *
     * @param movieSession à ajouter
     * @throws TooMuchMoviesException si on ne sait pas ajouter ce film
     */
    public void add(MovieSessions movieSession) {
        if (canAdd(movieSession)) {
            movieSessions.add(movieSession);
            searcher.AddMovieSessions(movieSession);
        } else {
            throw new TooMuchMoviesException("Impossible d'ajouter ce film: ce film déborde du planning ou le film existe deja.");
        }
    }

    /**
     * Calcule la durée totale des sessions de films
     *
     * @return la durée totale
     */
    public GlobalDuration getTotalDuration() {
        int i = 0;
        for (MovieSessions movieSession : movieSessions) {
            i += movieSession.getTotalDuration().get();
        }
        return GlobalDuration.of(i);
    }

    /**
     * Vérifie si la session de films peut être ajoutées
     *
     * @param movieSession à vérifier
     * @return vrai si l'ajout est authorisé
     */
    public boolean canAdd(MovieSessions movieSession) {
        return getTotalDuration().plus(movieSession.getTotalDuration()).getHours() < MAX_HOURS_TO_PLAN
                && !containsMovie(movieSession.movie().getSlug());
    }

    /**
     * Permet de vérifier si un film existe dans un MoviesToPlan
     *
     * @param slug le slug à vérifier
     * @return {@code true} si le film existe déjà, sinon {@code false}
     */
    public boolean containsMovie(Name slug) {
        return movieSessions.stream().anyMatch(movie -> movie.equalsOnSlug(slug));
    }

    /**
     * Permet de faire une recherche intelligente basée sur le slug
     *
     * @param slug Slug du film
     * @return Un optionnel contenant le film, ou vide si pas de résultat
     */
    public Optional<MovieSessions> search(Name slug) {
        return searcher.search(slug);
    }

}
