package org.helmo.planclap_admin.presentations.presenters;

import org.helmo.planclap_admin.domains.exceptions.RepositoryException;
import org.helmo.planclap_admin.domains.core.MoviesToPlan;
import org.helmo.planclap_admin.domains.iservices.PublishMovieEvents;
import org.helmo.planclap_admin.domains.iservices.GetMoviesToPlan;
import org.helmo.planclap_admin.domains.iservices.MovieEventSubscriber;
import org.helmo.planclap_admin.domains.iservices.Executable;
import org.helmo.planclap_admin.presentations.iview.DisplayMoviesToPlanPresenterView;
import org.helmo.planclap_admin.presentations.mapper.MoviesToPlanMapper;

public class DisplayMoviesToPlanPresenter implements Executable, MovieEventSubscriber {

    private final GetMoviesToPlan services;
    private final DisplayMoviesToPlanPresenterView view;

    private DisplayMoviesToPlanPresenter(GetMoviesToPlan services, DisplayMoviesToPlanPresenterView view, PublishMovieEvents publisher) {
        this.services = services;
        this.view = view;
        publisher.subscribe(this);
    }

    public static DisplayMoviesToPlanPresenter of(GetMoviesToPlan services, DisplayMoviesToPlanPresenterView view, PublishMovieEvents publisher) {
        return new DisplayMoviesToPlanPresenter(services, view, publisher);
    }

    @Override
    public void execute() {
        try {
            MoviesToPlan moviesToPlan = services.getMoviesToPlan();

            view.displayMoviesToPlan(MoviesToPlanMapper.mapToVM(moviesToPlan));
            view.displayTotalDuration(moviesToPlan.getTotalDuration().toHoursFormat());
        } catch (RepositoryException e) {
            view.displayErrorMessage("Une erreur est survenue lors de la recuperation des films.");
        }
    }

    @Override
    public void onMovieAdded() {
        execute();
    }

}
