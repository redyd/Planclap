package org.helmo.planclap_admin.domains.repository;

import org.helmo.planclap_admin.domains.core.Movie;
import org.helmo.planclap_admin.domains.core.MovieSessions;
import org.helmo.planclap_admin.domains.core.Name;
import org.helmo.planclap_admin.domains.exceptions.RepositoryException;
import org.helmo.planclap_admin.domains.core.MoviesToPlan;

import java.time.LocalDate;
import java.util.Optional;

public interface MoviesToPlanRepository {

    /**
     * Récupère les films à planifier.
     *
     * @param date la date des films à planifier
     * @return les films à planifier
     * @throws RepositoryException si une erreur survient lors de la lecture des données
     */
    MoviesToPlan getMoviesToPlan(LocalDate date) throws RepositoryException;

    /**
     * Ajoute un film à planifier.
     *
     * @param movieSessions la session de film (film + nombre de séance)
     * @param date          la date du film à planifier
     * @throws RepositoryException si une erreur survient lors de la lecture des données
     */
    void addMovieToPlan(MovieSessions movieSessions, LocalDate date) throws RepositoryException;

    /**
     * Vérifie si un film existe basé sur son slug.
     *
     * @param slug le slug du film à rechercher
     * @param date la date
     * @return {@code true} si le film existe déjà, sinon {@code false}
     * @throws RepositoryException si une erreur survient lors de la lecture des données
     */
    boolean isAlreadyPlanned(Name slug, LocalDate date) throws RepositoryException;

    Optional<Movie> getOnlyMovie(Name slug) throws RepositoryException;
}
