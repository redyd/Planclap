package org.helmo.planclap_admin.presentations.helper;

import org.helmo.planclap_admin.domains.core.*;
import org.helmo.planclap_admin.presentations.iview.AddMovieToPlanPresenterView;

import java.util.function.Predicate;

public class MovieBuilder {

    private final AddMovieToPlanPresenterView view;
    private final MovieDataValidator dataValidator;
    private final AddCineCheckValidator addCineCheckValidator;

    public MovieBuilder(AddMovieToPlanPresenterView view) {
        this.view = view;
        this.dataValidator = new MovieDataValidator(view);
        this.addCineCheckValidator = new AddCineCheckValidator(view);
    }

    public Name buildTitle(Predicate<Name> slugValidator) {
        return dataValidator.getValidatedTitle(slugValidator);
    }

    public Movie build(Name title) {
        var duration = dataValidator.getValidatedDuration();
        var poster = dataValidator.getValidatedPoster();
        var description = dataValidator.getValidatedDescription();
        var cineChecks = addCineCheckValidator.getValidatedCineChecks();

        return new Movie(title, description, duration, poster, cineChecks);
    }

    public SessionAmount getValidatedSessionAmount() {
        try {
            return SessionAmount.of(view.getSessionQuantity());
        } catch (IllegalArgumentException e) {
            view.displayErrorMessage("Le nombre de seances doit être compris entre 1 et 9.");
            return getValidatedSessionAmount();
        }
    }
}