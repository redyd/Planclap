package org.helmo.planclap_admin.presentations.helper;

import org.helmo.planclap_admin.domains.core.*;
import org.helmo.planclap_admin.presentations.iview.AddMovieToPlanPresenterView;

import java.util.function.Predicate;


public class MovieDataValidator {

    private final AddMovieToPlanPresenterView view;

    public MovieDataValidator(AddMovieToPlanPresenterView view) {
        this.view = view;
    }

    public Name getValidatedTitle(Predicate<Name> slugValidator) {
        try {
            Name name = Name.of(view.getTitle());

            if (!slugValidator.test(name.toMinimal())) {
                view.displayErrorMessage("Nom de film deja existant");
                return getValidatedTitle(slugValidator);
            }

            return name;
        } catch (IllegalArgumentException e) {
            view.displayErrorMessage("Le nom ne peut pas etre vide");
            return getValidatedTitle(slugValidator);
        }
    }

    public MovieDuration getValidatedDuration() {
        try {
            return MovieDuration.of(view.getDuration());
        } catch (IllegalArgumentException e) {
            view.displayErrorMessage("La duree du film doit etre comprise entre 1 et 240 minutes.");
            return getValidatedDuration();
        }
    }

    public PosterURI getValidatedPoster() {
        try {
            return PosterURI.of(view.getPosterURI());
        } catch (IllegalArgumentException e) {
            view.displayErrorMessage("L'URL ne peut pas etre nulle");
            return getValidatedPoster();
        }
    }

    public MovieDescription getValidatedDescription() {
        try {
            return MovieDescription.of(view.getDescription());
        } catch (IllegalArgumentException e) {
            view.displayErrorMessage("La description d'un film doit contenir entre 1 et 200 caracteres.");
            return getValidatedDescription();
        }
    }
}