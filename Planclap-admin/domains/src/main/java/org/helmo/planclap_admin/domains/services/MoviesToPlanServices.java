package org.helmo.planclap_admin.domains.services;

import org.helmo.planclap_admin.domains.core.Movie;
import org.helmo.planclap_admin.domains.core.MovieSessions;
import org.helmo.planclap_admin.domains.core.Name;
import org.helmo.planclap_admin.domains.core.MoviesToPlan;
import org.helmo.planclap_admin.domains.exceptions.RepositoryException;
import org.helmo.planclap_admin.domains.iservices.PublishMovieEvents;
import org.helmo.planclap_admin.domains.iservices.MoviesAction;
import org.helmo.planclap_admin.domains.repository.MoviesToPlanRepository;

import java.util.Optional;

public class MoviesToPlanServices extends Services implements MoviesAction {

    private final MoviesToPlanRepository moviesToPlanRepository;
    private final PublishMovieEvents notifier;

    public MoviesToPlanServices(MoviesToPlanRepository moviesToPlanRepository, PublishMovieEvents notifier) {
        this.moviesToPlanRepository = moviesToPlanRepository;
        this.notifier = notifier;
    }

    @Override
    public void addMovieToPlan(MovieSessions movieSessions) throws RepositoryException {
        moviesToPlanRepository.addMovieToPlan(movieSessions, getNextMonday());
        notifier.notifySubscribers();
    }

    @Override
    public boolean slugExists(Name slug) throws RepositoryException {
        return moviesToPlanRepository.isAlreadyPlanned(slug, getNextMonday());
    }

    @Override
    public boolean canAddMovieToPlan(MovieSessions movieSessions) throws RepositoryException {
        return moviesToPlanRepository.getMoviesToPlan(getNextMonday()).canAdd(movieSessions);
    }

    @Override
    public Optional<Movie> isAlreadyDefine(Name slug) throws RepositoryException {
        return moviesToPlanRepository.getOnlyMovie(slug);
    }

    @Override
    public MoviesToPlan getMoviesToPlan() throws RepositoryException {
        return moviesToPlanRepository.getMoviesToPlan(getNextMonday());
    }

    @Override
    public Optional<MovieSessions> searchMovieToPlan(Name slug) throws RepositoryException {
        return moviesToPlanRepository.getMoviesToPlan(getNextMonday()).search(slug);
    }

}
