package org.helmo.planclap_admin.domains.iservices;

import org.helmo.planclap_admin.domains.core.*;
import org.helmo.planclap_admin.domains.exceptions.RepositoryException;

import java.util.Optional;

public interface AddMovieToPlan {

    /**
     * Permet d'ajouter un film à planifier.
     *
     * @param movieSessions le film à planifier
     * @throws RepositoryException                                                si il y a un problème lors de l'enregistrement
     * @throws org.helmo.planclap_admin.domains.exceptions.TooMuchMoviesException si il y a trop de film à planifier
     */
    void addMovieToPlan(MovieSessions movieSessions) throws RepositoryException;

    /**
     * Permet de vérifier si un film existe basé sur son slug.
     *
     * @param slug le slug du film
     * @return {@code true} si le film existe, sinon {@code false}
     * @throws RepositoryException si il y a un problème lors de l'enregistrement du film
     */
    boolean slugExists(Name slug) throws RepositoryException;

    /**
     * Permet de savoir si on peut ajouter un film à planifier.
     *
     * @param movieSessions le film à ajouter
     * @return {@code true} si on peut ajouter le film, sinon {@code false}
     * @throws RepositoryException si il y a un problème lors de l'enregistrement du film
     */
    boolean canAddMovieToPlan(MovieSessions movieSessions) throws RepositoryException;

    /**
     * Permet de récupérer un film basé sur son slug s'il est déjà enregistré par le système.
     *
     * @param slug le slug du film
     * @return un optionnel contenant ou non le film
     * @throws RepositoryException si un problème survient lors de la récupération du film
     */
    Optional<Movie> isAlreadyDefine(Name slug) throws RepositoryException;
}
