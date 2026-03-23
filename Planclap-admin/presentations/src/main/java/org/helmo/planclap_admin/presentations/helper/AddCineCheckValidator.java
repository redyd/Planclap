package org.helmo.planclap_admin.presentations.helper;

import org.helmo.planclap_admin.domains.core.CineCheckAge;
import org.helmo.planclap_admin.domains.core.CineCheckGroup;
import org.helmo.planclap_admin.presentations.iview.AddMovieToPlanPresenterView;
import org.helmo.planclap_admin.presentations.mapper.CineCheckMapper;

public class AddCineCheckValidator {

    private final AddMovieToPlanPresenterView view;

    public AddCineCheckValidator(AddMovieToPlanPresenterView view) {
        this.view = view;
    }

    public CineCheckGroup getValidatedCineChecks() {
        var age = getValidatedAge();
        var checks = CineCheckMapper.map(view.getCineChecks(CineCheckMapper.mapToVM()));

        return CineCheckGroup.of(age, checks);
    }

    private CineCheckAge getValidatedAge() {
        try {
            return CineCheckMapper.map(view.getCineCheckAge(CineCheckMapper.mapAgeToVM()));
        } catch (IllegalArgumentException e) {
            view.displayErrorMessage("L'age fourni est invalide");
            return getValidatedAge();
        }
    }
}